using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "split executable")]
public class SplitExecutableSO : ExecutableSO
{
    public float chargeTime;
    public float maxTime;
    public ExecutionEvent keyDown;
    public ExecutionEvent earlyEvent;
    public ExecutionEvent lateEvent;

    public override IExecutable CreateExecutable() =>
        new SplitExecutable
        {
            chargeTime = chargeTime,
            maxTime = maxTime,
            keyDown = keyDown,
            earlyEvent = earlyEvent,
            lateEvent = lateEvent
        };
}
