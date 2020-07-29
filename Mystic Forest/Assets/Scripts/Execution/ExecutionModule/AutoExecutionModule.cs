using UnityEngine;
using System.Collections;

public class AutoExecutionModule : MonoBehaviour, IAutoExecutionModule
{
    public void StartExecution(IAutoExecutableChain chain, IBattler attacker, ITargetSet target)
    {
        IEnumerator AutoExecuteChain()
        {
            Debug.Log("LETS GO FEVA TIME");
            foreach (var item in chain)
            {
                bool done = false;
                item.setOnCancellableEvent(() => done = true);
                item.setOnFinishEvent(() => done = true);
                item.OnExecute(attacker, target);
                yield return new WaitUntil(() => done);
            }
        }


        IEnumerator travel = chain.travelMethod.Travel(attacker.transform, target.GetTarget().transform.position + chain.startPositionOffset, chain.travelSpeed, () => StartCoroutine(AutoExecuteChain()));
        StartCoroutine(travel);
    }
}
