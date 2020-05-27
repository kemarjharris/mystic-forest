using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Attack : IAttack
{


    public VectorZ force = VectorZ.zero;

    public float verticalForce = 0;

    VectorZ IAttack.force => force;
    float IAttack.verticalForce => verticalForce;
}