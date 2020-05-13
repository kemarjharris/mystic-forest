using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "ExecutableSO/On Release Hold Executable")]
public class OnReleaseHoldExecutableSO : ExecutableSO
{
    public HoldInstruction instruction = HoldInstruction.instance;
    public ExecutionEvent keyDownExecutionEvent;
    public ExecutionEvent releaseExecutionEvent;
    public float releaseTime;
    public IUnityTimeService service;
    float timeStarted;
    public System.Action onStartHolding;
    public System.Action onRelease;
    private ExecutionEvent keyDownExecutionEventInstance;
    private ExecutionEvent releaseExecutionEventInstance;

    public void Construct(HoldInstruction instruction, ExecutionEvent keyDownExecutionEvent, ExecutionEvent releaseExecutionEvent, float releaseTime)
    {
        this.instruction = instruction;
        this.keyDownExecutionEvent = keyDownExecutionEvent;
        this.releaseExecutionEvent = releaseExecutionEvent;
        this.releaseTime = releaseTime;
    }

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
            keyDownExecutionEventInstance.OnExecute(battler, targets);
            state.triggered = true;
        }
        else if (IsTriggered() && key == InstructionKeyEvent.KEYUP)
        {
            OnRelease(battler, targets);
        }
        if (key == InstructionKeyEvent.BADKEY)
        {
            OnRelease(battler, targets);
            state.finished = true;
        }
    }

    void OnRelease(IBattler battler, ITargetSet targets)
    {
        onRelease?.Invoke();
        releaseExecutionEventInstance.OnExecute(battler, targets);
    }

    public override void OnStart()
    {
      
        if (service == null) service = new UnityTimeService();
        AttributeCheck();
        state = new ExecutableState();
        instruction.reset();
        timeStarted = service.unscaledTime;
        keyDownExecutionEventInstance = Instantiate(keyDownExecutionEvent);
        releaseExecutionEventInstance = Instantiate(releaseExecutionEvent);
        releaseExecutionEventInstance.setOnCancellableEvent(delegate() { state.cancellable = true; });
        releaseExecutionEventInstance.setOnFinishEvent(delegate () { state.finished = true; });
    }

    private void AttributeCheck()
    {
        if (keyDownExecutionEvent == null) throw new System.ArgumentException("Key Down Execution Event is null");
        if (releaseExecutionEvent == null) throw new System.ArgumentException("Release Execution Event is null");
        if (releaseTime <= 0) throw new System.ArgumentException("Release Time must be positive");
    }

    /* For testing */
    public ExecutionEvent GetKeyDownExecutionEvent() => keyDownExecutionEventInstance;
    public ExecutionEvent GetReleaseExecutionEvent() => releaseExecutionEventInstance;

}