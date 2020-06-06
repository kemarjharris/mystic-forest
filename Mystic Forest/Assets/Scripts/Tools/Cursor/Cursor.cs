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
        newOffset += new Vector3(0, 0, 1);
        if (velocity.z < 0)
        {
            velocity = new Vector3(velocity.x, 0, 0);
        }
    }

    public void Down()
    {
        newOffset += new Vector3(0, 0, -1);
        if (velocity.z > 0)
        {
            velocity = new Vector3(velocity.x, 0, 0);
        }
    }

    public void Right()
    {
        newOffset += new Vector3(1, 0, 0);
        if (velocity.x < 0)
        {
            velocity = new Vector3(0, 0, velocity.z);
        }
    }

    public void Left()
    {
        newOffset += new Vector3(-1, 0, 0);
        if (velocity.x > 0)
        {
            velocity = new Vector3(0, 0, velocity.z);
        }
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = transform.position + (newOffset * speed);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        newOffset = Vector3.zero;
    }

    public void CheckCollision(Action<Collider> onCollide)
    {
        throw new NotImplementedException();
    }
}
