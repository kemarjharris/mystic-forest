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

    public override IExecutable CreateExecutable()
    {
        PressExecutable press = new PressExecutable
        {
            button = button,
            executionEvent = Instantiate(executionEvent)
        };
        return press;
    }
}
