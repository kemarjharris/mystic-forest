using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zenject;

public class JointController : MonoBehaviour, IPlayerController
{
    IBattler battler;
    // Use to move the battler
    IBattlerPhysics physics;
    // Data about how the battler moves
    public BattlerSpeed speeds;
    // input services
    public IUnityAxisService service;
    public IUnityInputService inputService;
    // smoothing for movement
    public float smoothTime = 0.1f;
    // physics values
    float horizontal;
    float vertical;
    float hVel;
    float vVel;
    bool jumped;

    [Inject]
    public void Construct(IUnityAxisService axisService, IUnityInputService inputService)
    {
        service = axisService;
        this.inputService = inputService;
    }

    private void Awake()
    {
        battler = GetComponent<IBattler>();
        physics = GetComponent<IBattlerPhysics>();
        // if (service == null) service = new UnityAxisService();
        // if (inputService == null) inputService = new UnityInputService();
        battler.eventSet.onPlayerSwitchedIn += Enable;
        battler.eventSet.onPlayerSwitchedOut += Disable;
    }

    private void OnDestroy()
    {
        battler.eventSet.onPlayerSwitchedIn -= Enable;
        battler.eventSet.onPlayerSwitchedOut -= Disable;
    }

    public void Update()
    {
        // no input during combat
        if (battler.executionState.combatState != CombatState.ATTACKING)
        {
            if (physics.IsGrounded)
            {
               
                horizontal = service.GetAxis("Horizontal");
                vertical = service.GetAxis("Vertical");
                if (inputService.GetKeyDown("space"))
                {
                    jumped = true;
                }
            }
        }
        else
        {
            
            horizontal = Mathf.SmoothDamp(horizontal, 0, ref hVel, smoothTime);
            vertical = Mathf.SmoothDamp(vertical, 0, ref vVel, smoothTime); ;
        }
       
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
        else if (physics.IsGrounded && battler.executionState.combatState == CombatState.NOT_ATTACKING)
        {
            // move
            physics.SetVelocity(new Vector3(horizontal, 0, battler.executionState.comboing ? 0 : vertical) * speeds.speed);
        }
    }


    void Enable() => enabled = true;
    void Disable() => enabled = false;

    /* for testing */

    public void SetStateAttacking()
    {
        battler.executionState.combatState = CombatState.ATTACKING;
    }

    public void SetStateAbleToCancelAttack()
    {
        battler.executionState.combatState = CombatState.ABLE_TO_CANCEL_ATTACK;
    }

    public void SetStateNotAttacking()
    {
        battler.executionState.combatState = CombatState.NOT_ATTACKING;
    }
}
