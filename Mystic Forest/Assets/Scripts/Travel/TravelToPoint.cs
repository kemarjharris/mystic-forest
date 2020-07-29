using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

public class TravelToPoint : TravelMethodSO
{

    public override IEnumerator Travel(Transform toMove, Vector3 dest, float speed, Action onFinish = null)
    {

        float maxDistance = speed * Time.fixedDeltaTime;
        do
        {
            toMove.position = Vector3.MoveTowards(toMove.position, dest, maxDistance);
            yield return new WaitForFixedUpdate();
        } while (toMove.transform.position != dest);
        onFinish?.Invoke();
    }
}