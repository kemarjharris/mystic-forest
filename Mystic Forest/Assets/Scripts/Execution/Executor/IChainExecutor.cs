using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChainExecutor
{
    void ExecuteChain(IBattler attacker, ITargetSet targets, IEnumerator<IExecutable> chain, System.Action onSuccessfulLoad = null);
    void StopExecuting();
    void Update();
    IActionWrapper OnChainCancellable { get; }
    IActionWrapper OnChainFired { get; }
    IActionWrapper OnChainFinished { get; }
}
