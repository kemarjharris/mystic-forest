using UnityEngine;
using UnityEditor;
using System.Collections;

public abstract class TravelMethodSO : ScriptableObject
{
    public abstract IEnumerator Travel(Transform toMove, Vector3 dest, float speed);
}