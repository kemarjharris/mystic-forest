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
        for (int i = 0; i < battlers.Length; i ++)
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
            };
            module.onChainFired = delegate
            {
                visual = CreateNewSetVisual();
            };
            module.onChainFinished = delegate
            {
                if (visual != null) visual.Destroy();
                if (chainVisual != null) chainVisual.Destroy();
                battler.animator.Stop();
            };
            module.StartExecution(set, battler);
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