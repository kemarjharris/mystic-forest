using UnityEngine;
using UnityEditor;

public abstract class Executable : IExecutable
{
    public ExecutableState state;

    public DirectionCommandButton button;

    public DirectionCommandButton GetButton() => button;

    public bool IsInCancelTime() => state.cancellable;

    public bool IsFinished() => state.finished;

    public bool IsTriggered() => state.triggered;

    public abstract void OnInput(string input, IBattler battler, ITargetSet targets);

    public abstract void OnStart();

    
}