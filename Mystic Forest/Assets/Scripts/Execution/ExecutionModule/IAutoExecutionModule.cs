using UnityEngine;
using UnityEditor;

public interface IAutoExecutionModule 
{
    void StartExecution(IAutoExecutableChain chain, IBattler attacker);
}