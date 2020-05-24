﻿using UnityEngine;
using UnityEditor;
using System.Collections;

public class TravelForward : TravelSO
{
    public override IEnumerator Travel(Transform toMove, Transform dest, float speed)
    {
        do
        {
            float secondsPassed = Time.deltaTime;
            float distanceTravelled = speed * secondsPassed;
            toMove.gameObject.transform.position += new Vector3(distanceTravelled, 0);
            yield return null;
        } while (toMove != null);
    }
}