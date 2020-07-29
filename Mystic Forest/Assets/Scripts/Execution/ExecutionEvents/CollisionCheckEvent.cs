using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Executable/Execution Event/Collision Check Event")]
public class CollisionCheckEvent : ExecutionEvent
{
    public Attack attack;

    public override void OnExecute(IBattler performer, ITargetSet targets)
    {
        bool madeContact = false;
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
        } else
        {
            onFinishEvent.Invoke();
        }

    }
}