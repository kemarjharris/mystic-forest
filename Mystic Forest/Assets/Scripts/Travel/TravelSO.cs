using UnityEngine;
using UnityEditor;
using System.Collections;

public abstract class TravelSO : ScriptableObject
{
    public abstract IEnumerator Travel(Transform toMove, Transform dest, float speed);
}