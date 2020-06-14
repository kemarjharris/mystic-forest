using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattlerPhysics : MonoBehaviour, IBattlerPhysics
{
    static Ground ground;
    // only use gravity if airborne
    //public bool IsGrounded => !rb.useGravity;
    public bool IsGrounded { private set; get; }
    [Range(0, 1)] public float dragFactor = 0.3f;
    [Range(0, 1)] public float airForcePercentage = 0.3f;
    Rigidbody rb;
    public new BoxCollider collider;


    public bool freeze {
        get => rb.isKinematic;
        set {
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
            GameObject groundGO = GameObject.FindGameObjectWithTag("Ground");
            // ground was not found
            if (groundGO == null)
            {
                groundGO = Instantiate(Resources.Load<GameObject>("Prefabs/Miscellaneous/2.5D Ground"));
            }
            
            ground = groundGO.GetComponent<Ground>();
        }
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;
        collider = GetComponent<BoxCollider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Case for landing on the ground
        if (collision.collider == ground.collider)
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
        if (collision.collider == ground.collider)
        {

            IsGrounded = false;
            //rb.useGravity = true;
        }
    }

    public void SetVelocity(Vector3 velocity)
    {
        float verticalVelocity = velocity.y;
        Vector3 groundVelocity = new Vector3(velocity.x, 0, velocity.z);
        //rb.velocity = Vector3.zero;
        if (verticalVelocity > 0 && IsGrounded) transform.position += Vector3.up * 0.1f;
        rb.velocity = groundVelocity + Vector3.up * (verticalVelocity < 0 && IsGrounded ? 0 : verticalVelocity);
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
                //if (rb.velocity.y < 0)
                //{
                  //  PushAwayCollider(hitInfo.collider);
                //}
            }
            if (hitInfo.collider == ground.collider)
            {
                collider.enabled = true;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 scaled = Vector3.Scale(collider.size, gameObject.transform.localScale);
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
