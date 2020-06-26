using UnityEngine;
using System.Collections;

public class JointController : MonoBehaviour, IPlayerController
{
    IBattler battler;
    IBattlerPhysics physics;
    IExecutionModule module;
    public BattlerSpeed speeds;
    CombatState state;
    public IUnityAxisService service;
    public IUnityInputService inputService;
    bool groundedLastFrame;
    public float smoothTime = 0.1f;
    float horizontal;
    float vertical;
    float hVel;
    float vVel;
    bool jumped;
    bool executing = true;
    bool comboing = false;

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
    }

    public void Update()
    {
        if (!groundedLastFrame && physics.IsGrounded)
        {
            module.ChangeSet(comboing? Skills() : StateNormals());
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
        module.StartExecution(comboing ? Skills() : StateNormals(), battler);
        executing = true;
    }

    IExecutableChainSet StateNormals()
    {
        bool StateNormal(IExecutableChain chain) => !chain.IsSkill && physics.IsGrounded ? !chain.IsAerial : chain.IsAerial;
        return battler.ChainSet.Where(StateNormal);
    }


    IExecutableChainSet Aerials()
    {
        // Aerial sets
        bool IsAerial(IExecutableChain chain) => chain.IsAerial && !chain.IsSkill;
        return battler.ChainSet.Where(IsAerial);
    }

    IExecutableChainSet Normals()
    {
        bool IsNormal(IExecutableChain chain) => !chain.IsAerial && !chain.IsSkill;
        return battler.ChainSet.Where(IsNormal);
    }

    IExecutableChainSet Skills()
    {
        bool IsSkill(IExecutableChain chain) => physics.IsGrounded ? !chain.IsAerial : chain.IsAerial;
        return battler.ChainSet.Where(IsSkill);
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
