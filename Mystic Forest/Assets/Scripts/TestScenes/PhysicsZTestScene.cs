using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsZTestScene : MonoBehaviour
{
    public IBattlerPhysics body;
    public float force;
    public float yForce;

    // Update is called once per frame
    void FixedUpdate()
    {
        body.SetVelocity(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
        if (Input.GetKeyDown("j")) body.SetVelocity(new Vector3(force, 0, 0));
        if (Input.GetKeyDown("k")) body.SetVelocity(new Vector3(0, yForce, 0));
        if (Input.GetKeyDown("l")) body.SetVelocity(new Vector3(force, yForce, force));

    }
}
