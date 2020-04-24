using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressInstruction : Instruction {

    public static readonly PressInstruction instance = new PressInstruction();


    public InstructionKeyEvent lookAtTime(string input)
    {
        try
        {
            if (service.GetKeyDown(input) && !successTimingDown)
            {
                return InstructionKeyEvent.KEYDOWN;
            }
        } catch (System.ArgumentException)
        {
            return InstructionKeyEvent.NOKEY;
        }
        return InstructionKeyEvent.NOKEY;
    }
}
