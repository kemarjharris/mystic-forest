using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(CapsuleCollider))]
public class HitCircle : MonoBehaviour, IHitBox
{ 
    CapsuleCollider hitCollider;

    private void Awake()
    {
        hitCollider = GetComponent<CapsuleCollider>();
    }

    public void CheckCollision(Action<Collider> onCollide)
    {
        if (onCollide == null) return;
        
        Vector3 pos = hitCollider.transform.position;
        float height = hitCollider.height / 2;
        Collider[] overlapColliders = Physics.OverlapCapsule(pos - new Vector3(0, 0, height), pos + new Vector3(0, 0, height), 
            hitCollider.radius);
        for (int i = 0; i < overlapColliders.Length; i++)
        {
            if (hitCollider != overlapColliders[i])
            {
                onCollide?.Invoke(overlapColliders[i]);
            }
        }
    }
}
