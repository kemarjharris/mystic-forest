using UnityEngine;
using UnityEditor;

public class CancelTestExecutionEvent : ExecutionEvent
{
    public override void OnExecute(IBattler attacker, ITargetSet targets)
    {
        onCancellableEvent();
    }
}