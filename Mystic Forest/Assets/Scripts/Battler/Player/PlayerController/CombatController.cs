using UnityEngine;
using System.Collections;

public class CombatController : IPlayerController
{
    IBattler battler;
    BattlerPhysicsZ physics;
    IExecutionModule module;
    BattlerSpeed speeds;
    CombatState state;
    bool groundedLastFrame;
    float horizontal;
    bool jumped;

    public CombatController(IBattler battler, BattlerPhysicsZ physics, IExecutionModule module, BattlerSpeed speeds)
    {
        this.battler = battler;
        this.physics = physics;
        this.module = module;
        this.speeds = speeds;
        state = CombatState.NOT_ATTACKING;
        groundedLastFrame = physics.IsGrounded;
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
                horizontal = Input.GetAxis("Horizontal");
                // jump when attack is cancellable, jump cancel
                if (state != CombatState.ATTACKING && Input.GetAxis("Vertical") > 0)
                {
                    jumped = true;
                    module.ChangeSet(Aerials());
                }
            }
        } else
        {
            horizontal = 0;
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
            physics.SetVelocity(new VectorZ(speeds.jumpHorizontalForce * horizontal, 0), speeds.jumpForce);
            jumped = false;
        } else if (physics.IsGrounded)
        {
            // move
            physics.SetVelocity(new VectorZ(horizontal * speeds.speed, 0), 0);
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
        physics.lockZ = true;
        module.OnNewChainLoaded.AddAction(OnNewChainLoaded);
        module.OnChainCancellable.AddAction(OnChainCancellable);
        module.OnChainFinished.AddAction(OnChainFinished);
        StartModuleExecution();
    }

    public void OnDisable()
    {
        module.OnNewChainLoaded.RemoveAction(OnNewChainLoaded);
        module.OnChainCancellable.RemoveAction(OnChainCancellable);
        module.OnChainFinished.RemoveAction(OnChainFinished);
        physics.lockZ = false;
    }

    void OnNewChainLoaded(ICustomizableEnumerator<IExecutable> obj) => state = CombatState.ATTACKING;
    void OnChainCancellable() => state = CombatState.ABLE_TO_CANCEL_ATTACK;
    void OnChainFinished()
    {
        battler.StopCombatAnimation();
        state = CombatState.NOT_ATTACKING;
        StartModuleExecution();
    }
}
