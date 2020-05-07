using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public interface ICustomizableEnumerator<T> : IEnumerator<T>
{
    void SetOnMoveNext(System.Action onMoveNext);
}