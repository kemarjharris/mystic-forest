using UnityEngine;
using UnityEditor;

public class MashInstruction : Instruction
{
    public static readonly MashInstruction instance = new MashInstruction();
    protected const float acceptedRange = 0.5f;

    public InstructionKeyEvent lookAtTime(string input, float timePressed, float endTime)
    {

        if (service.GetKeyDown(input))
        {
            successTimingDown = true;
            return timePressed <= endTime + acceptedRange ? InstructionKeyEvent.KEYDOWN : InstructionKeyEvent.BADKEY;

        } else if (successTimingDown && service.GetKeyUp(input))
        {
            return timePressed <= endTime + acceptedRange ? InstructionKeyEvent.KEYUP : InstructionKeyEvent.BADKEY;
        }
        else if (!successTimingDown && timePressed >= endTime + acceptedRange)
        {
            return InstructionKeyEvent.BADKEY;
        }
        return InstructionKeyEvent.NOKEY;
    }

}