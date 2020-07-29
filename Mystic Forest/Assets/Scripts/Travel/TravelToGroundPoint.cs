using UnityEngine;
using UnityEditor;
using System.Collections;

public class TravelToGroundPoint : TravelMethodSO
{
    public override IEnumerator Travel(Transform toMove, Vector3 dest, float speed, System.Action onFinish = null)
    {
        Vector3 destination = new Vector3(dest.x, toMove.position.y, dest.z);
        float maxDistance = speed * Time.fixedDeltaTime;
        do
        {
            toMove.position = Vector3.MoveTowards(toMove.position, destination, maxDistance);
            yield return new WaitForFixedUpdate();
        } while (Vector3.Distance(toMove.position, destination) > 0);
        onFinish?.Invoke();
    }
}