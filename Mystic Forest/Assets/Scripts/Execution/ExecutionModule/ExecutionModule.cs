using UnityEngine;
using UnityEditor;
using System.Runtime;
using System.Collections.Generic;
using System;
using Zenject;

public class ExecutionModule : MonoBehaviour, IExecutionModule
{
    // Dependencies
    IDirectionCommandPicker<IExecutableChain> picker;
    protected IChainExecutor executor;
    // Events
    public IActionWrapper OnChainFired => executor.OnChainFired;
    public IActionWrapper OnChainFinished => executor.OnChainFinished;
    public IActionWrapper<ICustomizableEnumerator<IExecutable>> OnNewChainLoaded { get; private set; } = new ActionWrapper<ICustomizableEnumerator<IExecutable>>();
    public IActionWrapper OnNewSetLoaded { get; private set; } = new ActionWrapper();
    IActionWrapper IExecutionModule.OnChainCancellable => executor.OnChainCancellable;
    IActionWrapper<IExecutableChain> IExecutionModule.OnChainSelected => picker.OnSelected;
    // State
    protected ITargetSet targetSet;
    protected bool linkerActive;
    public IExecutableChainSet set { get; private set; }
    public IBattler battler;
    public IExecutableChain current;
    bool updated;

    [Inject]
    public void Construct(IDirectionCommandPicker<IExecutableChain> picker, IChainExecutor executor)
    {
        Initialize(picker, executor);
    }

    public void StartExecution(IExecutableChainSet set, IBattler battler, ITargetSet targetSet = null)
    {
        picker.Set(set);
        this.set = set;
        this.battler = battler;
        this.targetSet = targetSet;
        linkerActive = true;
        OnNewSetLoaded.Invoke();
        // Call again if the module already updated this frame, otherwise its going to update again on its own
        if (updated) Update();
    }

    public void StartExecution(IExecutableChain chain, IBattler battler, ITargetSet targetSet = null)
    {
        this.battler = battler;
        if (targetSet == null)
        {
            targetSet = new TargetSet();
        }
        picker.OnSelected.Invoke(chain);
        // Call again if the module already updated this frame, otherwise its going to update again on its own
        if (updated) Update();
    }

    // calls on chain finished which sets linker false
    public void StopExecution() => executor.StopExecuting();

    public void ChangeSet(IExecutableChainSet set)
    {
        picker.Set(set);
        this.set = set;
        OnNewSetLoaded.Invoke();
    }

    public void Initialize(IDirectionCommandPicker<IExecutableChain> picker, IChainExecutor executor)
    {
        picker.OnSelected.AddAction(OnChainSelected);
        executor.OnChainFired.AddAction(OnChainFiredEvent);
        executor.OnChainFinished.AddAction(OnChainFinishedEvent);
        this.picker = picker;
        this.executor = executor;
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
        updated = true;
    }

    private void LateUpdate()
    {
        updated = false;
    }

    protected virtual void OnChainSelected(IExecutableChain chain)
    {
        linkerActive = false;
        current = chain;
        ICustomizableEnumerator<IExecutable> enumerator = chain.GetCustomizableEnumerator();
        if (targetSet == null)
        {
            targetSet = new TargetSet();
        }
        executor.ExecuteChain(battler, targetSet, enumerator, () => OnNewChainLoaded.Invoke(enumerator));
    }

    void OnChainFiredEvent()
    {
        
        if (battler != null && battler.ChainSet != null)
        {
            ChangeSet(current.NextChains(battler.ChainSet));
        }
        
        linkerActive = true;
    }

    void OnChainFinishedEvent()
    {
        targetSet = null;
        linkerActive = false;
    }

    /* For DI */
    public class Factory : PlaceholderFactory<ExecutionModule> { }

    /* For testing */

    public bool LinkerIsActive() => linkerActive;

    
}

// states:
// not started - linker running but executor not
// started - linker not running executor is
// started - linker running executor is cancellable
// finished - linker not running executor not running

// chain selected makes executor continue with new chain