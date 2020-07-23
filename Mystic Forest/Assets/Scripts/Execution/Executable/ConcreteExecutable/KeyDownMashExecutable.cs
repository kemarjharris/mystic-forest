using UnityEngine;
using UnityEditor;

public class KeyDownMashExecutable : Executable
{

    public ExecutionEvent executionEvent;
    public ExecutionEvent mashTimeEndedEvent;
    public float mashDuration;
    public MashInstruction instruction = MashInstruction.instance;
    public IUnityTimeService service = new UnityTimeService();
    public System.Action onKeyDown;
    private float firstKeyDownTime;

    public override void OnStart()
    {
        if (!executionEvent) throw new System.ArgumentException("Keydown execution event cannot be null");
        if (!mashTimeEndedEvent) throw new System.ArgumentException("MashTimeEndedEvent cannot be null");
        if (mashDuration <= 0) throw new System.ArgumentException("Mash Duration must be positive");
        if (instruction != null)
        {
            instruction = MashInstruction.instance;
        }
        instruction.reset();
        state = new ExecutableState();
        mashTimeEndedEvent.setOnCancellableEvent(delegate { state.cancellable = true; });
        mashTimeEndedEvent.setOnFinishEvent(delegate { state.finished = true; });
    }

    public override void OnInput(string input, IBattler battler, ITargetSet targets)
    {
        // Only start timer after first key down
        float pressTime = state.triggered ? service.unscaledTime - firstKeyDownTime : 0;
        if (pressTime <= mashDuration)
        {
            if (!CorrectButton(input)) return;
            InstructionKeyEvent key = instruction.lookAtTime(input, pressTime, mashDuration);

            if (key == InstructionKeyEvent.KEYDOWN)
            {

                if (!state.triggered)
                {
                    firstKeyDownTime = service.unscaledTime;
                    battler.eventSet.onEventExecuted?.Invoke();
                    state.triggered = true;
                }
                // visual.ExpandButton();
                onKeyDown?.Invoke();
                executionEvent.OnExecute(battler, targets);
            }
        }
        else
        {
            if (state.triggered)
            {
                executionEvent.Interrupt();
                mashTimeEndedEvent.OnExecute(battler, targets);
                state.fired = true;
            }
            else
            {
                state.cancellable = false;
                state.finished = true;
            }
        }
    }

    public ExecutionEvent GetExecutionEvent() => executionEvent;
    public ExecutionEvent GetMashEndedExecutionEvent() => mashTimeEndedEvent;
}