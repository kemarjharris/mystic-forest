using UnityEngine;
using UnityEditor;

public interface IAttack
{
    Vector3 force { get; }
    float freezeTime { get; }
    bool hasKnockBack { get; }
}