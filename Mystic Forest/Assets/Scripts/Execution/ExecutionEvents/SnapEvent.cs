using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Executable/Execution Event/Snap Event")]
public class SnapEvent : ExecutionEvent
{

    public TravelMethodSO travelMethod;
    public float travelTime;
    public bool useAnimLength;
    public PlayableAnimSO travelAnim;
    public ExecutionEvent nearPointEvent;
    public Vector3 nearPointDefinition;


    public override void OnExecute(IBattler battler, ITargetSet targetSet)
    {
        nearPointEvent.setOnCancellableEvent(onCancellableEvent);
        nearPointEvent.setOnFinishEvent(onFinishEvent);
        IBattler target = null;
        if (pool.target == null)
        {
            target = targetSet.GetTarget().GetComponent<Battler>();
        } else
        {
            target = pool.target.GetComponent<Battler>();
        }
        
        Debug.Log(target.transform);
        Vector3 snapPoint = Vector3.zero;
        if (target != null)
        {
            snapPoint = SnapPosition(battler.transform.gameObject, target.transform.gameObject);
        }
        else
        {
            snapPoint = targetSet.GetTarget().transform.position + (Vector3.up * battler.transform.position.y);
        }

        if (battler.transform.position == snapPoint)
        {
            snapPoint += Vector3.down * 0.1f;
        }

        float CalculateSpeed()
        {
            float time = 0;
            if (useAnimLength)
            {
                time = travelAnim.GetLength() * travelAnim.speed;
            } else
            {
                time = Mathf.Max(travelTime, 0.01f);
            }

            float distance = Vector3.Distance(battler.transform.position, snapPoint);
            return distance / time;
        }
        float speed = CalculateSpeed();
        battler.StartCoroutine(ExecuteNearPointEvent(battler, targetSet, snapPoint));
        battler.StartCoroutine(travelMethod.Travel(battler.transform, snapPoint, speed));
        battler.Play(travelAnim);
    }

    private IEnumerator ExecuteNearPointEvent(IBattler battler, ITargetSet target, Vector3 snapPoint)
    {
        bool isNearPoint()
        {
            Vector3 battlerPoint = battler.transform.position;
            return Mathf.Abs(snapPoint.x - battlerPoint.x) < nearPointDefinition.x && Mathf.Abs(snapPoint.y - battlerPoint.y) < nearPointDefinition.y && Mathf.Abs(snapPoint.z - battlerPoint.z) < nearPointDefinition.z;
        }
        yield return new WaitUntil(isNearPoint);
        nearPointEvent.OnExecute(battler, target);
    }

    private Vector3 SnapPosition(GameObject battler, GameObject target)
    {
        BoxCollider jumpInCollider = battler.GetComponent<BoxCollider>();
        BoxCollider targetCollider = target.gameObject.GetComponent<BoxCollider>();
        float jumpInWidth = (jumpInCollider.size.x * jumpInCollider.transform.localScale.x) / 2f;
        float targetWidth = (targetCollider.size.x * targetCollider.transform.localScale.x) / 2f;
        float xDistance = jumpInWidth + targetWidth;
        if (battler.transform.position.x <= target.transform.position.x)
        {
            return target.transform.position - (Vector3.right * xDistance);
        }
        else
        {
            return target.transform.position + (Vector3.right * xDistance);
        }
    }
}
