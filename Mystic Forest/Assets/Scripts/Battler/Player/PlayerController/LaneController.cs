using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zenject;

public class LaneController : MonoBehaviour, IPlayerController
{
    public int startLane;
    int lane;
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
    float hVel;
    bool jumped;

    LaneBattlefield field;

    [Inject]
    public void Construct(IUnityAxisService axisService, IUnityInputService inputService, LaneBattlefield field)
    {
        service = axisService;
        this.inputService = inputService;
        this.field = field;
    }

    private void Start()
    {
        transform.position = field.bottomLeft + new Vector3(10, 0, field.NthLane(startLane));
        lane = startLane;
    }

    private void Awake()
    {
        battler = GetComponent<IBattler>();
        physics = GetComponent<BattlerPhysics>();
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

                float vAxis = DirectionalInput.GetAxisDown("Vertical");
                if (vAxis > 0 && lane < field.numberOfLanes - 1)
                {
                    transform.position += new Vector3(0, 0, field.spaceBetweenLanes);
                    lane++;
                } else if (vAxis < 0 && lane > 0)
                {
                    transform.position -= new Vector3(0, 0, field.spaceBetweenLanes);
                    lane--;
                }

                //vertical = service.GetAxis("Vertical");
                if (inputService.GetKeyDown("space"))
                {
                    jumped = true;
                }
            }
        }
        else
        {
            horizontal = Mathf.SmoothDamp(horizontal, 0, ref hVel, smoothTime);
            //vertical = Mathf.SmoothDamp(vertical, 0, ref vVel, smoothTime); ;
        }

    }

    public void FixedUpdate()
    {
        if (jumped && !physics.freeze)
        {
            // jump
            if (horizontal > 0) horizontal = 1;
            else if (horizontal < 0) horizontal = -1;


            physics.SetVelocity(new Vector3(speeds.jumpHorizontalForce * horizontal, speeds.jumpForce));
            jumped = false;
        }
        else if (physics.IsGrounded && battler.executionState.combatState == CombatState.NOT_ATTACKING)
        {
            // move
            physics.SetVelocity(new Vector3(horizontal, 0, 0) * speeds.speed);
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
