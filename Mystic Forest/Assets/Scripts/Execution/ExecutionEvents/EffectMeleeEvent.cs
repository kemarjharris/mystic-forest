using UnityEngine;
using UnityEditor;
using System.Collections;

[CreateAssetMenu(menuName = "Executable/Execution Event/Effect Melee Event")]
public class EffectMeleeEvent : ExecutionEvent
{
    public float effectSpawnTime;
    public float timeOfContact;
    public PlayableAnimSO playerAnim;
    public PlayableAnimSO effectAnim;
    public GameObject effectPrefab;
    public Attack attack;

    public override void OnExecute(IBattler attacker, ITargetSet targets)
    {
        MeleeEventPOCO poco = new MeleeEventPOCO
        {
            onCancellableEvent = onCancellableEvent,
            onFinishEvent = onFinishEvent
        };
        attacker.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(poco.AttackDelay(attacker, targets, this));
    }

    private class MeleeEventPOCO
    {
        public System.Action onFinishEvent;
        public System.Action onCancellableEvent;

        public IEnumerator AttackDelay(IBattler performer, ITargetSet targets, EffectMeleeEvent @event)
        {
            // play attack animation
            performer.Play(@event.playerAnim);
            yield return new WaitForSeconds(@event.effectSpawnTime);
            // spawn effect and play animation
            GameObject gameObject = Instantiate(@event.effectPrefab, performer.gameObject.transform.position, Quaternion.identity);
            MixAnimator animator = gameObject.GetComponent<MixAnimator>();
            animator.Play(@event.effectAnim);
            yield return new WaitForSeconds(@event.timeOfContact);
            // Get hitbox to use
            HitBox hitBox = gameObject.GetComponentInChildren<HitBox>();
            // check hitbox of effect
            hitBox.CheckCollision(delegate (Collider collider) {
                IBattler battler = collider.gameObject.GetComponent<Battler>();
                if (battler == null || battler == performer) return;
                battler.GetAttacked(@event.attack);
            });
            // invoke cancellable event
            onCancellableEvent?.Invoke();
            yield return new WaitForSeconds(@event.effectAnim.GetLength() - @event.timeOfContact);
            Destroy(gameObject);
            onFinishEvent?.Invoke();
        }
    }
}