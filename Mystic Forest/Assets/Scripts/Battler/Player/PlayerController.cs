using UnityEngine;
using UnityEditor;
using System;

public class PlayerController : MonoBehaviour, IPlayerController
{

    BattlerPhysicsZ physics;
    bool inCombat;
    CombatState state { get; set; }
    public BattlerSpeed speeds;
    IBattler battler;
    IExecutionModule module;
    bool groundedLastFrame;

    private void Awake()
    {
        physics = GetComponent<BattlerPhysicsZ>();
        battler = GetComponent<IBattler>();
        module = FindObjectOfType<ExecutionModule>();
        if (module == null) module = new GameObject("Execution Module").AddComponent<ExecutionModule>();
        groundedLastFrame = physics.IsGrounded;
    }

    private void OnEnable()
    {
        module.OnNewChainLoaded.AddAction(OnNewChainLoaded);
        module.OnChainCancellable.AddAction(OnChainCancellable);
        module.OnChainFinished.AddAction(OnChainFinished);
    }

    private void OnDisable()
    {
        module.OnNewChainLoaded.RemoveAction(OnNewChainLoaded);
        module.OnChainCancellable.RemoveAction(OnChainCancellable);
        module.OnChainFinished.RemoveAction(OnChainFinished);
    }

    void OnNewChainLoaded(ICustomizableEnumerator<IExecutable> obj) => state = CombatState.ATTACKING;
    void OnChainCancellable() => state = CombatState.ABLE_TO_CANCEL_ATTACK;
    void OnChainFinished() {
        battler.StopCombatAnimation();
        state = CombatState.NOT_ATTACKING;
        StartModuleExecution();
    }

    public void Update()
    {
        if (state != CombatState.NOT_ATTACKING) return;
        if (Input.GetKeyDown("space"))
        {
            if (inCombat)
            {
                FinishCombat();
            }
            else
            {
                StartCombat();
            }
        }
    }

    public void FinishCombat()
    {
        
        physics.lockZ = false;
        inCombat = false;
    }

    public void StartCombat()
    {
        physics.lockZ = true;
        inCombat = true;
        StartModuleExecution();
    }

    public void FixedUpdate()
    {
        if (inCombat)
        {
            CombatFixedUpdate();
        }
        else
        {
            NeutralFixedUpdate();
        }
    }

    public void NeutralFixedUpdate()
    {
        if (physics.IsGrounded)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            physics.SetVelocity(new VectorZ(horizontal, vertical) * speeds.speed, 0);
        }
    }

    public void CombatFixedUpdate()
    {
        if (!groundedLastFrame && physics.CloseToGround)
        {
            module.ChangeSet(NonAerials());
        }

        // no input during combat
        if (state != CombatState.ATTACKING)
        {


            if (physics.IsGrounded)
            {
                float horizontal = Input.GetAxis("Horizontal");
                // jump when attack is cancellable, jump cancel
                if (state != CombatState.ATTACKING && Input.GetAxis("Vertical") > 0)
                {
                    // jump
                    if (horizontal > 0) horizontal = 1;
                    else if (horizontal < 0) horizontal = -1;
                    physics.SetVelocity(new VectorZ(speeds.jumpHorizontalForce * horizontal, 0), speeds.jumpForce);

                    module.ChangeSet(Aerials());

                }
                else if (state == CombatState.NOT_ATTACKING)
                {
                    // move
                    physics.SetVelocity(new VectorZ(horizontal * speeds.speed, 0), 0);
                }
            }
        }

        groundedLastFrame = physics.CloseToGround;
        Debug.Log("grounded last frame" + groundedLastFrame);

        
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
}