using UnityEngine;
using System.Collections;

public abstract class Instruction
{

    protected bool successTimingDown;
    protected const float acceptedRange = 0.2f;
    public IUnityInputService service;

    protected Instruction()
    {
        if (service == null)
            service = new UnityInputService();
    }

    public virtual void reset()
    {
        successTimingDown = false;
    }
}
