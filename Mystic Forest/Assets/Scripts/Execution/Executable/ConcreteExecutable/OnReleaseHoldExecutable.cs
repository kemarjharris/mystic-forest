using UnityEngine;
using UnityEditor;

public class OnReleaseHoldExecutable : Executable
{
    public HoldInstruction instruction = HoldInstruction.instance;
    public ExecutionEvent keyDownExecutionEvent;
    public ExecutionEvent releaseExecutionEvent;
    public float releaseTime;
    public IUnityTimeService service;
    float timeStarted;
    public System.Action onStartHolding;
    public System.Action onRelease;

    public override void OnInput(string input, IBattler battler, ITargetSet targets)
    {
        if (!CorrectButton(input)) return;
        // Dont start counting until first key down
        float timePassed = state.triggered ? service.unscaledTime - timeStarted : 0;
        InstructionKeyEvent key = instruction.lookAtTime(input, timePassed, releaseTime);

        if (key == InstructionKeyEvent.KEYDOWN)
        {
            timeStarted = service.unscaledTime;
            onStartHolding?.Invoke();
            keyDownExecutionEvent.OnExecute(battler, targets);
            state.triggered = true;
        }
        else if (IsTriggered() && key == InstructionKeyEvent.KEYUP)
        {
            OnRelease(battler, targets);
            state.fired = true;
        }
        if (key == InstructionKeyEvent.BADKEY)
        {
            onRelease?.Invoke();
            state.finished = true;
        }
    }

    void OnRelease(IBattler battler, ITargetSet targets)
    {
        onRelease?.Invoke();
        releaseExecutionEvent.OnExecute(battler, targets);
    }

    public override void OnStart()
    {

        if (service == null) service = new UnityTimeService();
        AttributeCheck();
        state = new ExecutableState();
        instruction.reset();
        timeStarted = service.unscaledTime;
        
        releaseExecutionEvent.setOnCancellableEvent(delegate () { state.cancellable = true; });
        releaseExecutionEvent.setOnFinishEvent(delegate () { state.finished = true; });
    }

    private void AttributeCheck()
    {
        if (keyDownExecutionEvent == null) throw new System.ArgumentException("Key Down Execution Event is null");
        if (releaseExecutionEvent == null) throw new System.ArgumentException("Release Execution Event is null");
        if (releaseTime <= 0) throw new System.ArgumentException("Release Time must be positive");
    }

    
    public ExecutionEvent GetKeyDownExecutionEvent() => keyDownExecutionEvent;
    public ExecutionEvent GetReleaseExecutionEvent() => releaseExecutionEvent;

}