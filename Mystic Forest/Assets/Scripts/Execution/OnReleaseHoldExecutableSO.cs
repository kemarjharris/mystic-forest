using UnityEngine;
using UnityEditor;

[CreateAssetMenu()]
public class OnReleaseHoldExecutableSO : ExecutableSO
{
    // public ChainExecutionButton button;
    public HoldInstruction instruction = HoldInstruction.instance;
    public ExecutionEvent keyDownExecutionEvent;
    public ExecutionEvent releaseExecutionEvent;
    public float releaseTime;
    public IUnityTimeService service;
    float timeStarted;

    // private static HoldVisual visualPrefab;
    // private HoldVisual visual;
    // public override ChainExecutionButton getButton() => button;

    public void Construct(HoldInstruction instruction, ExecutionEvent keyDownExecutionEvent, ExecutionEvent releaseExecutionEvent, float releaseTime)
    {
        this.instruction = instruction;
        this.keyDownExecutionEvent = keyDownExecutionEvent;
        this.releaseExecutionEvent = releaseExecutionEvent;
        this.releaseTime = releaseTime;
    }

    public override void OnInput(string input, IBattler battler, ITargetSet targets)
    {
        InstructionKeyEvent key = instruction.lookAtTime(input, service.unscaledTime - timeStarted, releaseTime);
        
        if (key == InstructionKeyEvent.KEYDOWN)
        {
            timeStarted = service.unscaledTime;
            keyDownExecutionEvent.OnExecute(battler, targets);
            //visual.Fill(releaseTime);
            state.triggered = true;
        }
        else if (IsTriggered() && key == InstructionKeyEvent.KEYUP)
        {
            OnRelease(battler, targets);
        }
        if (key == InstructionKeyEvent.BADKEY)
        {
            // probably change to bad release
            OnRelease(battler, targets);
            state.finished = true;
        }
    }

    void OnRelease(IBattler battler, ITargetSet targets)
    {
        releaseExecutionEvent.OnExecute(battler, targets);
       // visual.StopFill();
    }

    public override void OnStart()
    {
      
        if (service == null) service = new UnityTimeService();
        AttributeCheck();
        state = new ExecutableState();
        instruction.reset();
        timeStarted = service.unscaledTime;
        releaseExecutionEvent.setOnCancellableEvent(delegate() { state.cancellable = true; });
        releaseExecutionEvent.setOnFinishEvent(delegate () { state.finished = true; });
        //visual.FullSize();
    }

    private void AttributeCheck()
    {
        if (keyDownExecutionEvent == null) throw new System.ArgumentException("Key Down Execution Event is null");
        if (releaseExecutionEvent == null) throw new System.ArgumentException("Release Execution Event is null");
        if (releaseTime <= 0) throw new System.ArgumentException("Release Time must be positive");
    }

    /*
    public override AttackVisual draw(Vector3 postion, Transform parent)
    {
        if (visualPrefab == null)
        {
            visualPrefab = Resources.Load<HoldVisual>("Prefabs/ExecutableHoldVisual");
        }
        visual = Instantiate(visualPrefab, postion, Quaternion.identity, parent.transform);
        return visual;
    }
    */

}