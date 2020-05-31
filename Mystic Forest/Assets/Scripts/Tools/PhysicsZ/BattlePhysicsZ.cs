using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattlePhysicsZ : MonoBehaviour
{
    static Collider ground;
    // only use gravity if airborne
    //public bool IsGrounded => !rb.useGravity;
    public bool IsGrounded { private set; get; }
    [Range(0, 1)] public float dragFactor = 0.3f;
    [Range(0, 1)] public float airForcePercentage = 0.3f;
    Rigidbody rb;
    public new BoxCollider collider;

    public bool freeze { set {
            if (value)
            {
                frozenVelocity = rb.velocity;
                rb.isKinematic = true;
            } else
            {
                rb.isKinematic = false;
                if (frozenVelocity != Vector3.zero)
                {
                    rb.velocity = frozenVelocity;
                }
            }
        }
    }
    private Vector3 frozenVelocity;


    public bool lockZ {
        set {
            if (value)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
            }
            else
            {
                rb.constraints = RigidbodyConstraints.FreezeRotation;
            }

        } }

    private void Awake()
    {
        if (ground == null)
        {
            ground = Instantiate(Resources.Load<GameObject>("Prefabs/Miscellaneous/Ground")).GetComponent<Collider>();
            ground.GetComponent<MeshRenderer>().enabled = false;
        }
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
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
        IsGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider == ground)
        {

            IsGrounded = false;
            //rb.useGravity = true;
        }
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
            // fall
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + (Physics.gravity.y * Time.fixedDeltaTime), rb.velocity.z);
            
            // if approaching ground turn on collsion
            bool collided = CheckUnderCollider(out RaycastHit hitInfo);
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
                if (virtualCollision && virtualHitInfo.collider.gameObject.tag == "Battler")
                {
                    PushAwayCollider(virtualHitInfo.collider);
                }
                
            }
        }
    }

    public bool CloseToGround => CheckUnderCollider(out RaycastHit hit);

    private void OnDrawGizmos()
    {
        Vector3 scaled = Vector3.Scale(collider.size, gameObject.transform.localScale);
        Gizmos.DrawRay(new Ray(transform.position + collider.center, Vector3.down));
        BoxColliderDrawer.DrawBoxCollider(transform, collider.enabled? Color.magenta: Color.yellow, collider.center, collider.size);
        BoxCastVisualizer.DrawBoxCastBox(transform.position + collider.center + Vector3.down * 0.5f, new Vector3(scaled.x, 1, scaled.z) / 2, Quaternion.identity, Vector3.down, 0, Color.cyan);
    }

    private bool CheckUnderCollider(out RaycastHit hit)
    {
        Vector3 scaled = Vector3.Scale(collider.size, gameObject.transform.localScale);
        bool collided = Physics.BoxCast(transform.position + collider.center * 0.5f, new Vector3(scaled.x, 1, scaled.z) / 2, Vector3.down, out RaycastHit hitInfo, Quaternion.identity, 0.5f);
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
