using UnityEngine;
using UnityEditor;
using System.Collections;

public class TravelPastPoint : TravelSO
{
    public override IEnumerator Travel(Transform toMove, Transform dest, float speed)
    {
        Vector3 startPos = toMove.position;
        Vector3 destPos = dest.position;
        float distance = Vector3.Distance((Vector2) startPos, (Vector2) destPos);
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