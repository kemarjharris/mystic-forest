using UnityEngine;
using UnityEditor;

public class HoldInstruction : Instruction
{
    public static readonly HoldInstruction instance = new HoldInstruction();

    public InstructionKeyEvent lookAtTime(string input, float timePressed, float releaseTime)
    {
        if (releaseTime < 1) throw new System.ArgumentException();
        try
        {
            Debug.Log(string.Format("timeSincePress: {0}, releaseTime: {1}", timePressed, releaseTime));
            if (!successTimingDown && timePressed > releaseTime + acceptedRange)
            {
                // Key was never pressed
                return InstructionKeyEvent.BADKEY;
            }
            if (service.GetKeyDown(input) && !successTimingDown)
            {
                // Keydown successful
                successTimingDown = true;
                return InstructionKeyEvent.KEYDOWN;
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