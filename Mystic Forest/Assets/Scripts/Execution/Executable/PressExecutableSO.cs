using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PressExecutableSO : ExecutableSO {

    public PressInstruction instruction = null;
    public ExecutionEvent executionEvent = null;
    
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
        if (executionEvent == null)
        {
            throw new ArgumentException();
        }
        executionEvent.setOnCancellableEvent(delegate {
            state.cancellable = true;
        });
        executionEvent.setOnFinishEvent(delegate {
            state.finished = true;
        });
        instruction = PressInstruction.instance;
        instruction.reset();
    }

    public override void OnInput(string input, IBattler battler, ITargetSet targets)
    {
        InstructionKeyEvent keyEvent = instruction.lookAtTime(input);
        // only react on keydown
        if (keyEvent == InstructionKeyEvent.KEYDOWN)
        {
            if (!state.triggered)
            {
                state.triggered = true;
                executionEvent.OnExecute(battler, targets);
            } else
            {
                state.finished = true;
            }
        }
    }

    

    /*
    public override AttackVisual draw(Vector3 postion, Transform parent)
    {
        if (visualPrefab == null)
        {
            visualPrefab = Resources.Load<AttackVisual>("Prefabs/ExecutableAttackVisual");
        }

        return Instantiate(visualPrefab, postion , Quaternion.identity, parent.transform);
    }
    */

}
