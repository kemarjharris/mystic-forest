using UnityEngine;
using UnityEditor;

[CreateAssetMenu()]
public class KeyDownMashExecutableSO : ExecutableSO
{
    public ExecutionEvent executionEvent;
    public ExecutionEvent mashTimeEndedEvent;
    public float mashDuration;

    public override IExecutable CreateExecutable() =>
        new KeyDownMashExecutable
        {
            button = button,
            executionEvent = Instantiate(executionEvent),
            mashTimeEndedEvent = Instantiate(mashTimeEndedEvent),
            mashDuration = mashDuration,
        };
}