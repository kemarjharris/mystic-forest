using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PressExecutableSO : ExecutableSO {

    public PressInstruction instruction = null;
    public ExecutionEvent executionEvent = null;
    private bool isTriggered = false;
    private bool cancellable = false;
    private bool finished = false;

    //private static AttackVisual visualPrefab;
    //public ChainExecutionButton button;

    public void Construct(PressInstruction instruction, ExecutionEvent executionEvent)
    {
        
        this.instruction = instruction;
        this.executionEvent = executionEvent;
        this.executionEvent.setOnCancellableEvent(delegate {
            cancellable = true;
        });
        this.executionEvent.setOnFinishEvent(delegate {
            finished = true;
        });
    }

    public override void OnStart()
    {
        if (executionEvent == null)
        {
            throw new ArgumentException();
        }
        instruction = PressInstruction.instance;
        instruction.reset();
        isTriggered = false;
        cancellable = false;
        isTriggered = false;
    }

    public override void OnInput(string input, IBattler battler, ITargetSet targets)
    {
        InstructionKeyEvent keyEvent = instruction.lookAtTime(input);
        // only react on keydown
        if (keyEvent == InstructionKeyEvent.KEYDOWN)
        {
            if (!isTriggered)
            {
                isTriggered = true;
                executionEvent.OnExecute(battler, targets);
            } else
            {
                finished = true;
            }
        }
    }

    public override bool IsInCancelTime()
    {
        return cancellable;
    }

    public override bool IsFinished()
    {
        return finished;
    }

    public override bool IsTriggered()
    {
        return isTriggered;
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
