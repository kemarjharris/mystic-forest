﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattlerPhysics2 : MonoBehaviour, IBattlerPhysics
{
    static Ground ground;
    // only use gravity if airborne
    //public bool IsGrounded => !rb.useGravity;
    public bool IsGrounded { private set; get; }
    [Range(0, 1)] public float dragFactor = 0.3f;
    [Range(0, 1)] public float airForcePercentage = 0.3f;
    public float terminalVelocity = -20;
    Rigidbody rb;
    public new BoxCollider collider;
    Vector3 jumpHorizontalVelocity;
    IRoutine forceRoutine;

    private bool colliderOn
    {
        get
        {
            // return collider.enabled;
            return !collider.isTrigger;
        }

        set
        {
            //collider.enabled = value;
            collider.isTrigger = !value;
        }
    }

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


    private void OnColliderEnter(Collider other)
    {
        // Case for landing on the ground
        if (other == ground.collider)
        {
            HandleGroundCollision();
        }
    }

    private void OnColliderExit(Collider other)
    {
        if (other == ground.collider)
        {

            IsGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision) => OnColliderEnter(collision.collider);
    private void OnCollisionExit(Collision collision) => OnColliderExit(collision.collider);
    private void OnTriggerEnter(Collider other) => OnColliderEnter(other);

    /*
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Battler" && !colliderOn && PointInOABB(transform.position, (BoxCollider)other))
        {
            PushAwayCollider(other);
        }
    }
    */
    private void HandleGroundCollision()
    {
        IsGrounded = true;
    }

    public void SetVelocity(Vector3 velocity)
    {
        float verticalVelocity = velocity.y;
        Vector3 groundVelocity = new Vector3(velocity.x, 0, velocity.z);
        jumpHorizontalVelocity = groundVelocity;
        //rb.velocity = Vector3.zero;
        if (verticalVelocity > 0 && IsGrounded) transform.position += Vector3.up * 0.1f;
        rb.velocity = groundVelocity + Vector3.up * (verticalVelocity < 0 && IsGrounded ? 0 : verticalVelocity);
    }

    public void ApplyForce(IForce force, Transform origin)
    {
        if (forceRoutine != null && forceRoutine.IsRunning())
        {
            forceRoutine.Stop();
        }
        forceRoutine = new RoutineImpl(force.ApplyForce(this, origin), this);
        forceRoutine.OnRoutineFinished = () => forceRoutine = null;
        forceRoutine.Start();
    }


    public void FixedUpdate()
    {
        if (IsGrounded) // apply drag
        {
            if (forceRoutine == null)
            {   
                rb.velocity *= 1 - dragFactor;
            }
        }
        else
        {
            // fall
            rb.velocity = new Vector3(jumpHorizontalVelocity.x, Mathf.Max(rb.velocity.y + (Physics.gravity.y * Time.fixedDeltaTime), terminalVelocity), jumpHorizontalVelocity.z);
            /*
            if (!colliderOn) // something currently underneath battler
            {
                RaycastHit[] hits = CheckUnderColliderAll();
                if (hits.Length > 0)
                {
                    int battlerCollider = -1;
                    int groundCollider = -1;
                    int wallCollider = -1;
                    int edgeWallCollider = -1;
                    for (int i = 0; i < hits.Length; i ++)
                    {
                        if (hits[i].collider.tag == "Battler" && hits[i].collider != collider) battlerCollider = i;
                        else if (hits[i].collider.tag == "Ground") groundCollider = i;
                        else if (hits[i].collider.tag == "Wall") wallCollider = i;
                        else if (hits[i].collider.tag == "Edge Wall") edgeWallCollider = i;
                    }

                    if (groundCollider >= 0) // collided w ground 
                    {
                        colliderOn = true;
                    } else if (wallCollider >= 0 || edgeWallCollider >= 0) // colliding with wall
                    {
                        if (edgeWallCollider >= 0 && battlerCollider >= 0) 
                        {

                            // if battler is touching wall and ground
                            RaycastHit[] battlerHits = CheckCollider((BoxCollider) hits[battlerCollider].collider);


                            float offset = 0.1f;
                            if (hits[edgeWallCollider].collider.transform.position.x > transform.position.x) // wall in front of battler
                            {
                                offset *= -1;
                            }

                            // move slighty to the side of battler to trigger push away
                            transform.position =
                                new Vector3(hits[battlerCollider].transform.position.x + offset, transform.position.y, transform.position.z);

                            // push yourself away from the wall and battler
                            PushSelfFromCollider(hits[battlerCollider].collider);
                            jumpHorizontalVelocity = Vector3.zero;

                        } else if (wallCollider >= 0 && battlerCollider >= 0)
                        {
                            PushAwayCollider(hits[battlerCollider].collider);
                        } else
                        {
                            colliderOn = true;
                        }
                    }
                } else //  if nothing beneath battler enable collider
                {
                    colliderOn = true;
                }
            } else // nothing underneath battler
            {
                bool collided = CheckUnderCollider(out RaycastHit hitInfo);
                // if approaching battler turn off collision
                if (collided && hitInfo.collider.gameObject.tag == "Battler" && rb.velocity.y < 0)
                {
                    colliderOn = false;
                    if (hitInfo.collider.attachedRigidbody.velocity.y > 0)
                    {    // other battler is rising
                        PushAwayCollider(hitInfo.collider);
                    }
                }
            }
            */
        }
    }

    /*
    private void OnDrawGizmosSelected()
    {
        Vector3 scaled = Vector3.Scale(collider.size, gameObject.transform.localScale);
        BoxColliderDrawer.DrawBoxCollider(transform, colliderOn ? Color.magenta: Color.yellow, collider.center, collider.size, transform.rotation);
        BoxCastVisualizer.DrawBoxCastBox(transform.position + collider.center + Vector3.down * 0.5f, new Vector3(scaled.x, 1, scaled.z) / 2, Quaternion.identity, Vector3.down, 0, Color.cyan);
    }

    private bool CheckUnderCollider(out RaycastHit hit)
    {
        Vector3 scaled = Vector3.Scale(collider.size, gameObject.transform.localScale);
        bool collided = Physics.BoxCast(transform.position + collider.center * 0.5f, new Vector3(scaled.x, 1, scaled.z) / 2, Vector3.down, out RaycastHit hitInfo, Quaternion.identity, 0.5f);
        hit = hitInfo;
        return collided;
    }

    private RaycastHit[] CheckCollider(BoxCollider collider)
    {
        Vector3 scaled = Vector3.Scale(collider.size, gameObject.transform.localScale);
        return Physics.BoxCastAll(transform.position + collider.center * 0.5f, new Vector3(scaled.x, 1, scaled.z) / 2, Vector3.zero, Quaternion.identity, 0.5f);
    }

    private RaycastHit[] CheckUnderColliderAll()
    {
        Vector3 scaled = Vector3.Scale(collider.size, gameObject.transform.localScale);
        return Physics.BoxCastAll(transform.position + collider.center * 0.5f, new Vector3(scaled.x, 1, scaled.z) / 2, Vector3.down, Quaternion.identity, 0.5f);
    }

    private void PushAwayCollider(Collider col, float amount = 1)
    {
        float colliderCenter = transform.position.x + collider.center.x;
        Vector3 push = new Vector3((collider.size.x * transform.localScale.x) / 2, 0, 0);

        if (colliderCenter > col.transform.position.x) push *= -1;
        col.transform.position += push * amount;
    }

    private void PushSelfFromCollider(Collider other, float amount = 1)
    {
        float colliderCenter = transform.position.x + collider.center.x;
        Vector3 push = new Vector3((collider.size.x * transform.localScale.x) / 2, 0, 0);

        if (colliderCenter < other.transform.position.x) push *= -1;
        collider.transform.position += push * amount;
    }

    private bool PointInOABB(Vector3 point, BoxCollider box)
    {
        point = box.transform.InverseTransformPoint(point) - box.center;

        // Vector3 size = Vector3.Scale(box.size, box.transform.localScale);
        float halfX = (box.size.x * 0.5f);
        float halfY = (box.size.y * 0.5f);
        float halfZ = (box.size.z * 0.5f);
        if (point.x < halfX && point.x > -halfX &&
           point.y < halfY && point.y > -halfY &&
           point.z < halfZ && point.z > -halfZ)
            return true;
        else
            return false;
    }
    */
}
