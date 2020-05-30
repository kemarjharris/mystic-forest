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
            /*
            ExecutableChainVisual chainVisual = null;
            module.onNewChainLoaded = delegate (ICustomizableEnumerator<IExecutable> chain)
            {
                battler.state = CombatState.ATTACKING;

            };
            module.onChainCancellable = () => battler.state = CombatState.ABLE_TO_CANCEL_ATTACK;
            module.onChainFinished = delegate
            {
                battler.FinishAttacking();
                battler.state = CombatState.NOT_ATTACKING;
            };
            */
           // module.StartExecution(set, battler, () => battler.StartCombat());
        }
        if (Input.GetKeyDown("r"))
        {
            foreach (KeyValuePair<Vector3, Battler> pair in start)
            {
                pair.Value.transform.position = pair.Key;
            }
        }
    }
}