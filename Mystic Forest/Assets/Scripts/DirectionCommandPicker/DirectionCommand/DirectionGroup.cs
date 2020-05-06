using UnityEngine;
using UnityEditor;

[CreateAssetMenu()]
public class DirectionGroup : ScriptableObject
{
    public Direction[] directions = new Direction[0];
}