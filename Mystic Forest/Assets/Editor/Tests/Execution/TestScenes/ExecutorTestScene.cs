using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecutorTestScene : MonoBehaviour
{
    public ExecutableChainSO testObject;
    public ExecutableChainVisual visual;
    public ChainExecutorLinkImpl chainExecutor;

    private void Update()
    {

        if (chainExecutor != null)
        {
            Debug.Log("vu so ve vi ka");
            chainExecutor.Update();
        }
        if (Input.GetKeyDown("space"))
        {
            if (chainExecutor == null)
            {
                ExecutableChainSO executable = Instantiate(testObject);
                if (visual != null) visual.Destroy();
                ICustomizableEnumerator<IExecutable> enumerator = executable.GetCustomizableEnumerator();
                visual = new ExecutableChainVisual(enumerator);
                enumerator.SetOnMoveNext(visual.MoveNext);
                chainExecutor = new ChainExecutorLinkImpl();
                chainExecutor.onChainCancellable = delegate {
                    visual.Destroy();
                    visual = null;
                    chainExecutor = null;
                };
                chainExecutor.ExecuteChain(null, null, enumerator);
            }
        }

        
    }
    
}
