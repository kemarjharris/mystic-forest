using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Executable/ExecutableSO/Key Down Mash Executable")]
public class KeyDownMashExecutableSO : ExecutableSO
{
    public ExecutionEvent executionEvent;
    public ExecutionEvent mashTimeEndedEvent;
    public float mashDuration;
    public bool aerial;

    public override IExecutable CreateExecutable() =>
        new KeyDownMashExecutable
        {
            button = button,
            executionEvent = Instantiate(executionEvent),
            mashTimeEndedEvent = Instantiate(mashTimeEndedEvent),
            mashDuration = mashDuration,
        };
}