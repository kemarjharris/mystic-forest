using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecutableChainEnumerator : IEnumerator<IExecutable>
{

    IEnumerator<IExecutable> enumerator;
    System.Action onMoveNext;

    public ExecutableChainEnumerator(IEnumerator<IExecutable> enumerator)
    {
        this.enumerator = enumerator;
    }

    public IExecutable Current => enumerator.Current;

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
