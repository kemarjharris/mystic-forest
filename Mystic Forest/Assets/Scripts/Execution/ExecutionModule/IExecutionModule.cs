using UnityEngine;
using UnityEditor;

public interface IExecutionModule 
{
    void StartExecution(IExecutableChainSet set, IBattler battler);
}