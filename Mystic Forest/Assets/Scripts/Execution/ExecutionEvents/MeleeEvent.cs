﻿using UnityEngine;
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
            bool madeContact = false;
            float freezeTime = 0.15f;
            performer.CheckCollision(delegate (Collider collider) {
                IBattler battler = collider.gameObject.GetComponent<Battler>();
                if (battler == null || battler == performer) return;
                madeContact = true;
                // aerial attack
                battler.GetAttacked(attack);
            });
            if (madeContact)
            {
                performer.FreezeFrame(freezeTime);
                onCancellableEvent?.Invoke();
            }
            float waitTime = anim.GetLength() - timeOfContact + (madeContact ? freezeTime : 0);
            yield return new WaitForSecondsRealtime(waitTime);
            onFinishEvent?.Invoke();
        }
    }
}