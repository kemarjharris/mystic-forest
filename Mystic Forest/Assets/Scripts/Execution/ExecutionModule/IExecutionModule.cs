using UnityEngine;
using UnityEditor;

public interface IExecutionModule
{
    void StartExecution(IExecutableChainSet set, IBattler battler);
    void ChangeSet(IExecutableChainSet set);

    IActionWrapper OnChainCancellable { get; }
    IActionWrapper OnChainFired { get; }
    IActionWrapper OnChainFinished { get; }
    IActionWrapper<IExecutableChain> OnChainSelected { get; }
    IActionWrapper<ICustomizableEnumerator<IExecutable>> OnNewChainLoaded { get; }
    IActionWrapper OnNewSetLoaded { get; }

    IExecutableChainSet set { get; }


}