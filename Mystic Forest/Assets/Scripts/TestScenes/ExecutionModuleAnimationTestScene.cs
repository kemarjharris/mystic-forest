using System.Collections.Generic;
using UnityEngine;

public class ExecutionModuleAnimationTestScene : MonoBehaviour
{
    public ExecutableChainSetSOImpl set;
    private ExecutionModule module;
    public Battler battler;

    public Dictionary<Vector3, Battler> start = new Dictionary<Vector3, Battler>();

    private void Start()
    {
        module = new GameObject("Execution Module").AddComponent<ExecutionModule>();
        Battler[] battlers = FindObjectsOfType<Battler>();
        for (int i = 0; i < 1; i ++)
        {
            start.Add(battlers[i].transform.position, battlers[i]);
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            ExecutableChainSetVisual visual = CreateNewSetVisual();
            ExecutableChainVisual chainVisual = null;
            module.onNewChainLoaded = delegate (ICustomizableEnumerator<IExecutable> chain)
            {
                if (visual != null) visual.Destroy();
                chainVisual = new ExecutableChainVisual(chain);
                chain.SetOnMoveNext(chainVisual.MoveNext);

                battler.state = CombatState.ATTACKING;

            };
            module.onChainFired = delegate
            {
                visual = CreateNewSetVisual();
            };
            module.onChainCancellable = () => battler.state = CombatState.ABLE_TO_CANCEL_ATTACK;
            module.onChainFinished = delegate
            {

                battler.FinishAttacking();
                if (visual != null) visual.Destroy();
                if (chainVisual != null) chainVisual.Destroy();

                battler.state = CombatState.NOT_ATTACKING;
            };
            module.StartExecution(set, battler, () => battler.StartCombat());
        }
        if (Input.GetKeyDown("r"))
        {
            foreach (KeyValuePair<Vector3, Battler> pair in start)
            {
                pair.Value.transform.position = pair.Key;
            }
        }
    }

    private ExecutableChainSetVisual CreateNewSetVisual()
    {
        ExecutableChainSetVisual visual = new ExecutableChainSetVisual(set);
        visual.parent.transform.localPosition = new Vector2(-23, -129);
        return visual;
    }
}