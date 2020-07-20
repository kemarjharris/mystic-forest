using UnityEngine;
using System.Collections;

public interface IBattlerPhysics 
{

    void SetVelocity(Vector3 velocity);
    void ApplyForce(IForce force, Transform origin);
    Transform transform { get; }
    bool IsGrounded { get; }
    bool freeze { get; set; }
    bool lockZ { set; }

}
