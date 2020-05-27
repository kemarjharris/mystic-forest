﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsZ : MonoBehaviour
{

    public bool IsGrounded { get => transform.position.y <= transform.position.z; }
    private Vector3 movementVelocity = Vector3.zero;
    public float smoothness =  0.3f;
    [Range(0, 1)] public float dragFactor = 0.3f;
    [Range(0, 1)] public float airForcePercentage = 0.3f;
    public float speed = 10;
    public float jumpForce = 10;
    Rigidbody rb;

    public Vector3 Velocity() => rb.velocity;

    private void Awake()
    {
        transform.position = new VectorZ(transform.position.x, transform.position.y);
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;
    }

    public void Move(float horizontal, float vertical)
    {
        rb.position = Vector3.SmoothDamp(rb.position, rb.position + new VectorZ(horizontal * speed, vertical * speed), ref movementVelocity, smoothness);
    }

    public void SetVelocity(VectorZ groundVelocity, float verticalVelocity)
    {

        rb.velocity = groundVelocity + 
            (Vector3.up * (IsGrounded && verticalVelocity < 0 ? 0 : verticalVelocity)); // no decreasing vertical velocity if grounded
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
            rb.position = new Vector3(rb.position.x, rb.position.z + 0.01f, rb.position.z);
            rb.AddForce(Vector3.up * force, ForceMode.VelocityChange);
            rb.useGravity = true;
        } 
        
    }

    public void FixedUpdate()
    {
        if (IsGrounded) // apply drag
        {
            if (rb.useGravity) rb.useGravity = false;
            rb.velocity *= 1 - dragFactor;
        }
        else // if is not grounded
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
                rb.useGravity = true;
            }
        } 
    }
}
