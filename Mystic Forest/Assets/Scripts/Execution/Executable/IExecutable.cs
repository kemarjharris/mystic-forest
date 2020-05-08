using UnityEngine;
using UnityEditor;

public interface IExecutable
{

    void OnInput(string input, IBattler battler, ITargetSet targets);

    void OnStart();

    bool IsTriggered();

    bool IsFinished();

    bool IsInCancelTime();

    DirectionCommandButton GetButton();

    //AttackVisual draw(Vector3 postion, Transform parent);
}