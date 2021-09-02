using UnityEngine;
using UnityEditor;
using System.Collections;

[CreateAssetMenu(menuName = "Executable/Execution Event/Effect Melee Event")]
public class EffectMeleeEvent : MeleeEvent
{
    public float effectSpawnTime;
    public PlayableAnimSO effectAnim;
    public GameObject effectPrefab;

    protected override IEnumerator AttackDelay(IBattler performer, System.Action onCancellableEvent, System.Action onFinishEvent)
    {
        yield return new WaitWhile(() => performer.IsFrozen);
        // play attack animation
        performer.Play(animSO);
        yield return new WaitForSeconds(effectSpawnTime);

        // spawn effect and play animation
        GameObject gameObject = Instantiate(effectPrefab, performer.transform.position, Quaternion.identity);
        MixAnimator animator = gameObject.GetComponent<MixAnimator>();
        animator.Play(effectAnim);
        // invoke cancellable event
        onCancellableEvent?.Invoke();
        yield return new WaitForSeconds(timeOfContact);
        // Get hitbox to use
        IHitBox hitBox = gameObject.GetComponentInChildren<IHitBox>();
        // check hitbox of effect
        hitBox.CheckCollision(delegate (Collider collider) {
            IBattler battler = collider.gameObject.GetComponent<Battler>();
            if (battler == null || battler == performer) return;
            attack.origin = gameObject.transform;
            battler.GetAttacked(attack);
        });
        yield return new WaitForSeconds(animSO.GetLength() - effectSpawnTime);
        Destroy(gameObject);
        onFinishEvent?.Invoke();
    }
    
}