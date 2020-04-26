using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PressExecutableSO : ExecutableSO {

    
    public float cancelDuration;
    public PressInstruction instruction;
    //public ChainExecutionButton button;
    public ExecutionEvent executionEvent;
    public float timeBeforeCancel;

    //private static AttackVisual visualPrefab;
    private float timeTriggered;
    private float timeStarted;

    public override bool IsTriggered()
    {
        return timeTriggered > 0;
    }

    public override void OnStart()
    {
        instruction = PressInstruction.instance;
        instruction.reset();
        timeTriggered = 0;
        timeStarted = Time.unscaledTime;
    }

    public override void OnInput(string input, IBattler battler, ITargetSet targets)
    {
        if (input == "") return;
        bool successfulInput = instruction.lookAtTime(input) > 0;
        if (successfulInput)
        {
            timeTriggered = Time.unscaledTime;
            executionEvent.OnExecute(battler, targets);
        }
        
    }

    public override void OnFinish()
    {
        timeTriggered = 0;
    }

    public override bool IsInCancelTime()
    {
        
        return timeTriggered > 0 //  attack has been triggered
            && Time.unscaledTime - timeTriggered <= cancelDuration //  attakc is within cancel duration
            && Time.unscaledTime - timeStarted >= timeBeforeCancel; // attack has made contact
    }

    public override bool IsFinished()
    {
        return timeTriggered > 0 && Time.unscaledTime - timeTriggered > cancelDuration;
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
