using UnityEngine;
using UnityEditor;

public interface IExecutable
{
    void OnInput(string input, IBattler battler);

    void OnStart();

    bool IsTriggered();

    bool IsFinished();

    bool IsInCancelTime();

    bool HasFired();

    DirectionCommandButton GetButton();
}