using UnityEngine;
using UnityEditor;

public abstract class InstructionFactory
{
    public static Instruction GetInstruction(InstructionID instructionID)
    {
        switch (instructionID)
        {
            
            case InstructionID.PRESS:
                return PressInstruction.instance;
           // case InstructionID.HOLD:
             //   return HoldInstruction.instance;
           // case InstructionID.MASH:
             //   return MashInstruction.instance;
        }
        return null;
    }

    public enum InstructionID
    {
        PRESS, HOLD, MASH
    }
}