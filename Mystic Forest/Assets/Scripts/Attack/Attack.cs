using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Attack : IAttack
{
    public Force force;
    public float freezeTime = 0.1f;
    public bool hasKnockBack = true;
    public float hitStun = 0;

    public Transform origin { get; set; }

    float IAttack.hitStun => hitStun;
    IForce IAttack.force => force;
    float IAttack.freezeTime => freezeTime;
    bool IAttack.hasKnockBack => hasKnockBack;
}