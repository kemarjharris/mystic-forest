using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsZ : MonoBehaviour, IPhysics
{
    
    public bool IsGrounded { get => transform.position.y <= transform.position.z && rb.position.y <= rb.position.z; }
    private Vector3 movementVelocity = Vector3.zero;
    public float smoothness = 0.3f;
    [Range(0, 1)] public float dragFactor = 0.3f;
    [Range(0, 1)] public float airForcePercentage = 0.3f;
    public float speed = 10;
    public float jumpForce = 10;
    Rigidbody rb;
    Vector3 velocity;
    Vector3 resumeVelocity;

    private void Awake()
    {
        transform.position = new VectorZ(transform.position.x, transform.position.y);
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;
    }

    public void SetVelocity(VectorZ groundVelocity, float verticalVelocity)
    {
        rb.velocity = Vector3.zero;
        rb.velocity = groundVelocity + Vector3.up * (verticalVelocity < 0 && IsGrounded ? 0 : verticalVelocity );
    }

    public void FixedUpdate()
    {
        if (IsGrounded) // apply drag
        {
            if (rb.useGravity) rb.useGravity = false;
            rb.velocity *= 1 - dragFactor;
        }
        else // if is not grounded and using gravity
        {
            // if airborne && falling
            if (rb.useGravity)
            {
                // if going to hit the ground
                if (rb.position.y + (rb.velocity.y * Time.fixedDeltaTime) <= rb.position.z)
                {
                    // air borne and hitting the ground next frame
                    rb.useGravity = false;
                    rb.position = new VectorZ(rb.position.x, rb.position.z);
                    rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                }
            } else //  start falling
            {
                Debug.Log(transform.position.y + " " + transform.position.z);
                rb.useGravity = true;
            }
        }
        
    }

    private void OnEnable()
    {
        rb.constraints = RigidbodyConstraints.None;
        rb.velocity = resumeVelocity;
    }

    private void OnDisable()
    {
        resumeVelocity = rb.velocity;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        
    }
}
