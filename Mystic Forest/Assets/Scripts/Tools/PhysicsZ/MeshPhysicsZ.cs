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

        // Case for landing on a battler
        else if (!IsGrounded && collision.gameObject.tag == "Battler")
        {
            float temp = transform.position.x;
            //transform.position = new Vector3(collision.transform.position.x, transform.position.y, transform.position.z);
            //collision.transform.position = new Vector3(temp, collision.transform.position.y, collision.transform.position.z);
            //HandleLandOnBattlerCollision(collision);
        }
    }


    private void HandleGroundCollision()
    {
        rb.useGravity = false;
    }

    private void HandleLandOnBattlerCollision(Collision collision)
    {
        float contactCount = collision.contactCount;
        if (contactCount != 4)
        {
            return;
        }
        // Get first cotnact point to store point
        float xDistance = 0.2f;
        // Battlers have box colliders, so collision point will be a rectangle.

        // assuming unity doesnt gives me diagonal points


        Vector3 pointA = collision.GetContact(0).point;
        Vector3 pointB = collision.GetContact(1).point;
        int i = 2;
        while (i < contactCount && pointA.x == pointB.x)
        {
            pointB = collision.GetContact(i).point;
            i += 1;
        }
        

        if (pointA.y != pointB.y)
        {
            Debug.LogWarning("Running code to push two battlers apart when landing, but given points do not form a rectangle");
            Debug.Log(pointA + " " + pointB);
            return;
            
        }

        xDistance += Mathf.Abs(pointB.x - pointA.x);

        Vector3 seperation = new Vector3(xDistance, 0, 0);
        Debug.Log(pointA + " " + pointB);
        Debug.Log(seperation);
        // Push objects apart depending on which object is on the left and the right of the collision
        if (gameObject.transform.position.x <= collision.transform.position.x)
        {
            collision.transform.position += seperation;
        } else
        {
            collision.transform.position -= seperation;
        }
    }

    private void ThrowIncorrectContactException(Collision collision)
    {
        IList<Vector3> list = new List<Vector3>();
        for (int i = 0; i < collision.contactCount; i++)
        {
            list.Add(collision.GetContact(i).point);
        }
        throw new System.Exception(
            "Collision with battler occured on a battler's head, but the resulting collision was not a rectangle. The collision points in question: "
            + CollectionUtils.Print<Vector3>(list)
            );
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
            //collider.enabled = false;
            //transform.position = new Vector3(collision.transform.position.x, transform.position.y, transform.position.z);
            //collision.transform.position = new Vector3(temp, collision.transform.position.y, collision.transform.position.z);
            //HandleLandOnBattlerCollision(collision);
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
        } else
        {
            bool collided = CheckUnderBattler(out RaycastHit hitInfo);
            if (collided && rb.velocity.y < 0 && hitInfo.collider.gameObject.tag == "Battler" )
            {
                Debug.Log("landed on battler");
                //hitInfo.collider.transform.position += new Vector3(-rb.velocity.x/ 2, 0, 0);
                //hitInfo.collider.enabled = false;
                collider.enabled = false;
            } else if (hitInfo.collider == ground)
            {
                collider.enabled = true;
            }
            
        }
    }

    private void OnDrawGizmos()
    {
        BoxColliderDrawer.DrawBoxCollider(transform, Color.magenta, collider.center, collider.size);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 scaled = Vector3.Scale(collider.size, gameObject.transform.localScale);
        BoxCastVisualizer.DrawBoxCastBox(transform.position + collider.center + Vector3.down * 0.5f, new Vector3(scaled.x + 0.2f, 1, scaled.z) / 2, transform.rotation, Vector3.down, 0, Color.cyan);
    }

    private bool CheckUnderBattler(out RaycastHit hit)
    {
        Vector3 scaled = Vector3.Scale(collider.size, gameObject.transform.localScale);
        bool collided = Physics.BoxCast(transform.position + collider.center * 0.5f, new Vector3(scaled.x + 0.2f, 1, scaled.z) / 2, Vector3.down, out RaycastHit hitInfo, gameObject.transform.rotation, 0.5f);
        hit = hitInfo;
        return collided;
    }


}
