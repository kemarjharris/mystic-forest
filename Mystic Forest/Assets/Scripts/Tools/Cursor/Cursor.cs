using UnityEngine;
using System.Collections;

public class Cursor : MonoBehaviour
{
    public float smoothTime = 0.3f;
    public float speed = 5;
    private Vector3 velocity = new Vector3();

    Vector3 targetPosition = new Vector3(10, 0, 0);
    // Update is called once per frame
    void Update()
    {

        Vector3 newOffset = Vector3.zero;
        if (Input.GetKey("w"))
        {
            newOffset += Vector3.up;
        } else if (Input.GetKey("s"))
        {
            newOffset += Vector3.down;
        }

        if (Input.GetKey("d"))
        {
            newOffset += Vector3.right;
        }
        else if (Input.GetKey("a"))
        {
            newOffset += Vector3.left;
        }

        Debug.Log(velocity);
        Vector3 targetPosition = transform.position + (newOffset * speed);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        /*
        if (newOffset != Vector3.zero)
        {
            Vector3 newPos = transform.position + newOffset;
            Vector3 screenPoint = Camera.main.WorldToViewportPoint(newPos);
            if (IsOnCamera(screenPoint))
            {
                
            }
        }
        */
    }

    /*
    bool IsOnCamera(Vector3 screenPoint)
    {
        return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
    }
    */
}
