using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChainExecutor
{
    void ExecuteChain(IBattler attacker, ITargetSet targets, IEnumerator<IExecutable> chain, System.Action onSuccessfulLoad = null);
    void Update();
    System.Action OnChainCancellable { set; }
    System.Action OnChainFinished { set; }
    System.Action OnChainFired { set; }
}
