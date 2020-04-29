using UnityEngine;
using UnityEditor;

[CreateAssetMenu()]
public class KeyDownMashExecutableSO : ExecutableSO
{
    public ExecutionEvent executionEvent;
    public ExecutionEvent mashTimeEndedEvent;
    public float mashDuration;
    public MashInstruction instruction = MashInstruction.instance;
    public IUnityTimeService service = new UnityTimeService();
    private float firstKeyDownTime;

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
        mashTimeEndedEvent.setOnCancellableEvent(delegate { state.cancellable = true; });
        mashTimeEndedEvent.setOnFinishEvent(delegate { state.finished = true; });
        // visual.StartTimer(mashDuration + beforeCancelInputLeeway);
    }

    /*
    public override AttackVisual draw(Vector3 postion, Transform parent)
    {

        if (visualPrefab == null)
        {
            visualPrefab = Resources.Load<ExpandingButtonMashVisual>("Prefabs/ExpandingButtonExecutableMashVisual");
        }
        visual = Instantiate(visualPrefab, postion, Quaternion.identity, parent.transform);
        return visual;
    }*/

    // public override ChainExecutionButton getButton() => button;

    public override void OnInput(string input, IBattler battler, ITargetSet targets)
    {
        float pressTime = service.unscaledTime - firstKeyDownTime;
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
                executionEvent.OnExecute(battler, targets);
            }
        } else
        {
            if (state.triggered)
            {
                mashTimeEndedEvent.OnExecute(battler, targets);
            } else
            {
                state.cancellable = false;
                state.finished = true;
            }
            
        }
        
    }

   
}