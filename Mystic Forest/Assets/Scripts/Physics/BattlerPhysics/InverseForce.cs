using UnityEngine;
using System.Collections;

public class InverseForce : OpposingForce
{
    public override Vector3 OppositeForce(Vector3 force, Vector3 incomingDirection)
    {

        Vector3 opposing = Vector3.zero;

        if (incomingDirection == Vector3.right && force.x > 0 || incomingDirection == Vector3.left && force.x < 0)
        {
            opposing += new Vector3(-force.x, 0, 0);
        }
        if (incomingDirection == Vector3.forward && force.z > 0 || incomingDirection == Vector3.back && force.z < 0)
        {
            opposing += new Vector3(0, 0, -force.z);
        }
        return opposing;
    }
}
