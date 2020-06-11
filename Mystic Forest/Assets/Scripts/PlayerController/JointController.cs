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
    bool groundedLastFrame;
    float horizontal;
    float vertical;
    bool jumped;

    public ExecutableChainSO jumpIn;


    private static ClosestLockOn lockOn;
    private GameObject lockedOn;
    public float timeToHoldForLockOn = 0.2f;
    private float currentTime;

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
        state = CombatState.NOT_ATTACKING;
        groundedLastFrame = physics.IsGrounded;
        if (lockOn == null)
        {
            lockOn = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Miscellaneous/Closest Lock On Area")).GetComponent<ClosestLockOn>();
            lockOn.rule = (Collider collider) => collider.gameObject.tag == "Battler";
            lockOn.enabled = false;
        }

    }

    public void Update()
    {
        if (!groundedLastFrame && physics.IsGrounded)
        {
            module.ChangeSet(NonAerials());
        }

        // no input during combat
        if (state != CombatState.ATTACKING)
        {
            if (physics.IsGrounded)
            {
                horizontal = service.GetAxis("Horizontal");
                vertical = service.GetAxis("Vertical");

                LockOn();

                // jump when attack is cancellable, jump cancel
                if (Input.GetKeyDown("x"))
                {
                    jumped = true;
                    module.ChangeSet(Aerials());
                }
            }
        }
        else
        {
            horizontal = 0;
            vertical = 0;
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
            physics.SetVelocity(new Vector3(horizontal * speeds.speed, 0, vertical * speeds.speed));
        }
    }

    void StartModuleExecution() => module.StartExecution(physics.IsGrounded ? NonAerials() : Aerials(), battler);

    IExecutableChainSet Aerials()
    {
        // Aerial sets
        bool IsAerial(IExecutableChain chain) => chain.IsAerial;
        return battler.ChainSet.Where(IsAerial);
    }

    IExecutableChainSet NonAerials()
    {
        bool IsNotAerial(IExecutableChain chain) => !chain.IsAerial;
        return battler.ChainSet.Where(IsNotAerial);
    }

    public void OnEnable()
    {

        lockOn.enabled = false;
        lockOn.gameObject.SetActive(true);
        lockOn.gameObject.transform.SetParent(battler.gameObject.transform);
        lockOn.gameObject.transform.localPosition = Vector3.zero;
        lockOn.gameObject.transform.localScale = new Vector3(3.2f, 0.16f, 2.25f);

        module.OnNewChainLoaded.AddAction(OnNewChainLoaded);
        module.OnChainCancellable.AddAction(OnChainCancellable);
        module.OnChainFinished.AddAction(OnChainFinished);
        StartModuleExecution();
    }

    public void OnDisable()
    {
        lockOn.enabled = false;
        lockedOn = null;
        currentTime = 0;
        lockOn.gameObject.SetActive(false);

        module.OnNewChainLoaded.RemoveAction(OnNewChainLoaded);
        module.OnChainCancellable.RemoveAction(OnChainCancellable);
        module.OnChainFinished.RemoveAction(OnChainFinished);
        physics.lockZ = false;
    }

    void LockOn()
    {
        if (Input.GetKey("z"))
        {
            // Wait until time for leap in
            currentTime += Time.deltaTime;
            // Scan for leap in
            if (currentTime >= timeToHoldForLockOn && !lockOn.enabled)
            {
                lockOn.enabled = true;
            }
        }
        else if (Input.GetKeyUp("z"))
        {
            if (currentTime < timeToHoldForLockOn)
            {
                // switch to combat mode
                //mainController.SwapToCombatMode();
            }
            else
            {
                if (lockOn.lockedOn != null)
                {
                    lockedOn = lockOn.lockedOn;
                    Debug.Log(lockedOn.name);
                    // leap in at lockon on
                    TargetSet target = new TargetSet();
                    target.SetTarget(lockedOn.transform);
                    module.StartExecution(jumpIn, target, battler);
                    //battler.JumpIn(lockedOn.GetComponent<IBattler>());
                }
                lockOn.enabled = false;
                lockedOn = null;
            }
            currentTime = 0;
        }
    }

    void OnNewChainLoaded(ICustomizableEnumerator<IExecutable> obj) => state = CombatState.ATTACKING;
    void OnChainCancellable() => state = CombatState.ABLE_TO_CANCEL_ATTACK;
    void OnChainFinished()
    {
        battler.StopCombatAnimation();
        state = CombatState.NOT_ATTACKING;
        //mainController.SwapToNeutralMode();
        StartModuleExecution();
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
