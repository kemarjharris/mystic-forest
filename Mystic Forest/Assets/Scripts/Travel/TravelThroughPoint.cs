using UnityEngine;
using UnityEditor;
using System.Collections;

[CreateAssetMenu()]
public class TravelThroughPoint : TravelMethodSO
{
    public override IEnumerator Travel(Transform toMove, Vector3 destPos, float speed)
    {
        Vector3 startPos = toMove.position;
        //Vector3 destPos = dest.position + new Vector3(0, startPos.y, 0);
        float distance = Vector3.Distance(startPos, destPos);
        float newSpeed = distance / speed;
        float secondsPassed = 0;
        do
        {
            secondsPassed += Time.deltaTime / newSpeed;
            toMove.position = Vector3.LerpUnclamped(startPos, destPos, secondsPassed);
            yield return null;
        } while (toMove != null);
    }
}