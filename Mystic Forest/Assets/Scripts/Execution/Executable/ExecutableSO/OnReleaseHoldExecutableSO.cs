using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Executable/ExecutableSO/OnReleaseHold Executable")]
public class OnReleaseHoldExecutableSO : ExecutableSO
{
    public ExecutionEvent keyDownExecutionEvent;
    public ExecutionEvent releaseExecutionEvent;
    public float releaseTime;

    public override IExecutable CreateExecutable()
    {
        return new OnReleaseHoldExecutable
        {
            button = button,
            keyDownExecutionEvent = Instantiate(keyDownExecutionEvent),
            releaseExecutionEvent = Instantiate(releaseExecutionEvent),
            releaseTime = releaseTime
        };
    }
}