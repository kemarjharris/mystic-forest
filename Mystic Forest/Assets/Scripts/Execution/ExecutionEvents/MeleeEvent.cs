using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Executable/Execution Event/Melee Event")]
public class MeleeEvent : ExecutionEvent
{
    public float timeOfContact;
    public PlayableAnimSO animSO;
    public Attack attack;

    public override void OnExecute(IBattler attacker, ITargetSet targets)
    {
        attacker.Play(animSO);
        MeleeEventPOCO poco = new MeleeEventPOCO
        {
            onCancellableEvent = onCancellableEvent,
            onFinishEvent = onFinishEvent
        };
        attacker.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(poco.AttackDelay(attacker, targets, animSO, attack, timeOfContact));
    }

    private class MeleeEventPOCO
    {
        public System.Action onFinishEvent;
        public System.Action onCancellableEvent;

        public IEnumerator AttackDelay(IBattler performer, ITargetSet targets, IPlayableAnim anim, IAttack attack, float timeOfContact)
        {
            yield return new WaitForSeconds(timeOfContact);
            performer.CheckCollision(delegate (Collider collider) {
                IBattler battler = collider.gameObject.GetComponent<Battler>();
                if (battler == null) return;
                battler.GetAttacked(attack);
            });
            onCancellableEvent?.Invoke();
            yield return new WaitForSeconds(anim.GetLength() - timeOfContact);
            onFinishEvent?.Invoke();
        }
    }
}