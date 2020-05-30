using UnityEngine;
using UnityEditor;
using NSubstitute;

public class ExecutionModuleTestScene : MonoBehaviour
{
    public ExecutableChainSetSOImpl set;
    private ExecutionModule module;

    private void Start()
    {
        module = new GameObject("Execution Module").AddComponent<ExecutionModule>();
        new GameObject("Executable Chain Set Visual").AddComponent<ExecutableChainSetVisual>();

    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            module.StartExecution(set, Substitute.For<IBattler>());
        }
    }
}