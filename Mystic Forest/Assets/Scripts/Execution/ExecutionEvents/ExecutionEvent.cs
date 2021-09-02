using UnityEngine;
using UnityEditor;

public abstract class ExecutionEvent : ScriptableObject
{
    protected System.Action onFinishEvent;
    protected System.Action onCancellableEvent;
    public IExecutablePool pool = new ExecutablePool();

    public abstract void OnExecute(IBattler attacker);

    public void setOnCancellableEvent(System.Action onCancellableEvent)
    {
        this.onCancellableEvent = onCancellableEvent;
    }

    public void setOnFinishEvent(System.Action onFinishEvent)
    {
        this.onFinishEvent = onFinishEvent;
    }

    public virtual void Interrupt() { }

}