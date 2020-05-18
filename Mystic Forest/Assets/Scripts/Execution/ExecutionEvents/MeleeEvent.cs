using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

[CreateAssetMenu(menuName = "Executable/Execution Event/Melee Event")]
public class MeleeEvent : ExecutionEvent
{
    public float timeOfContact;
    public PlayableAnimSO animSO;

    public override void OnExecute(IBattler attacker, ITargetSet targets)
    {
        attacker.Play(animSO);
        MeleeEventPOCO poco = new MeleeEventPOCO
        {
            onCancellableEvent = onCancellableEvent,
            onFinishEvent = onFinishEvent
        };
        attacker.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(poco.AttackDelay(attacker, targets, animSO, timeOfContact));
    }

    private class MeleeEventPOCO
    {
        public System.Action onFinishEvent;
        public System.Action onCancellableEvent;

        public IEnumerator AttackDelay(IBattler performer, ITargetSet targets, IPlayableAnim anim, float timeOfContact)
        {
            performer.SetOnCollide(delegate (Collider2D collider) { Debug.Log("Collided with " + collider); });
            yield return new WaitForSeconds(timeOfContact);
            performer.CheckCollision();
            onCancellableEvent?.Invoke();
            yield return new WaitForSeconds(anim.GetLength() - timeOfContact);
            onFinishEvent?.Invoke();
        }
    }
}