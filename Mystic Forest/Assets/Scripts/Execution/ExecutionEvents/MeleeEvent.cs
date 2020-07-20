using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Executable/Execution Event/Melee Event")]
public class MeleeEvent : ExecutionEvent
{
    public float timeOfContact;
    public PlayableAnimSO animSO;
    public Attack attack;
    public Vector3 playerVelocity;
    public bool aerial;
    public bool interrupted;

    public override void OnExecute(IBattler attacker, ITargetSet targets)
    {
        interrupted = false;
        attacker.StartCoroutine(AttackDelay(attacker, targets, onCancellableEvent, onFinishEvent));
    }

    public override void Interrupt()
    {
        interrupted = true;
    }

    protected virtual IEnumerator AttackDelay(IBattler performer, ITargetSet targets, System.Action onCancellableEvent, System.Action onFinishEvent)
    {
        yield return new WaitWhile(() => performer.IsFrozen);
        performer.Play(animSO);
        if (playerVelocity != Vector3.zero)
        {
            performer.SetVelocity(playerVelocity);
        }
        yield return new WaitForSeconds(timeOfContact);
        if (!interrupted)
        {
            bool madeContact = false;
            float freezeTime = 0.1f;
            performer.CheckCollision(delegate (Collider collider) {
                IBattler battler = collider.gameObject.GetComponent<Battler>();
                if (battler == null || battler == performer) return;
                madeContact = true;
                attack.origin = performer.transform;
                battler.GetAttacked(attack);
            });
            if (madeContact)
            {
                performer.FreezeFrame(attack.freezeTime);
                onCancellableEvent?.Invoke();
            }
            float waitTime = animSO.GetLength() - timeOfContact + (madeContact ? freezeTime : 0);
            yield return new WaitForSecondsRealtime(waitTime);
        }
        onFinishEvent?.Invoke();
    }
}
