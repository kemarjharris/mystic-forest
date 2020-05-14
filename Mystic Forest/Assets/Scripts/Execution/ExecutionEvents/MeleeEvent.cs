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
        attacker.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(AttackDelay(attacker, targets));
    }

    private IEnumerator AttackDelay(IBattler performer, ITargetSet targets)
    {
        yield return new WaitForSeconds(timeOfContact);
        performer.SetOnCollide(delegate(Collider2D collider) { Debug.Log("Collided with " + collider); });
        performer.CheckCollision();
        onCancellableEvent();
        
    }
}