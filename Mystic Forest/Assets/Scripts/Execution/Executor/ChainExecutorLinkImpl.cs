using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChainExecutorLinkImpl  : IChainExecutor// : Activity, Observable<AttackChainExecutionModule.ExecutionStatus>
{
    //public ExecutableAttackChain executableAttackChainSO; 
    IEnumerator<IExecutable> seconds;
    bool timeCheck;
    //AttackChainExecutionVisual visual;
    //Observer<AttackChainExecutionModule.ExecutionStatus> observer;

    ITargetSet targets;
    IBattler attacker;

    IExecutable prev = null;
    IExecutable curr;

    public System.Action OnChainCancellable;
    public System.Action OnChainFinished;

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

        // Everything that happens in this block means the chain finished executing
        // Prev attack finished, current attack never triggered, unsuccessful chain
        if (prev != null && prev.IsFinished() && curr != null && !curr.IsTriggered())
        {
            // Runs when chain finishes running
            // tell observer that the attack chain is done
            seconds = null;
            OnChainFinished?.Invoke();
            // notifyObserver(AttackChainExecutionModule.ExecutionStatus.CHAIN_FINISHED);
            // visual.Destroy();
        } else if (timeCheck) // Only try to execute if there are attacks in the chain
        { // Chain currently executing in the else block
            // Chain is waiting to be cancelled into next attack in this block
            // This block also executes if the current attack has been successfully triggered
            if (prev == null || prev.IsInCancelTime() || curr.IsTriggered())
            {
                // Read the input for this frame
                curr.OnInput("return"/*ChainInputReader.readInput()*/, attacker, targets);

                // curr attack finished after input was read, move to next attack.
                // This block gets executed on the first frame where curr is in cancel time
                if (curr.IsInCancelTime())
                {
                    NextExecutable();
                }
            }
        }
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
            // visual.MoveNext();
        }
        else
        {
            // Notify observer that the attack chain can now be cancelled
            // notifyObserver(AttackChainExecutionModule.ExecutionStatus.CHAIN_CANCELLABLE);
            // visual.Destroy();
            curr = null;
            OnChainCancellable?.Invoke();
        }
    }

    /* For testing purposes */
    public void Construct(IEnumerator<IExecutable> chain, IExecutable previous, IExecutable current)
    {
        seconds = chain;
        timeCheck = true;
        prev = previous;
        curr = current;
    }

    public bool IsExecuting() => seconds != null;

    public IEnumerator<IExecutable> GetExecutables() => seconds;

    public IExecutable GetCurr() => curr;

    public IExecutable GetPrev() => prev;



}
