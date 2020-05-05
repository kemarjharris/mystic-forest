using UnityEngine;
using System.Collections;

public abstract class Instruction
{

    protected bool successTimingDown;
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
