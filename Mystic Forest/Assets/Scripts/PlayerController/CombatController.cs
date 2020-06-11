using UnityEngine;
using System.Collections;

public class CombatController : IPlayerController
{
    IBattler battler;
    IBattlerPhysics physics;
    IExecutionModule module;
    BattlerSpeed speeds;
    CombatState state;
    IMainPlayerController mainController;
    public IUnityAxisService service;
    bool groundedLastFrame;
    float horizontal;
    float vertical;
    bool jumped;

    public CombatController(IBattler battler, IBattlerPhysics physics, IExecutionModule module, BattlerSpeed speeds, IMainPlayerController mainController)
    {
        this.battler = battler;
        this.physics = physics;
        this.module = module;
        this.speeds = speeds;
        this.mainController = mainController;
        state = CombatState.NOT_ATTACKING;
        groundedLastFrame = physics.IsGrounded;
        if (service == null) service = new UnityAxisService();
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

                // jump when attack is cancellable, jump cancel
                if (Input.GetKeyDown("x"))
                {
                    jumped = true;
                    module.ChangeSet(Aerials());
                }
            }
        } else
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
            
            if (state == CombatState.ABLE_TO_CANCEL_ATTACK)
            {
                physics.SetVelocity(new Vector3(speeds.jumpHorizontalForce * horizontal, speeds.jumpForce));
                jumped = false;
            } else if (state == CombatState.NOT_ATTACKING)
            {
                physics.SetVelocity(new Vector3(speeds.jumpHorizontalForce * horizontal, speeds.jumpForce, speeds.jumpHorizontalForce * vertical));
                jumped = false;
            }
            
            
        } else if (state == CombatState.NOT_ATTACKING && physics.IsGrounded)
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
        //physics.lockZ = true;
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
        //physics.lockZ = false;
    }

    void OnNewChainLoaded(ICustomizableEnumerator<IExecutable> obj) => state = CombatState.ATTACKING;
    void OnChainCancellable() => state = CombatState.ABLE_TO_CANCEL_ATTACK;
    void OnChainFinished()
    {
        battler.StopCombatAnimation();
        state = CombatState.NOT_ATTACKING;
        mainController.SwapToNeutralMode();
        //StartModuleExecution();
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
