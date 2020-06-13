using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Executable/Execution Event/Snap Event")]
public class SnapEvent : ExecutionEvent
{

    public TravelMethodSO travelMethod;
    public float travelTime;
    public PlayableAnimSO travelAnim;
    public ExecutionEvent nearPointEvent;
    public Vector3 nearPointDefinition;


    public override void OnExecute(IBattler battler, ITargetSet targetSet)
    {
        nearPointEvent.setOnCancellableEvent(onCancellableEvent);
        nearPointEvent.setOnFinishEvent(onFinishEvent);
        IBattler target = targetSet.GetTarget().GetComponent<Battler>();
        Vector3 snapPoint = Vector3.zero;
        if (target != null)
        {
            snapPoint = SnapPosition(battler.gameObject, target.gameObject);
        }
        else
        {
            snapPoint = targetSet.GetTarget().transform.position + (Vector3.up * battler.gameObject.transform.position.y);
        }

        if (battler.gameObject.transform.position == snapPoint)
        {
            snapPoint += Vector3.down * 0.1f;
        }

        float CalculateSpeed()
        {
            float time = Mathf.Max(travelTime, 0.01f);
            float distance = Vector3.Distance(battler.gameObject.transform.position, snapPoint);
            return distance / time;
        }
        float speed = CalculateSpeed();
        battler.StartCoroutine(ExecuteNearPointEvent(battler, targetSet, snapPoint));
        battler.StartCoroutine(travelMethod.Travel(battler.gameObject.transform, snapPoint, speed));
        battler.Play(travelAnim);
    }

    private IEnumerator ExecuteNearPointEvent(IBattler battler, ITargetSet target, Vector3 snapPoint)
    {
        bool isNearPoint()
        {
            Vector3 battlerPoint = battler.gameObject.transform.position;
            return Mathf.Abs(snapPoint.x - battlerPoint.x) < nearPointDefinition.x && Mathf.Abs(snapPoint.y - battlerPoint.y) < nearPointDefinition.y && Mathf.Abs(snapPoint.z - battlerPoint.z) < nearPointDefinition.z;
        }
        yield return new WaitUntil(isNearPoint);
        nearPointEvent.OnExecute(battler, target);
    }

    private Vector3 SnapPosition(GameObject battler, GameObject target)
    {
        BoxCollider jumpInCollider = battler.GetComponent<BoxCollider>();
        BoxCollider targetCollider = target.gameObject.GetComponent<BoxCollider>();
        float jumpInWidth = (jumpInCollider.size.x * jumpInCollider.gameObject.transform.localScale.x) / 2f;
        float targetWidth = (targetCollider.size.x * targetCollider.gameObject.transform.localScale.x) / 2f;
        float xDistance = jumpInWidth + targetWidth;
        if (battler.transform.position.x <= target.gameObject.transform.position.x)
        {
            return target.gameObject.transform.position - (Vector3.right * xDistance);
        }
        else
        {
            return target.gameObject.transform.position + (Vector3.right * xDistance);
        }
    }
}
