using UnityEngine;
using UnityEditor;
using System.Runtime;
using System.Collections.Generic;
using System;

public class ExecutionModule : MonoBehaviour, IExecutionModule
{
    IDirectionCommandPicker<IExecutableChain> picker;
    IChainExecutor executor;
    IBattler battler;
    bool linkerActive;
    public IExecutableChainSet set { get; private set; }

    IActionWrapper IExecutionModule.OnChainCancellable => executor.OnChainCancellable;
    public IActionWrapper OnChainFired => executor.OnChainFired;
    public IActionWrapper OnChainFinished => executor.OnChainFinished;
    IActionWrapper<IExecutableChain> IExecutionModule.OnChainSelected => picker.OnSelected;
    public IActionWrapper<ICustomizableEnumerator<IExecutable>> OnNewChainLoaded { get; private set; }
    public IActionWrapper OnStart { get; private set; }

    public void StartExecution(IExecutableChainSet set, IBattler battler)
    {
        picker.Set(set);
        this.set = set;
        this.battler = battler;
        linkerActive = true;
        OnStart.Invoke();
    }

    public void Initialize(IDirectionCommandPicker<IExecutableChain> picker, IChainExecutor executor)
    {
        picker.OnSelected.AddAction(OnChainSelected);
        //executor.OnChainCancellable = onChainCancellable;
        executor.OnChainFired.AddAction(OnChainFiredEvent);
        executor.OnChainFinished.AddAction(OnChainFinishedEvent);
        this.picker = picker;
        this.executor = executor;
        OnNewChainLoaded = new ActionWrapper<ICustomizableEnumerator<IExecutable>>();
        OnStart = new ActionWrapper();
    }

    private void Awake()
    {
        Initialize(new DirectionCommandPicker<IExecutableChain>(0.4f), new ChainExecutorLinkImpl());
    }

    private void OnDestroy()
    {
        picker.OnSelected.RemoveAction(OnChainSelected);
        executor.OnChainFired.RemoveAction(OnChainFiredEvent);
        executor.OnChainFinished.RemoveAction(OnChainFinishedEvent);
    }

    private void Update()
    {
        if (linkerActive)
        {
            IExecutableChain chain = picker.InputSelect();
        }
        executor.Update();
    }

    void OnChainSelected(IExecutableChain chain)
    {
        linkerActive = false;
        ICustomizableEnumerator<IExecutable> enumerator = chain.GetCustomizableEnumerator();
        executor.ExecuteChain(battler, new TargetSet(), enumerator, () => OnNewChainLoaded.Invoke(enumerator));
    }

    void OnChainFiredEvent()
    {
        linkerActive = true;
    }

    void OnChainFinishedEvent()
    {
        linkerActive = false;
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