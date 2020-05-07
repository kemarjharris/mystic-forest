using UnityEngine;
using UnityEditor;

[CreateAssetMenu()]
public class KeyDownMashExecutableSO : ExecutableSO
{
    public ExecutionEvent executionEvent;
    public ExecutionEvent mashTimeEndedEvent;
    public float mashDuration;

    // public ChainExecutionButton button;
    // private ExpandingButtonMashVisual visualPrefab;
    // private ExpandingButtonMashVisual visual;

    public void Construct(ExecutionEvent executionEvent, ExecutionEvent mashTimeEndedEvent, float mashDuration)
    {
        this.executionEvent = executionEvent;
        this.mashTimeEndedEvent = mashTimeEndedEvent;
        this.mashDuration = mashDuration;
    }

    public KeyDownMashExecutable CreateExecutable() =>
        new KeyDownMashExecutable
        {
            button = button,
            executionEvent = Instantiate(executionEvent),
            mashTimeEndedEvent = Instantiate(mashTimeEndedEvent),
            mashDuration = mashDuration,
        };
}