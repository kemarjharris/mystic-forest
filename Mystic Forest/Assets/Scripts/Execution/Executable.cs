using UnityEngine;
using UnityEditor;

public interface IExecutable
{

    void OnInput(string input, IBattler battler, ITargetSet targets);

    void OnStart();

    void OnFinish();

    bool IsTriggered();

    bool IsFinished();

    bool IsInCancelTime();

    //ChainExecutionButton getButton();

    //AttackVisual draw(Vector3 postion, Transform parent);
}