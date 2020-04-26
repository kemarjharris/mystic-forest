using UnityEngine;
using UnityEditor;

public class TestExecutionEvent : ExecutionEvent
{
    public int timesExecuted = 0;

    public override void OnExecute(IBattler attacker, ITargetSet targets)
    {
        timesExecuted++;
    }
}