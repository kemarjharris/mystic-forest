using UnityEngine;
using UnityEditor;

public abstract class ExecutionEvent : ScriptableObject
{
    public abstract void OnExecute(IBattler attacker, ITargetSet targets);
}