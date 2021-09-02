using UnityEngine;
using System.Collections;
using Zenject;

public class TargetAxisMovementController : MonoBehaviour
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
    // range for valid movement input
    public float liveRange = 0.5f;
    // physics values
    float horizontal;
    float vertical;
    float horizontalRaw;
    float verticalRaw;
    float hVel;
    float vVel;
    bool jumped;
    ITargeter targeter;


    [Inject]
    public void Construct(IUnityAxisService axisService, IUnityInputService inputService, ITargeter targeter)
    {
        service = axisService;
        this.inputService = inputService;
        this.targeter = targeter;
    }

    private void Awake()
    {
        battler = GetComponent<IBattler>();
        physics = GetComponent<IBattlerPhysics>();
        battler.eventSet.onPlayerSwitchedIn += Enable;
        battler.eventSet.onPlayerSwitchedOut += Disable;
        targeter.onLockOn += RotateToTarget;
    }

    private void OnDestroy()
    {
        battler.eventSet.onPlayerSwitchedIn -= Enable;
        battler.eventSet.onPlayerSwitchedOut -= Disable;
        targeter.onLockOn -= RotateToTarget;
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
                horizontalRaw = service.GetAxisRaw("Horizontal");
                verticalRaw = service.GetAxisRaw("Vertical");

                if (inputService.GetKeyDown("space"))
                {
                    jumped = true;
                }

                // flip to face
                Transform target = targeter.targetSet.GetTarget();
                if (target != null)
                {
                    Vector3 faceVector = (target.position - transform.position).normalized;
                    transform.right = new Vector3(faceVector.x, 0, faceVector.z);
                }
            } else
            {
                horizontal = 0;
                vertical = 0;
                horizontalRaw = 0;
                verticalRaw = 0;
            }
        }

        if (physics.IsGrounded) RotateToTarget(targeter.targetSet.GetTarget());



    }

    public void FixedUpdate()
    {
        Vector3 right = Right().normalized;

        // jump
        if (jumped && !physics.freeze)
        {
            float h = 0;

            if (horizontalRaw > liveRange) h = 1;
            else if (horizontalRaw < -liveRange) h = -1;

            h *= speeds.jumpHorizontalForce;

            if (targeter.targetSet.GetTarget() != null)
            {
                physics.SetVelocity(Vector3.Scale(new Vector3(right.x, 1, right.z), new Vector3(h, speeds.jumpForce, h)));
                
            } else
            {
                float v = 0;

                if (verticalRaw > liveRange) v = 1;
                else if (verticalRaw < -liveRange) v = -1;
                
                v *= speeds.jumpHorizontalForce;

                physics.SetVelocity((right * h) + (Forward() * v) + (Vector3.up * speeds.jumpForce));

            }

            jumped = false;
        }
        else if (physics.IsGrounded && battler.executionState.combatState == CombatState.NOT_ATTACKING)
        {
            
            // move
            if (targeter.targetSet.GetTarget() != null)
            {
                physics.SetVelocity(right * horizontal * speeds.speed);
            } else if (horizontalRaw != 0 || verticalRaw != 0)
            {
                Vector3 movementVector = ((right * horizontal) + Forward() * vertical) *speeds.speed;
                physics.SetVelocity(movementVector);
            }
        }
    }
    
    Vector3 Right()
    {
        return Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * Vector3.right;
    }

    Vector3 Forward()
    {
        return Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * Vector3.forward;
    }
    
    void RotateToTarget(Transform target)
    {
        if (target != null)
        {
            // midpoint between player and target
            Vector3 midpoint = new Vector3((transform.position.x + target.transform.position.x) / 2, 0, (transform.position.z + target.position.z) / 2);
            // rotate only along y axis
            Quaternion rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, GeometryUtils.PlaneRotation(transform.position, target.position, midpoint).eulerAngles.y, transform.rotation.eulerAngles.z);

            transform.rotation = rotation;
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + Right());
    }
}
