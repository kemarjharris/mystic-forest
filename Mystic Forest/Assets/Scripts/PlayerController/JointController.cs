using UnityEngine;
using System.Collections;

public class JointController : MonoBehaviour, IPlayerController
{
    // The battler being controlled
    IBattler battler;
    // Use to move the battler
    IBattlerPhysics physics;
    // Used for execution
    IExecutionModule module;
    // Data about how the battler moves
    public BattlerSpeed speeds;
    // What the battler is doing
    CombatState state;
    // input services
    public IUnityAxisService service;
    public IUnityInputService inputService;
    // flag for if the battler was airborne or not
    bool groundedLastFrame;
    // smoothing for movement
    public float smoothTime = 0.1f;
    // physics values
    float horizontal;
    float vertical;
    float hVel;
    float vVel;
    bool jumped;
    // if executor is active
    bool executing = true;
    // if the battler has a combo active
    bool comboing = false;
    // Currently targeted enemy
    ITargetSet target;

    public GameObject lockOnPrefab;
    GameObject lockOnReticle;

    bool lockedOn { get => lockOnReticle.gameObject.activeSelf; }
    bool selectingSkill;
    float skillTimeOut;

    public ExecutableChainSO jumpIn;
    IExecutableChainSet jumpInSet; 

    private void Awake()
    {

        battler = GetComponent<IBattler>();
        physics = GetComponent<BattlerPhysics>();
        GameObject moduleGO = GameObject.Find("Execution Module");
        if (moduleGO == null)
        {
            module = new GameObject("Execution Module").AddComponent<ExecutionModule>();
        }
        else
        {
            module = moduleGO.GetComponent<IExecutionModule>();
        }
        if (service == null) service = new UnityAxisService();
        if (inputService == null) inputService = new UnityInputService();
        state = CombatState.NOT_ATTACKING;
        groundedLastFrame = physics.IsGrounded;
        lockOnReticle = Instantiate(lockOnPrefab);
        lockOnReticle.SetActive(false);
        jumpInSet = new ExecutableChainSetImpl(new IExecutableChain[] { jumpIn });

    }

    public void Update()
    {
        if (!groundedLastFrame && physics.IsGrounded)
        {
            module.ChangeSet(!comboing && state == CombatState.NOT_ATTACKING ? JumpIn() : StateNormals());
        }

        if (state == CombatState.NOT_ATTACKING && !lockedOn && Input.GetKeyDown("l"))
        {
            skillTimeOut = 1;
            if (!selectingSkill)
            {
                StartCoroutine(SelectSkill());
            }
        }


        // no input during combat
        if (state != CombatState.ATTACKING)
        {
            if (physics.IsGrounded)
            {
                horizontal = service.GetAxis("Horizontal");
                vertical = service.GetAxis("Vertical");
                if (inputService.GetKeyDown("space"))
                {
                    jumped = true;
                    module.ChangeSet(Aerials());
                }
            }
        }
        else
        {
            
            horizontal = Mathf.SmoothDamp(horizontal, 0, ref hVel, smoothTime);
            vertical = Mathf.SmoothDamp(vertical, 0, ref vVel, smoothTime); ;
        }
        groundedLastFrame = physics.IsGrounded;
    }

    public void FixedUpdate()
    {
        if (jumped && !physics.freeze)
        {
            // jump
            if (horizontal > 0) horizontal = 1;
            else if (horizontal < 0) horizontal = -1;


            physics.SetVelocity(new Vector3(speeds.jumpHorizontalForce * horizontal, speeds.jumpForce, speeds.jumpHorizontalForce * vertical));
            jumped = false;
        }
        else if (physics.IsGrounded && state == CombatState.NOT_ATTACKING)
        {
            // move
            physics.SetVelocity(new Vector3(horizontal, 0, comboing ? 0 : vertical) * speeds.speed);
        }
    }

    public void LateUpdate()
    {
        if (!executing)
        {
            StartModuleExecution();
        }
    }

    void StartModuleExecution()
    {
        module.StartExecution(comboing ? StateNormals() : JumpIn(), battler, NewTargetSet());
        executing = true;
    }

    IExecutableChainSet StateNormals()
    {
        bool StateNormal(IExecutableChain chain) => physics.IsGrounded ? !chain.IsAerial : chain.IsAerial;
        return battler.ChainSet.Where(StateNormal);
    }

    IExecutableChainSet Aerials()
    {
        // Aerial sets
        bool IsAerial(IExecutableChain chain) => chain.IsAerial && !chain.IsSkill;
        return battler.ChainSet.Where(IsAerial);
    }

    IExecutableChainSet JumpIn()
    {
        if (!physics.IsGrounded)
        {
            return new ExecutableChainSetImpl(new IExecutableChain[0]);
        } else
        {
            return jumpInSet;
        }
    }

    public void OnEnable()
    {
        module.OnNewChainLoaded.AddAction(OnNewChainLoaded);
        module.OnChainCancellable.AddAction(OnChainCancellable);
        module.OnChainFinished.AddAction(OnChainFinished);
        SetUpComboEvents();
        StartModuleExecution();
    }

    public void OnDisable()
    {
        module.OnNewChainLoaded.RemoveAction(OnNewChainLoaded);
        module.OnChainCancellable.RemoveAction(OnChainCancellable);
        module.OnChainFinished.RemoveAction(OnChainFinished);
        TearDownComboEvents();
    }

    void OnNewChainLoaded(ICustomizableEnumerator<IExecutable> obj)
    {
        state = CombatState.ATTACKING;
        selectingSkill = false;
       
    }
    void OnChainCancellable() => state = CombatState.ABLE_TO_CANCEL_ATTACK;
    void OnChainFinished()
    {
        battler.StopCombatAnimation();
        state = CombatState.NOT_ATTACKING;
        executing = false;
    }

    void SetUpComboEvents()
    {
        GameObject counter = GameObject.FindGameObjectWithTag("Combo Counter");
        if (counter != null)
        {
            ComboCounter comboCounter = counter.GetComponent<ComboCounter>();
            comboCounter.onCountIncremented += RaiseComboFlag;
            comboCounter.onComboFinished += LowerComboFlag;
        }

    }

    void TearDownComboEvents()
    {
        GameObject counter = GameObject.FindGameObjectWithTag("Combo Counter");
        if (counter != null)
        {
            ComboCounter comboCounter = counter.GetComponent<ComboCounter>();
            comboCounter.onCountIncremented -= RaiseComboFlag;
            comboCounter.onComboFinished -= LowerComboFlag;
        }
    }

    void RaiseComboFlag(int i)
    {
        GetComponentInChildren<SpriteRenderer>().color = Color.cyan;
        comboing = true;
    }

    void LowerComboFlag()
    {
        GetComponentInChildren<SpriteRenderer>().color = Color.white;
        comboing = false;
        lockOnReticle.transform.parent = null;
        lockOnReticle.SetActive(false);
        StartModuleExecution();
    }

    ITargetSet NewTargetSet()
    {
        target = new EventTargetSet(delegate(Transform t) {
            lockOnReticle.SetActive(true);
            lockOnReticle.transform.SetParent(t);
            lockOnReticle.transform.localPosition = Vector3.zero;
        });
        return target;
    }

    IEnumerator SelectSkill()
    {
        selectingSkill = true;
        module.ChangeSet(StateNormals());
        GetComponentInChildren<SpriteRenderer>().color = Color.yellow;

        float fdt = Time.fixedDeltaTime;
        Time.timeScale = 0.1f;
        Time.fixedDeltaTime = Time.timeScale * fdt;

        while (skillTimeOut >= 0 && selectingSkill)
        {
            skillTimeOut -= Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = 1;
        Time.fixedDeltaTime = fdt;

        // timed out
        if (skillTimeOut < 0)
        {
            selectingSkill = false;
            module.ChangeSet(JumpIn());
        }

        GetComponentInChildren<SpriteRenderer>().color = Color.white;
    }

    /* for testing */

    public void SetStateAttacking()
    {
        state = CombatState.ATTACKING;
    }

    public void SetStateAbleToCancelAttack()
    {
        state = CombatState.ABLE_TO_CANCEL_ATTACK;
    }

    public void SetStateNotAttacking()
    {
        state = CombatState.NOT_ATTACKING;
    }
}
