using UnityEngine;
using UnityEditor;

[CreateAssetMenu()]
public class BattlerSpeed : ScriptableObject
{
    public float jumpForce = 8;
    public float jumpHorizontalForce;
    public float speed = 6;
}