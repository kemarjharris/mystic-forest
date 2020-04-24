using UnityEngine;
using System.Collections;

public class Instruction
{

    protected bool successTimingDown;
    protected const float acceptedRange = 0.2f;
    public IUnityService service;

    protected Instruction()
    {
        if (service == null)
            service = new UnityService();
    }

    public virtual void reset()
    {
        successTimingDown = false;
    }
}
