using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Executable/ExecutableSO/Key Down Mash Executable")]
public class KeyDownMashExecutableSO : ExecutableSO
{
    public ExecutionEvent executionEvent;
    public ExecutionEvent mashTimeEndedEvent;
    public float mashDuration;
    public MashInstruction instruction = MashInstruction.instance;
    public IUnityTimeService service = new UnityTimeService();
    public System.Action onKeyDown;
    private float firstKeyDownTime;
    private ExecutionEvent executionEventInstance;
    private ExecutionEvent mashTimeEndedEventInstance;

    // public ChainExecutionButton button;
    // private ExpandingButtonMashVisual visualPrefab;
    // private ExpandingButtonMashVisual visual;

    public void Construct(MashInstruction instruction, ExecutionEvent executionEvent, ExecutionEvent mashTimeEndedEvent, float mashDuration)
    {
        this.instruction = instruction; 
        this.executionEvent = executionEvent;
        this.mashTimeEndedEvent = mashTimeEndedEvent;
        this.mashDuration = mashDuration;
    }

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
        executionEventInstance = Instantiate(executionEvent);
        mashTimeEndedEventInstance = Instantiate(mashTimeEndedEvent);
        mashTimeEndedEventInstance.setOnCancellableEvent(delegate { state.cancellable = true; });
        mashTimeEndedEventInstance.setOnFinishEvent(delegate { state.finished = true; });
        
        // visual.StartTimer(mashDuration + beforeCancelInputLeeway);
    }

    // public override ChainExecutionButton getButton() => button;

    public override void OnInput(string input, IBattler battler, ITargetSet targets)
    {
        if (!CorrectButton(input)) return;
        // Only start timer after first key down
        float pressTime = state.triggered ? service.unscaledTime - firstKeyDownTime : 0;
        if (pressTime <= mashDuration)
        {
            InstructionKeyEvent key = instruction.lookAtTime(input, pressTime, mashDuration);

            if (key == InstructionKeyEvent.KEYDOWN)
            {
               
                if (!state.triggered)
                {
                    firstKeyDownTime = service.unscaledTime;
                    state.triggered = true;
                }
                // visual.ExpandButton();
                onKeyDown?.Invoke();
                executionEventInstance.OnExecute(battler, targets);
            }
        } else
        {
            if (state.triggered)
            {
                mashTimeEndedEventInstance.OnExecute(battler, targets);
                state.fired = true;
            } else
            {
                state.cancellable = false;
                state.finished = true;
            }   
        }
    }

    /* For Testing */
    public ExecutionEvent GetExecutionEvent() => executionEventInstance;
    public ExecutionEvent GetMashEndedExecutionEvent() => mashTimeEndedEventInstance;


}