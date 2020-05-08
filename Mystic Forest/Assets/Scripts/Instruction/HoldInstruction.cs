using UnityEngine;
using UnityEditor;

public class HoldInstruction : Instruction
{
    public static readonly HoldInstruction instance = new HoldInstruction();
    private const float acceptedRange = 0.2f;

    public InstructionKeyEvent lookAtTime(string input, float timePressed, float releaseTime)
    {
        if (! (releaseTime > 0)) throw new System.ArgumentException("Release time must be positive");
        if (timePressed < 0) throw new System.ArgumentException("Time pressed must be non-negative");
        try
        {
            if (!successTimingDown && timePressed > releaseTime + acceptedRange)
            {
                // Key was never pressed
                return InstructionKeyEvent.BADKEY;
            }
            if (service.GetKeyDown(input))
            {
                // Keydown successful
                if (!successTimingDown)
                {
                    successTimingDown = true;
                    return InstructionKeyEvent.KEYDOWN;
                } else // key down pressed twice, bad key
                {
                    return InstructionKeyEvent.BADKEY;
                }
                
            }
            else if (successTimingDown && service.GetKey(input))
            {
                if (timePressed <= releaseTime + acceptedRange)
                {
                    // Key being held down
                    return InstructionKeyEvent.KEYHELD;
                }
            }

            if (successTimingDown && service.GetKeyUp(input))
            {
                if (Mathf.Abs(timePressed - releaseTime) < acceptedRange)
                {
                    // Key up in correct time
                    return InstructionKeyEvent.KEYUP;
                }
                else
                {
                    // Key up in wrong time
                    return InstructionKeyEvent.BADKEY;
                }
            }
            else if (successTimingDown && service.GetKey(input) && timePressed >= releaseTime + acceptedRange)
            {
                // Key was never released
                return InstructionKeyEvent.BADKEY;
            }
            return InstructionKeyEvent.NOKEY;
        } catch (System.ArgumentException)
        {
            return InstructionKeyEvent.NOKEY;
        }
        
    }

}