using UnityEngine;
using System.Collections;

public class MeshPhysicsZ : MonoBehaviour
{
    static Collider ground;
    // only use gravity if airborne
    public bool IsGrounded => !rb.useGravity;
    private Vector3 velocity = Vector3.zero;
    public float smoothness = 0.3f;
    [Range(0, 1)] public float dragFactor = 0.3f;
    [Range(0, 1)] public float airForcePercentage = 0.3f;
    public float speed = 10;
    public float jumpForce = 10;
    Rigidbody rb;

    private void Awake()
    {
        if (ground == null)
        {
            ground = Instantiate(Resources.Load<GameObject>("Prefabs/Miscellaneous/Ground")).GetComponent<Collider>();
        }
        //transform.position = new VectorZ(transform.position.x, transform.position.y);
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        //rb.useGravity = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider == ground) rb.useGravity = false;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider == ground) rb.useGravity = true;
    }

    public void Move(float horizontal, float vertical)
    {
        rb.velocity = Vector3.SmoothDamp(rb.velocity, new VectorZ(horizontal * speed, vertical * speed), ref velocity, smoothness);
    }

    public void SetVelocity(VectorZ groundVelocity, float verticalVelocity)
    {
        rb.velocity = Vector3.zero;
        rb.velocity = groundVelocity + (verticalVelocity * Vector3.up);
    }

    public void AddForce(VectorZ groundForce, float verticalForce)
    {
        AddVerticalForce(verticalForce);
        AddGroundForce(groundForce);
    }

    private void AddGroundForce(VectorZ force)
    {
        if (!IsGrounded) force *= airForcePercentage;
        rb.AddForce(force, ForceMode.VelocityChange);

    }

    private void AddVerticalForce(float force)
    {
        if (!IsGrounded)
        {
            rb.AddForce(Vector3.up * force, ForceMode.VelocityChange);
        }
        else if (force > 0) // and grounded
        {
            //rb.position = new Vector3(rb.position.x, rb.position.z + 0.01f, rb.position.z);
            rb.AddForce(Vector3.up * force, ForceMode.VelocityChange);
            rb.useGravity = true;
        }

    }




    public void FixedUpdate()
    {
        if (IsGrounded) // apply drag
        {
            rb.velocity *= 1 - dragFactor;
        }
    }

    
}
