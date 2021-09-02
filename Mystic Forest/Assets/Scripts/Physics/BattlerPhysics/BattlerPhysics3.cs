using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattlerPhysics3 : MonoBehaviour, IBattlerPhysics
{

    Vector3 jumpHorizontalVelocity;
    Vector3 oldPos;
    Vector3 velocity;
    Vector3 aerialHorizontalVelocity;
    Vector3 waitingVelocity;



    public SpriteRenderer sprite;

    float feet => sprite.bounds.min.y;
    float centerFromGround => sprite.bounds.center.y - feet;
    float freezeTime = 0;
    Ground ground;
    ISet<BattlerPhysics3> touching;
    public FloorDetection detection;

    public bool IsGrounded { get; private set; }



    public bool freeze { get => freezeTime > 0; set => throw new System.NotImplementedException(); }
    public bool lockZ { set => throw new System.NotImplementedException(); }

    public float terminalVelocity;
    public float drag;

    private void Awake()
    {
        GameObject groundGO = GameObject.FindGameObjectWithTag("Ground");
        if (groundGO == null)
        {
            throw new System.Exception("No ground present for battler physics 3 to use. Insert a game object with a 'Ground' tag to the scene.");
        }
        ground = groundGO.GetComponent<Ground>();
        touching = new HashSet<BattlerPhysics3>();
    }

    void OnTriggerEnter(Collider collider)
    {
        BattlerPhysics3 p = collider.transform.GetComponent<BattlerPhysics3>();
        if (p != null)
        {
            touching.Add(p);
        }

    }

    void OnTriggerExit(Collider collider)
    {
        BattlerPhysics3 p = collider.transform.GetComponent<BattlerPhysics3>();
        if (p != null)
        {
            touching.Remove(p);
        }
    }

    private void FixedUpdate()
    {
        if (waitingVelocity != Vector3.zero)
        {
            velocity = waitingVelocity;
            waitingVelocity = Vector3.zero;
        }

        if (!IsGrounded) velocity = velocity.y * Vector3.up + aerialHorizontalVelocity;

        ApplyGravity();
        ApplyDrag();

        velocity += OpposingForces();

        transform.position += velocity * Time.fixedDeltaTime;

        // grounded if feet hit the floor and falling
        IsGrounded = feet <= ground.FloorPosition().y;
        if (IsGrounded && velocity.y <= 0)
        {
            // kill velocity, and position so youre standing directly on top of the ground
            velocity = new Vector3(velocity.x, 0, velocity.z);
            aerialHorizontalVelocity = Vector3.zero;
            transform.position = new Vector3(transform.position.x, ground.FloorPosition().y + centerFromGround, transform.position.z);
        }
    }

    private void ApplyGravity()
    {
        
        if (!IsGrounded)
        {
            // fall
            velocity += Vector3.up * Physics.gravity.y * Time.fixedDeltaTime;
            if (velocity.y < terminalVelocity)
            {
                velocity = new Vector3(velocity.x, terminalVelocity, velocity.z);
            }
        }
    }

    private void ApplyDrag()
    {
        // only apply drag on the ground
        if (IsGrounded)
        {

            if (velocity.magnitude < 0.1f) // force complete stop if velocity is almost zero
            {
                velocity = Vector3.zero;
            } else
            {
                // the opposite direction of movement, shrunken so it equals 1
                Vector3 normalizedVelocity = velocity.normalized;
                // apply drag force to velocity
                Vector3 dragVector = new Vector3(normalizedVelocity.x, 0, normalizedVelocity.z) * -drag;
                velocity += dragVector * Time.fixedDeltaTime;
            }

        }
    }

    private Vector3 OpposingForces()
    {
        if (detection == null) return Vector3.zero;
        Collider[][] fTouching = detection.BoxCast();
        Vector3[] dirs = new Vector3[] { Vector3.forward, Vector3.right, Vector3.back, Vector3.left};

        Vector3 opposing = Vector3.zero;

        for (int i = 0; i < fTouching.Length; i ++)
        {
            for (int j = 0; j < fTouching[i].Length; j++)
            {
                Collider collider = fTouching[i][j];
                OpposingForce opposingForce = OpposingForce.ColliderToOpposingForce(collider);
                if (opposingForce != null)
                {
                    opposing += opposingForce.OppositeForce(velocity, dirs[i]);
                }
            }
        }
        return opposing;
    }

    public void SetVelocity(Vector3 velocity)
    {
        waitingVelocity = velocity;
        if (velocity.y > 0 || !IsGrounded)
        {
            aerialHorizontalVelocity = Vector3.Scale(new Vector3(1, 0, 1), velocity);
        }
    }

    public Vector3 OnOutwardVelocityApplied(Transform origin, Vector3 other)
    {
        return -new Vector3(other.x, 0, other.z);
    }

    public void ApplyForce(IForce force, Transform origin)
    {
        throw new System.NotImplementedException();
    }
}
