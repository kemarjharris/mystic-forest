using UnityEngine;

[CreateAssetMenu(menuName = "Executable/Execution Event/Loop Event" )]
public class LoopEvent : ExecutionEvent
{
    public ExecutionEvent[] events;

    public override void OnExecute(IBattler attacker)
    {
        if (FireNext) return; // do nothing if event is going to fire again already
        if (IsExecuting)
        {
            // currently executing attack isnt finished yet, signal to fire again when the current one is done
            FireNext = true;
        }
        else // if no event is currently firing
        {
           
            void fireNextEvent()
            {
                // when the current event finishes, signal that its finished
                IsExecuting = false;
                // Move to the next event
                pos = (pos + 1) % events.Length;
                // if the signal was set to fire again, fire the next event in the loop
                if (FireNext)
                {
                    events[pos].setOnCancellableEvent(fireNextEvent);
                    // Execute the next attack
                    events[pos].OnExecute(attacker);
                    // Ready to set to fire again if more input occurs
                    FireNext = false;
                    IsExecuting = true;
                }
            }

            events[pos].setOnCancellableEvent(fireNextEvent);
            IsExecuting = true;
            // Fire the current event
            events[pos].OnExecute(attacker);
        }
    }

    public override void Interrupt()
    {
        FireNext = false;
    }

    /* For testing */
    public bool IsExecuting { get; set; } = false;
    public bool FireNext { get; set; } = false;
    public int pos { get; set; } = 0;
}