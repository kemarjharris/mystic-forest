using UnityEngine;
using UnityEditor;
using System.Runtime;
using System.Collections.Generic;

public class ExecutionModule : MonoBehaviour, IExecutionModule
{
    IDirectionCommandPicker<IExecutableChain> picker;
    IChainExecutor executor;
    bool linkerActive;
    public System.Func<IExecutableChain, IEnumerator<IExecutable>> onNewChainSelected;
    public System.Action onChainCancellable;
    public System.Action onChainFinished;

    public void StartExecution(IExecutableChainSet set)
    {
        picker.Set(set);
    }

    public void Initialize(IDirectionCommandPicker<IExecutableChain> picker, IChainExecutor executor)
    {
        linkerActive = true;
        picker.OnSelected = delegate (IExecutableChain chain)
        {
            linkerActive = false;
            IEnumerator<IExecutable> enumerator = onNewChainSelected(chain);
            executor.ExecuteChain(null, null, enumerator);
        };
        executor.OnChainCancellable = delegate
        {
            onChainCancellable();
            linkerActive = true;
        };
        executor.OnChainFinished = delegate
        {
            onChainFinished();
            linkerActive = false;
        };
        this.picker = picker;
        this.executor = executor;
        
    }

    private void Start()
    {
        Initialize(new DirectionCommandPicker<IExecutableChain>(0.2f), new ChainExecutorLinkImpl());
    }

    private void Update()
    {
        Debug.Log(linkerActive);
        if (linkerActive)
        {
            IExecutableChain chain = picker.InputSelect();
        }
        executor.Update();
    }

    /* For testing */

    public bool LinkerIsActive() => linkerActive;
}

// states:
// not started - linker running but executor not
// started - linker not running executor is
// started - linker running executor is cancellable
// finished - linker not running executor not running

// chain selected makes executor continue with new chain