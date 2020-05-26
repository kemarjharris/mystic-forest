using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsZTestScene : MonoBehaviour
{
    public PhysicsZ body;
    public float force;
    public float yForce;

    // Update is called once per frame
    void FixedUpdate()
    {
        body.Move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (Input.GetKeyDown("j")) body.SetVelocity(new VectorZ(force, 0), 0);
        if (Input.GetKeyDown("k")) body.SetVelocity(new VectorZ(0, 0), yForce);
        if (Input.GetKeyDown("l")) body.SetVelocity(new VectorZ(force, 0), yForce);

    }
}
