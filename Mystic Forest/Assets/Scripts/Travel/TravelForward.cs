using UnityEngine;
using UnityEditor;
using System.Collections;

public class TravelForward : TravelMethodSO
{
    public override IEnumerator Travel(Transform toMove, Vector3 dest, float speed, System.Action onFinish = null)
    {
        do
        {
            float secondsPassed = Time.fixedDeltaTime;
            float distanceTravelled = speed * secondsPassed;
            toMove.position += new Vector3(distanceTravelled, 0, 0);
            yield return new WaitForFixedUpdate();
        } while (toMove != null);
        onFinish?.Invoke();
    }
}