using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu]
public class CapsuleRangeSO : RangeSO
{
    public Vector3 point1;
    public Vector3 point2;
    public float radius;

    public override bool BattlerInRange(Transform transform)
    {
        Collider[] colliders = Physics.OverlapCapsule(transform.position + point1, transform.position + point2, radius);  //Physics.OverlapSphere(transform.position + (Vector3.right * 0.5f), 0.5f);
        List<Collider> inRange = new List<Collider>(colliders);
        inRange.RemoveAll((c) => c.gameObject.tag != "Battler" || c.transform == transform);
        return inRange.Count > 0;
    }

    public override void Draw(Transform transform)
    {
        CapsuleVisualizer.DrawWireCapsule(transform.position + point1, transform.position + point2, radius);
    }
}
