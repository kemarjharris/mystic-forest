using UnityEngine;

public class ExecutionModuleAnimationTestScene : MonoBehaviour
{
    public ExecutableChainSetSOImpl set;
    private ExecutionModule module;
    public Battler battler;

    private void Start()
    {
        module = new GameObject("Execution Module").AddComponent<ExecutionModule>();
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
    }

    private ExecutableChainSetVisual CreateNewSetVisual()
    {
        ExecutableChainSetVisual visual = new ExecutableChainSetVisual(set);
        visual.parent.transform.localPosition = new Vector2(-23, -129);
        return visual;
    }
}