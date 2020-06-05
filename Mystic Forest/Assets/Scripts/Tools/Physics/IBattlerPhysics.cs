using UnityEngine;
using System.Collections;

public interface IBattlerPhysics 
{

    void SetVelocity(Vector3 velocity);
    Transform transform { get; }
    bool IsGrounded { get; }
    bool freeze { get; set; }
    bool lockZ { set; }

}
