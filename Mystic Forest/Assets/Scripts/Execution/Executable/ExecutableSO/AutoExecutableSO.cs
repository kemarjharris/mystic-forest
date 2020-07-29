using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Executable/ExecutableSO/Auto Executable")]
public class AutoExecutableSO : ExecutableSO
{
    public ExecutionEvent executionEvent = null;

    public override IExecutable CreateExecutable() =>
        new AutoExecutable
        {
            executionEvent = Instantiate(executionEvent)
        };
}