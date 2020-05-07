using UnityEngine;
using UnityEditor;

public class ExecutionModuleTestScene : MonoBehaviour
{
    public ExecutableChainSetSOImpl set;
    public ExecutionModule module;

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            ExecutableChainSetVisual visual = new ExecutableChainSetVisual(set);
            ExecutableChainVisual chainVisual = null;
            module.onNewChainSelected = delegate (IExecutableChain chain)
            {
                if (visual != null) visual.Destroy();
                chainVisual = new ExecutableChainVisual(chain);
                ICustomizableEnumerator<IExecutable> enumerator = chain.GetCustomizableEnumerator();
                enumerator.SetOnMoveNext(chainVisual.MoveNext);
                return enumerator;
            };
            module.onChainCancellable = delegate
            {
                visual = new ExecutableChainSetVisual(set);
            };
            module.onChainFinished = delegate
            {
                if (visual != null) visual.Destroy();
                if (chainVisual != null) chainVisual.Destroy();
            };

        }
    }

}