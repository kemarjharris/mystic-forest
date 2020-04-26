using UnityEngine;
using UnityEditor;

public abstract class ExecutionEvent : ScriptableObject
{
    protected System.Action onFinishEvent;
    protected System.Action onCancellableEvent;

    public abstract void OnExecute(IBattler attacker, ITargetSet targets);

    public void setOnCancellableEvent(System.Action onCancellableEvent)
    {
        this.onCancellableEvent = onCancellableEvent;
    }

    public void setOnFinishEvent(System.Action onFinishEvent)
    {
        this.onFinishEvent = onFinishEvent;
    }

}