using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    public new BoxCollider collider;

    private void Awake()
    {
        if (ground == null)
        {
            ground = Instantiate(Resources.Load<GameObject>("Prefabs/Miscellaneous/Ground")).GetComponent<Collider>();
        }
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        collider = GetComponent<BoxCollider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Case for landing on the ground
        if (collision.collider == ground)
        {
            HandleGroundCollision();
        }

        // Case for landing in a battler
        else if (collision.gameObject.tag == "Battler" && collider.bounds.Contains(collision.transform.position)) 
        {
            Debug.Log("purr");
            float colliderCenter = transform.position.x + collider.center.x;
            Vector3 push = new Vector3(collider.size.x, 0, 0);

            if (colliderCenter > collision.transform.position.x) push *= -1;
        
            transform.position -= push;
            collision.transform.position += push;


            //rb.velocity += rb;
        }
    }


    private void HandleGroundCollision()
    {
        rb.useGravity = false;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider == ground)
        {
            rb.useGravity = true;
        }
        // Case for landing on a battler
        else if (!IsGrounded && collision.gameObject.tag == "Battler")
        {
            float temp = transform.position.x;
        }
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
        }

    }

    public void FixedUpdate()
    {
        if (IsGrounded) // apply drag
        {
            rb.velocity *= 1 - dragFactor;
        }
        else
        {
            bool collided = CheckUnderBattler(out RaycastHit hitInfo);
            if (hitInfo.collider == ground)
            {
                collider.enabled = true;
            }
            if (collided && rb.velocity.y < 0 && hitInfo.collider.gameObject.tag == "Battler" )
            {
             //   SeparateBattlers(hitInfo.collider, hitInfo.point);
                
                collider.enabled = false;
            } 
            
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 scaled = Vector3.Scale(collider.size, gameObject.transform.localScale);
        BoxColliderDrawer.DrawBoxCollider(transform, Color.magenta, collider.center, collider.size);
        BoxCastVisualizer.DrawBoxCastBox(transform.position + collider.center + Vector3.down * 0.5f, new Vector3(scaled.x, 1, scaled.z) / 2, transform.rotation, Vector3.down, 0, Color.cyan);
    }

    private bool CheckUnderBattler(out RaycastHit hit)
    {
        Vector3 scaled = Vector3.Scale(collider.size, gameObject.transform.localScale);
        bool collided = Physics.BoxCast(transform.position + collider.center * 0.5f, new Vector3(scaled.x, 1, scaled.z) / 2, Vector3.down, out RaycastHit hitInfo, gameObject.transform.rotation, 0.5f);
        hit = hitInfo;
        return collided;
    }
}
