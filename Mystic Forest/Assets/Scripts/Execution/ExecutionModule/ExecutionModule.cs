using UnityEngine;
using UnityEditor;
using System.Runtime;
using System.Collections.Generic;
using System;

public class ExecutionModule : MonoBehaviour, IExecutionModule
{
    IDirectionCommandPicker<IExecutableChain> picker;
    IChainExecutor executor;
    ITargetSet targetSet;
    bool linkerActive;
    public IExecutableChainSet set { get; private set; }
    public IBattler battler { get; private set; }
    public IExecutableChain current;

    IActionWrapper IExecutionModule.OnChainCancellable => executor.OnChainCancellable;
    public IActionWrapper OnChainFired => executor.OnChainFired; 
    public IActionWrapper OnChainFinished => executor.OnChainFinished;
    IActionWrapper<IExecutableChain> IExecutionModule.OnChainSelected => picker.OnSelected;
    public IActionWrapper<ICustomizableEnumerator<IExecutable>> OnNewChainLoaded { get; private set; } = new ActionWrapper<ICustomizableEnumerator<IExecutable>>();
    public IActionWrapper OnNewSetLoaded { get; private set; } = new ActionWrapper();

    public void StartExecution(IExecutableChainSet set, IBattler battler, ITargetSet targetSet = null)
    {
        picker.Set(set);
        this.set = set;
        this.battler = battler;
        this.targetSet = targetSet;
        linkerActive = true;
        OnNewSetLoaded.Invoke();
    }

    public void StartExecution(IExecutableChain chain, IBattler battler, ITargetSet targetSet = null)
    {
        this.battler = battler;
        if (targetSet == null)
        {
            targetSet = new TargetSet();
        }
        picker.OnSelected.Invoke(chain);
        executor.Update();
    }

    public void ChangeSet(IExecutableChainSet set)
    {
        picker.Set(set);
        this.set = set;
        OnNewSetLoaded.Invoke();
    }

    public void Initialize(IDirectionCommandPicker<IExecutableChain> picker, IChainExecutor executor)
    {
        picker.OnSelected.AddAction(OnChainSelected);
        //executor.OnChainCancellable = onChainCancellable;
        executor.OnChainFired.AddAction(OnChainFiredEvent);
        executor.OnChainFinished.AddAction(OnChainFinishedEvent);
        this.picker = picker;
        this.executor = executor;
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
            ChangeSet(current.NextChains(battler.ChainSet).Where(chain => battler.IsGrounded ? !chain.IsAerial : chain.IsAerial));
        }
        linkerActive = true;
    }

    void OnChainFinishedEvent()
    {
        targetSet = null;
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