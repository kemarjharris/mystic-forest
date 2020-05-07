using UnityEngine;
using UnityEditor;

[CreateAssetMenu()]
public class OnReleaseHoldExecutableSO : ExecutableSO
{
    public ExecutionEvent keyDownExecutionEvent;
    public ExecutionEvent releaseExecutionEvent;
    public float releaseTime;

    public void Construct(ExecutionEvent keyDownExecutionEvent, ExecutionEvent releaseExecutionEvent, float releaseTime)
    {
        this.keyDownExecutionEvent = keyDownExecutionEvent;
        this.releaseExecutionEvent = releaseExecutionEvent;
        this.releaseTime = releaseTime;
    }

    public OnReleaseHoldExecutable CreateExecutable =>
        new OnReleaseHoldExecutable
        {
            keyDownExecutionEvent = Instantiate(keyDownExecutionEvent),
            releaseExecutionEvent = Instantiate(releaseExecutionEvent),
        };
}