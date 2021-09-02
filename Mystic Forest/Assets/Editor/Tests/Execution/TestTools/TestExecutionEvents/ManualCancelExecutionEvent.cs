using UnityEngine;
using UnityEditor;

public class ManualCancelExecutionEvent : ExecutionEvent
{
    public override void OnExecute(IBattler attacker) { }

    public void FireOnCancelEvent()
    {
        onCancellableEvent?.Invoke();
    }
}