using UnityEngine;
using UnityEditor;
using System;

public class PlayerController : MonoBehaviour, IPlayerController
{

    BattlePhysicsZ physics;
    bool inCombat;
    CombatState state { get; set; }
    public BattlerSpeed speeds;
    IBattler battler;
    IExecutionModule module;

    private void Awake()
    {
        physics = GetComponent<BattlePhysicsZ>();
        battler = GetComponent<IBattler>();
        module = FindObjectOfType<ExecutionModule>();
        if (module == null) module = new GameObject("Execution Module").AddComponent<ExecutionModule>();
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
    }

    public void Update()
    {

        Debug.Log(state);
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
        module.StartExecution(battler.ChainSet, battler);
        physics.lockZ = true;
        inCombat = true;
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
        // no input during combat
        if (state == CombatState.ATTACKING) return;

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
            }
            else if (state == CombatState.NOT_ATTACKING)
            {
                // move
                physics.SetVelocity(new VectorZ(horizontal * speeds.speed, 0), 0);
            }
        }

    }




}