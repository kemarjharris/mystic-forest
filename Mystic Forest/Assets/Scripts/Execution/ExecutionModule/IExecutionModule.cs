using UnityEngine;
using UnityEditor;

public interface IExecutionModule
{
    void StartExecution(IExecutableChainSet set, IBattler battler, ITargetSet targetSet = null);
    void StartExecution(IExecutableChain chain, IBattler battler, ITargetSet targetSet = null);
    void ChangeSet(IExecutableChainSet set);

    IActionWrapper OnChainCancellable { get; }
    IActionWrapper OnChainFired { get; }
    IActionWrapper OnChainFinished { get; }
    IActionWrapper<IExecutableChain> OnChainSelected { get; }
    IActionWrapper<ICustomizableEnumerator<IExecutable>> OnNewChainLoaded { get; }
    IActionWrapper OnNewSetLoaded { get; }

    IExecutableChainSet set { get; }
    IBattler battler { get; }


}