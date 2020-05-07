using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

public class CustomizableEnumerator<T> : ICustomizableEnumerator<T>
{
    IEnumerator<T> enumerator;
    System.Action onMoveNext;

    public CustomizableEnumerator(IEnumerator<T> enumerator)
    {
        this.enumerator = enumerator;
    }

    public T Current => enumerator.Current;

    object IEnumerator.Current => Current;

    public void Dispose() => enumerator.Dispose();

    public void Reset() => enumerator.Reset();

    public void SetOnMoveNext(System.Action onMoveNext)
    {
        this.onMoveNext = onMoveNext;
    }

    public bool MoveNext()
    {
        onMoveNext?.Invoke();
        return enumerator.MoveNext();
    }
}