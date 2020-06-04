using UnityEngine;
using System.Collections;

public interface IPhysics
{
    void SetVelocity(VectorZ groundVelocity, float verticalVelocity);

    bool IsGrounded { get; }
    Transform transform { get; }
}
