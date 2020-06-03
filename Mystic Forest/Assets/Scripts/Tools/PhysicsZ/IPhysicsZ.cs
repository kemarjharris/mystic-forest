using UnityEngine;
using System.Collections;

public interface IPhysicsZ 
{
    void SetVelocity(VectorZ groundVelocity, float verticalVelocity);

    bool IsGrounded { get; }
    Transform transform { get; }
}
