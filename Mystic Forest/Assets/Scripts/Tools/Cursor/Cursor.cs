using UnityEngine;
using System.Collections;
using System;

public class Cursor : MonoBehaviour, ICursor
{
    public float smoothTime = 0.3f;
    public float speed = 5;
    private Vector3 velocity = new Vector3();

    Vector3 targetPosition;
    Vector3 newOffset = Vector3.zero;

    public void Up()
    {
        newOffset += Vector3.up;
        if (velocity.y < 0)
        {
            velocity = new Vector3(velocity.x, 0, velocity.z);
        }
    }

    public void Down()
    {
        newOffset += Vector3.down;
        if (velocity.y > 0)
        {
            velocity = new Vector3(velocity.x, 0, velocity.z);
        }
    }

    public void Right()
    {
        newOffset += Vector3.right;
        if (velocity.x < 0)
        {
            velocity = new Vector3(0, velocity.y, velocity.z);
        }
    }

    public void Left()
    {
        newOffset += Vector3.left;
        if (velocity.x > 0)
        {
            velocity = new Vector3(0, velocity.y, velocity.z);
        }
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = transform.position + (newOffset * speed);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        newOffset = Vector3.zero;
    }

    public void CheckCollision(Action<Collider2D> onCollide)
    {
        throw new NotImplementedException();
    }
}
