using UnityEngine;
using UnityEditor;
using System.Collections;

[CreateAssetMenu(menuName = "Executable/Execution Event/Effect Melee Event")]
public class EffectMeleeEvent : MeleeEvent
{
    public float effectSpawnTime;
    public PlayableAnimSO effectAnim;
    public GameObject effectPrefab;

    protected override IEnumerator AttackDelay(IBattler performer, ITargetSet targets, System.Action onCancellableEvent, System.Action onFinishEvent)
    {
        yield return new WaitWhile(() => performer.IsFrozen);
        // play attack animation
        performer.Play(animSO);
        yield return new WaitForSeconds(effectSpawnTime);

        // spawn effect and play animation
        GameObject gameObject = Instantiate(effectPrefab, performer.gameObject.transform.position, Quaternion.identity);
        MixAnimator animator = gameObject.GetComponent<MixAnimator>();
        animator.Play(effectAnim);
        yield return new WaitForSeconds(timeOfContact);
        // Get hitbox to use
        HitBox hitBox = gameObject.GetComponentInChildren<HitBox>();
        // check hitbox of effect
        bool madeContact = false;
        hitBox.CheckCollision(delegate (Collider collider) {
            IBattler battler = collider.gameObject.GetComponent<Battler>();
            if (battler == null || battler == performer) return;
            battler.GetAttacked(attack);
            madeContact = true;
        });
        if (madeContact)
        {
            // invoke cancellable event
            onCancellableEvent?.Invoke();
        }
        yield return new WaitForSeconds(effectAnim.GetLength() - timeOfContact);
        Destroy(gameObject);
        onFinishEvent?.Invoke();
    }
    
}