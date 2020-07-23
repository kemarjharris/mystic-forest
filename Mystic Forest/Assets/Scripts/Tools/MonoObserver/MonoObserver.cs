using UnityEngine;
using UnityEditor;

public abstract class MonoObserver<T> : MonoBehaviour
{

    public abstract void Observe(T observable);
}