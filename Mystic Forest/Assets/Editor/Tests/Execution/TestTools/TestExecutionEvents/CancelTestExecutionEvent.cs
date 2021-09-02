using UnityEngine;
using UnityEditor;

public class CancelTestExecutionEvent : ExecutionEvent
{
    public override void OnExecute(IBattler attacker)
    {
        onCancellableEvent?.Invoke();
    }
}