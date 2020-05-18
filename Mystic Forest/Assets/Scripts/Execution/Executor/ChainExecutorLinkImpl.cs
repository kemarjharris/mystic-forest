using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class ChainExecutorLinkImpl : IChainExecutor// : Activity, Observable<AttackChainExecutionModule.ExecutionStatus>
{
    //public ExecutableAttackChain executableAttackChainSO; 
    IEnumerator<IExecutable> seconds;
    bool timeCheck;
    //AttackChainExecutionVisual visual;
    //Observer<AttackChainExecutionModule.ExecutionStatus> observer;

    ITargetSet targets;
    IBattler attacker;

    IExecutable prev = null;
    IExecutable curr = null;

    public Action onChainCancellable;
    public Action onChainFinished;
    public Action onChainFired;

    Action IChainExecutor.OnChainCancellable { set => onChainCancellable = value; }
    Action IChainExecutor.OnChainFinished { set => onChainFinished = value; }
    Action IChainExecutor.OnChainFired { set => onChainFired = value; }

    IChainInputReader reader = new ChainInputReader();

    public void ExecuteChain(IBattler attacker, ITargetSet targets, IEnumerator<IExecutable> chain)
    {
        this.attacker = attacker;
        this.targets = targets;
        Load(chain);
        // visual = chain.getVisual(attacker);
        // visual.Expand();
    }

    void Load(IEnumerator<IExecutable> seconds)
    {
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
            if (prev == null || prev.HasFired() || curr.IsTriggered())
            {

                string input = reader.ReadInput();
                if (prev == null || prev.IsInCancelTime())
                {
                    curr.OnInput(input, attacker, targets);
                } else if (prev != null)
                {
                    // prev isnt cancellable yet, keep reading input for it
                    prev.OnInput(input, attacker, targets);
                }
                // curr attack finished after input was read, move to next attack.
                // This block gets executed on the first frame where curr is in cancel time
                if (curr.HasFired())
                {
                    NextExecutable();
                }
            }
        }
        // polling for onCancellableEvent
        if (!timeCheck && curr != null && prev.IsInCancelTime())
        {
            Debug.Log("FIRE");
            onChainCancellable?.Invoke();
            curr = null;
        }
        // Everything that happens in this block means the chain finished executing
        // Prev attack finished, current attack never triggered, unsuccessful chain
        if (ExecutionFinished())
        {
            // Runs when chain finishes running
            // tell observer that the attack chain is done
            seconds = null;
            onChainFinished?.Invoke();
        }
    }

    private bool ExecutionFinished()
    {
        return prev != null && prev.IsFinished() // prev has finished 
           && (curr == null || (curr != null && !curr.IsTriggered()));  // end of chain OR middle of chain and hasnt been triggered
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
            // Notify observer that the attack chain can now be cancelled
            onChainFired?.Invoke();
            //onChainCancellable?.Invoke();
        }
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
