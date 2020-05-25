using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTestScene : MonoBehaviour
{
    public Movement body;

    // Update is called once per frame
    void FixedUpdate()
    {
        body.Move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (Input.GetKey("space")) body.Jump();
    }
}
