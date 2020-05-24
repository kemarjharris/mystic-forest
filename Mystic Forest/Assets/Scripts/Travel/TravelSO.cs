using UnityEngine;
using UnityEditor;
using System.Collections;

public abstract class TravelMethodSO : ScriptableObject
{
    public abstract IEnumerator Travel(Transform toMove, Transform dest, float speed);
}