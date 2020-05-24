using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


public class MultiEvent : ExecutionEvent
{
    public ExecutionEvent[] events;
    public ISet<ExecutionEvent> cancellableEvents = new HashSet<ExecutionEvent>();
    public ISet<ExecutionEvent> finishedEvents = new HashSet<ExecutionEvent>();

    public override void OnExecute(IBattler attacker, ITargetSet targets)
    {
        // track all events
        if (cancellableEvents.Count > 0) cancellableEvents.Clear();
        if (finishedEvents.Count > 0) finishedEvents.Clear();
        foreach (ExecutionEvent @event in events)
        {
            void AddCancellableEvent()
            {
                cancellableEvents.Add(@event);
                if (cancellableEvents.Count == events.Length)
                {
                    onCancellableEvent?.Invoke();
                }
            }
            void AddFinishedEvent()
            {
                finishedEvents.Add(@event);
                if (finishedEvents.Count == events.Length)
                {
                    onFinishEvent?.Invoke();
                }
            }
            // when this event is cancellable, add it to the set
            @event.setOnCancellableEvent(AddCancellableEvent);
            // when this event is finished, add it from the set
            @event.setOnFinishEvent(AddFinishedEvent);
        }
    }
}