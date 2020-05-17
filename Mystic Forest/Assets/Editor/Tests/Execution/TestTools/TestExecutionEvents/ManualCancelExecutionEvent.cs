using UnityEngine;
using UnityEditor;

public class ManualCancelExecutionEvent : ExecutionEvent
{
    public override void OnExecute(IBattler attacker, ITargetSet targets) { }

    public void FireOnCancelEvent()
    {
        onCancellableEvent?.Invoke();
    }
}