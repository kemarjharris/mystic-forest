using UnityEngine;
using UnityEditor;

[CreateAssetMenu()]
public class OnReleaseHoldExecutableSO : ExecutableSO
{
    public ExecutionEvent keyDownExecutionEvent;
    public ExecutionEvent releaseExecutionEvent;
    public float releaseTime;

    public override IExecutable CreateExecutable()
    {
        return new OnReleaseHoldExecutable
        {
            keyDownExecutionEvent = Instantiate(keyDownExecutionEvent),
            releaseExecutionEvent = Instantiate(releaseExecutionEvent),
            releaseTime = releaseTime
        };
    }
}