using UnityEngine;
using UnityEditor;

public class FinishTestExecutionEvent : TestExecutionEvent
{
    public override void OnExecute(IBattler attacker, ITargetSet targets)
    {
        onFinishEvent();
    }
}