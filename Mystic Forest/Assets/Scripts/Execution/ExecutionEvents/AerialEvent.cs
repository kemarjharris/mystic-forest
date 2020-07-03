using UnityEngine;
using System.Collections;

public class AerialEvent : ExecutionEvent
{
    public ExecutionEvent executionEvent;

    public override void OnExecute(IBattler attacker, ITargetSet targets)
    {
        if (!attacker.IsGrounded)
        {

            bool eventFired = false;
            void cancelWrapper()
            {
                onCancellableEvent?.Invoke();
                eventFired = true;
            }
            void finishWrapper()
            {
                onFinishEvent?.Invoke();
                eventFired = true;
            }
            executionEvent.setOnCancellableEvent(cancelWrapper);
            executionEvent.setOnFinishEvent(finishWrapper);
            IEnumerator Interrupt()
            {
                yield return new WaitWhile(() => !eventFired && !attacker.IsGrounded);
                if (attacker.IsGrounded && !eventFired) executionEvent.Interrupt();
            }
            attacker.StartCoroutine(Interrupt());
            executionEvent.OnExecute(attacker, targets);
        } else
        {
            onFinishEvent?.Invoke();
        }
    }
}
