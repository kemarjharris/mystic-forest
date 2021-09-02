 using UnityEngine;
using UnityEditor;

public class AutoExecutable : Executable
{

    public ExecutionEvent executionEvent;

    public override void OnInput(string input, IBattler battler)
    {
        if (!state.triggered)
        {
            executionEvent.OnExecute(battler);
            state.triggered = true;
            state.fired = true;
        }
    }

    public override void OnStart()
    {
        state = new ExecutableState();
        if (executionEvent == null)
        {
            throw new System.ArgumentException();
        }
        executionEvent.setOnCancellableEvent(delegate {
            state.cancellable = true;
        });
        executionEvent.setOnFinishEvent(delegate {
            state.finished = true;
        });
    }
}