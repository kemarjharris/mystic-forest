using UnityEngine;
using UnityEditor;

public interface IAttack
{
    IForce force { get; }
    float freezeTime { get; }
    float hitStun { get; }
    bool hasKnockBack { get; }
    Transform origin { get; set; }
}