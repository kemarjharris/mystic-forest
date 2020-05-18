using UnityEngine;
using UnityEditor;
using System.Runtime;
using System.Collections.Generic;

public class ExecutionModule : MonoBehaviour, IExecutionModule
{
    IDirectionCommandPicker<IExecutableChain> picker;
    IChainExecutor executor;
    IBattler battler;
    bool linkerActive;
    public System.Action<ICustomizableEnumerator<IExecutable>> onNewChainSelected;
    public System.Action onChainCancellable;
    public System.Action onChainFinished;
    public System.Action onChainFired;

    public void StartExecution(IExecutableChainSet set, IBattler battler)
    {
        picker.Set(set);
        this.battler = battler;
        linkerActive = true;
    }

    public void Initialize(IDirectionCommandPicker<IExecutableChain> picker, IChainExecutor executor)
    {
        picker.OnSelected = delegate (IExecutableChain chain)
        {
            linkerActive = false;
            ICustomizableEnumerator<IExecutable> enumerator = chain.GetCustomizableEnumerator();
            onNewChainSelected?.Invoke(enumerator);
            executor.ExecuteChain(battler, null, enumerator);
        };
        executor.OnChainCancellable = delegate
        {
            onChainCancellable?.Invoke();
            linkerActive = true;
        };
        executor.OnChainFired = delegate
        {
            onChainFired.Invoke();
        };
        executor.OnChainFinished = delegate
        {
            onChainFinished?.Invoke();
            linkerActive = false;
        };
        this.picker = picker;
        this.executor = executor;
    }

    private void Start()
    {
        Initialize(new DirectionCommandPicker<IExecutableChain>(0.4f), new ChainExecutorLinkImpl());
    }

    private void Update()
    {
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