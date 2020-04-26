using UnityEngine;
using UnityEditor;

public abstract class ExecutableSO : ScriptableObject, IExecutable
{
    //public abstract AttackVisual draw(Vector3 postion, Transform parent);
    //public abstract ChainExecutionButton getButton();
    public abstract bool IsFinished();
    public abstract bool IsInCancelTime();
    public abstract bool IsTriggered();
    public abstract void OnInput(string input, IBattler battler, ITargetSet targets);
    public abstract void OnStart();
}