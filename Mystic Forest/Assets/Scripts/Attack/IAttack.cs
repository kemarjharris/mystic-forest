using UnityEngine;
using UnityEditor;

public interface IAttack
{
    VectorZ force { get; }
    float verticalForce { get; }
    float freezeTime { get; }
    bool hasKnockBack { get; }
}