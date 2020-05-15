using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Executable/ExecutableSO/Press Executable")]
public class PressExecutableSO : ExecutableSO {

    public PressInstruction instruction = null;
    public ExecutionEvent executionEvent = null;
    private ExecutionEvent executionEventInstance = null;
    
    //private static AttackVisual visualPrefab;
    //public ChainExecutionButton button;

    public void Construct(PressInstruction instruction, ExecutionEvent executionEvent)
    {
        this.instruction = instruction;
        this.executionEvent = executionEvent;
    }

    public override void OnStart()
    {
        state = new ExecutableState();
        executionEventInstance = Instantiate(executionEvent);
        if (executionEventInstance == null)
        {
            throw new ArgumentException();
        }
        executionEventInstance.setOnCancellableEvent(delegate {
            state.cancellable = true;
        });
        executionEventInstance.setOnFinishEvent(delegate {
            state.finished = true;
        });
        instruction = PressInstruction.instance;
        instruction.reset();
    }

    public override void OnInput(string input, IBattler battler, ITargetSet targets)
    {
        if (!CorrectButton(input)) return;
        InstructionKeyEvent keyEvent = instruction.lookAtTime(input);
        // only react on keydown
        if (keyEvent == InstructionKeyEvent.KEYDOWN)
        {
            if (!state.triggered)
            {
                state.triggered = true;
                state.fired = true;
                executionEventInstance.OnExecute(battler, targets);
            } else
            {
                state.finished = true;
            }
        }
    }

    /* For testing*/
    public ExecutionEvent GetExecutionEvent() => executionEventInstance;
}
