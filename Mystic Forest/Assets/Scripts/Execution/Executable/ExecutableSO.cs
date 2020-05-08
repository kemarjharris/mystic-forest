using UnityEngine;
using UnityEditor;

public abstract class ExecutableSO : ScriptableObject, IExecutable
{

    public DirectionCommandButton button;

    protected ExecutableState state;

    public bool IsInCancelTime() => state.cancellable;

    public bool IsFinished() => state.finished;

    public bool IsTriggered() => state.triggered;

    public abstract void OnInput(string input, IBattler battler, ITargetSet targets);
    public abstract void OnStart();

    public DirectionCommandButton GetButton() => button;

    protected  bool CorrectButton(string input) => button.ToString().ToLower().Equals(input.ToLower());

}