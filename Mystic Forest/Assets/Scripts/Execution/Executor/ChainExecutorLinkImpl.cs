using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class ChainExecutorLinkImpl : IChainExecutor// : Activity, Observable<AttackChainExecutionModule.ExecutionStatus>
{
    // Dependendencies
    IChainInputReader reader = new ChainInputReader();

    // Events
    public IActionWrapper OnChainCancellable { get; }
    public IActionWrapper OnChainFired { get; }
    public IActionWrapper OnChainFinished { get; }

    // State
    IBattler attacker;
    IEnumerator<IExecutable> seconds;
    IExecutable prev = null;
    IExecutable curr = null;
    bool timeCheck;

    public ChainExecutorLinkImpl()
    {
        OnChainCancellable = new ActionWrapper();
        OnChainFired = new ActionWrapper();
        OnChainFinished = new ActionWrapper();
    }

    public void ExecuteChain(IBattler attacker, IEnumerator<IExecutable> chain, Action onSuccessfulLoad = null)
    {
        this.attacker = attacker;
        Load(chain, onSuccessfulLoad);
    }

    void Load(IEnumerator<IExecutable> seconds, Action onSuccesfulLoad)
    {

        // Do not load new chain if chain is not null or chain is in cancel time
        if (!(prev == null || prev.IsInCancelTime())) return;
        onSuccesfulLoad?.Invoke();
        this.seconds = seconds;
        timeCheck = this.seconds.MoveNext();
        // have to keep prev from previous attack chain in case it wasnt done executing yet
        if (prev != null && prev.IsFinished())
        {
            prev = null;
        }
        curr = this.seconds.Current;
        curr.OnStart();
    }

    public void Update() {
        if (seconds == null) return;
        if (timeCheck) // Only try to execute if there are attacks in the chain
        { // Chain currently executing in the else block
            // Chain is waiting to be cancelled into next attack in this block
            // This block also executes if the current attack has been successfully triggered
            if (prev == null || prev.IsInCancelTime() || curr.IsTriggered())
            {

                string input = reader.ReadInput();
                curr.OnInput(input, attacker);
                // curr attack finished after input was read, move to next attack.
                // This block gets executed on the first frame where curr is in cancel time
                if (curr.HasFired())
                {
                    NextExecutable();
                }
            }
        }
        // polling for onCancellableEvent
        if (!timeCheck && curr != null && prev != null && prev.IsInCancelTime())
        {
            OnChainCancellable.Invoke();
            curr = null;
        }
        // Everything that happens in this block means the chain finished executing
        // Prev attack finished, current attack never triggered, unsuccessful chain
        if (ExecutionFinished())
        {
            // Runs when chain finishes running
            // tell observer that the attack chain is done
            StopExecuting();
        }
    }

    private bool ExecutionFinished()
    {
        return prev != null && prev.IsFinished() // prev has finished 
           && (curr == null || (curr != null && !curr.IsTriggered())) // end of chain OR middle of chain and hasnt been triggered
           || (curr != null && curr.IsFinished()); // curr failed
    }

    public void NextExecutable()
    {
        // Move to next attack
        prev = curr;
        timeCheck = seconds.MoveNext();
        // if timeCheck is true, there are attacks left in the chain
        if (timeCheck)
        {
            curr = seconds.Current;
            curr.OnStart();
        }
        else
        {
            // Notify observer that all attacks in the chain were fired
            OnChainFired.Invoke();
        }
    }

    public void StopExecuting()
    {
        seconds = null;
        prev = null;
        curr = null;
        OnChainFinished?.Invoke();
    }

    /* For testing purposes */
    public void Construct(IEnumerator<IExecutable> chain, IExecutable previous, IExecutable current)
    {
        seconds = chain;
        timeCheck = true;
        prev = previous;
        curr = current;
        if (ExecutionFinished())
        {
            timeCheck = false;
        }
    }

    public bool IsExecuting() => seconds != null;

    public IEnumerator<IExecutable> GetExecutables() => seconds;

    public IExecutable GetCurr() => curr;

    public IExecutable GetPrev() => prev;
}
