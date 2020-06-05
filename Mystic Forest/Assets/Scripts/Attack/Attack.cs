using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Attack : IAttack
{
    public Vector3 force = Vector3.zero;
    public float freezeTime = 0.1f;
    public bool hasKnockBack = true;

    Vector3 IAttack.force => force;
    float IAttack.freezeTime => freezeTime;
    bool IAttack.hasKnockBack => hasKnockBack;
}