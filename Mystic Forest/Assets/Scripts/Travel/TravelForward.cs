using UnityEngine;
using UnityEditor;
using System.Collections;

[CreateAssetMenu()]
public class TravelForward : TravelMethodSO
{
    public override IEnumerator Travel(Transform toMove, Vector3 dest, float speed)
    {
        do
        {
            float secondsPassed = Time.deltaTime;
            float distanceTravelled = speed * secondsPassed;
            toMove.gameObject.transform.position += new Vector3(distanceTravelled, 0, 0);
            yield return null;
        } while (toMove != null);
    }
}