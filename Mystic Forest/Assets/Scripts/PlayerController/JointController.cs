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
    public float smoothTime = 0.1f;
    float horizontal;
    float vertical;

    float hVel;
    float vVel;

    bool jumped;
    bool lockingOn;

    public ExecutableChainSO jumpIn;


    private static LockOnController lockOn;
    // private GameObject lockedOn;
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
            lockOn = Instantiate(Resources.Load<GameObject>("Prefabs/Miscellaneous/Lock On Area")).GetComponent<LockOnController>();
            lockOn.rule = (Collider collider) => collider.gameObject.tag == "Battler";
            lockOn.onStartLockOn += OnStartLockOn;
            lockOn.onTargetSelected += OnTargetSelected;
            
            lockOn.AttachToBattler(battler);
        }

    }

    public void Update()
    {
        if (!groundedLastFrame && physics.IsGrounded)
        {
            module.ChangeSet(Normals());
        }

        // no input during combat
        if (state != CombatState.ATTACKING && !lockingOn)
        {
            if (physics.IsGrounded)
            {
                horizontal = service.GetAxis("Horizontal");
                vertical = service.GetAxis("Vertical");

                if (Input.GetKeyDown("k"))
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
            physics.SetVelocity(new Vector3(horizontal * speeds.speed, 0, vertical * speeds.speed));
        }
    }

    void StartModuleExecution()
    {
        lockOn.OnDisable();
        lockingOn = false;
        module.StartExecution(physics.IsGrounded ? Normals() : Aerials(), battler);
    }

    IExecutableChainSet Aerials()
    {
        // Aerial sets
        bool IsAerial(IExecutableChain chain) => chain.IsAerial;
        return battler.ChainSet.Where(IsAerial);
    }

    IExecutableChainSet Normals()
    {
        bool IsNotAerial(IExecutableChain chain) => !chain.IsAerial;
        return battler.ChainSet.Where(IsNotAerial);
    }

    public void OnEnable()
    {
        lockOn.OnEnable();
        lockingOn = false;
        module.OnNewChainLoaded.AddAction(OnNewChainLoaded);
        module.OnChainCancellable.AddAction(OnChainCancellable);
        module.OnChainFinished.AddAction(OnChainFinished);
        //StartModuleExecution();
    }

    public void OnDisable()
    {
        lockOn.OnDisable();
        module.OnNewChainLoaded.RemoveAction(OnNewChainLoaded);
        module.OnChainCancellable.RemoveAction(OnChainCancellable);
        module.OnChainFinished.RemoveAction(OnChainFinished);
    }

    void OnNewChainLoaded(ICustomizableEnumerator<IExecutable> obj) => state = CombatState.ATTACKING;
    void OnChainCancellable() => state = CombatState.ABLE_TO_CANCEL_ATTACK;
    void OnChainFinished()
    {
        battler.StopCombatAnimation();
        state = CombatState.NOT_ATTACKING;
        lockOn.OnEnable();
        lockingOn = false;
        //mainController.SwapToNeutralMode();
        // StartModuleExecution();
    }

    // lock on 
    void OnStartLockOn()
    {
        lockingOn = true;
    }

    void OnTargetSelected(GameObject target)
    {
        if (target != null)
        {
            lockOn.gameObject.SetActive(false);
            ITargetSet targetSet = new TargetSet();
            targetSet.SetTarget(target.transform);
            module.StartExecution(jumpIn, targetSet, battler);
        }
        lockingOn = false;
    }

    void OnDestroy()
    {
        lockOn.onStartLockOn -= OnStartLockOn;
        lockOn.onTargetSelected -= OnTargetSelected;
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
