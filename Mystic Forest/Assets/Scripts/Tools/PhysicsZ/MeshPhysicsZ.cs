using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshPhysicsZ : MonoBehaviour
{
    static Collider ground;
    // only use gravity if airborne
    //public bool IsGrounded => !rb.useGravity;
    public bool IsGrounded { private set; get; }
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
        else if (collision.gameObject.tag == "Battler" && collider.bounds.Contains(collision.transform.position + ((BoxCollider) collision.collider).center)) 
        {
            PushAwayCollider(collision.collider);
        }
    }


    private void HandleGroundCollision()
    {
        rb.useGravity = false;
        IsGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider == ground)
        {

            IsGrounded = false;
            rb.useGravity = true;
        }
    }

    public void Move(float horizontal, float vertical)
    {
        rb.velocity = Vector3.SmoothDamp(rb.velocity, new VectorZ(horizontal * speed, vertical * speed), ref velocity, smoothness);
    }

    public void SetVelocity(VectorZ groundVelocity, float verticalVelocity)
    {
        //rb.velocity = Vector3.zero;
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

            // if approaching ground turn on collsion
            bool collided = CheckUnderBattler(out RaycastHit hitInfo);
            // if approaching battler turn off collision
            if (collided && hitInfo.collider.gameObject.tag == "Battler")
            {
                collider.enabled = false;
            }
            if (hitInfo.collider == ground)
            {
                collider.enabled = true;
            } 

            if (!collider.enabled) // perform our own collision check to push away battlers directly underneath us
            {
                
                bool virtualCollision = Physics.Raycast(new Ray(transform.position + collider.center, Vector3.down), out RaycastHit virtualHitInfo, collider.size.y * transform.localScale.y / 2);
                Debug.Log("checking virtual Collision did collide:" + virtualCollision);
                if (virtualCollision && virtualHitInfo.collider.gameObject.tag == "Battler")
                {
                    PushAwayCollider(virtualHitInfo.collider);
                }
                
            }
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 scaled = Vector3.Scale(collider.size, gameObject.transform.localScale);
        Gizmos.DrawRay(new Ray(transform.position + collider.center, Vector3.down));
        BoxColliderDrawer.DrawBoxCollider(transform, collider.enabled? Color.magenta: Color.yellow, collider.center, collider.size);
        BoxCastVisualizer.DrawBoxCastBox(transform.position + collider.center + Vector3.down * 0.5f, new Vector3(scaled.x, 1, scaled.z) / 2, transform.rotation, Vector3.down, 0, Color.cyan);
    }

    private bool CheckUnderBattler(out RaycastHit hit)
    {
        Vector3 scaled = Vector3.Scale(collider.size, gameObject.transform.localScale);
        bool collided = Physics.BoxCast(transform.position + collider.center * 0.5f, new Vector3(scaled.x, 1, scaled.z) / 2, Vector3.down, out RaycastHit hitInfo, gameObject.transform.rotation, 0.5f);
        hit = hitInfo;
        return collided;
    }

    private void PushAwayCollider(Collider col)
    {
        float colliderCenter = transform.position.x + collider.center.x;
        Vector3 push = new Vector3((collider.size.x * transform.localScale.x) / 2, 0, 0);

        if (colliderCenter > col.transform.position.x) push *= -1;

        transform.position -= push;
        col.transform.position += push;
    }
}
