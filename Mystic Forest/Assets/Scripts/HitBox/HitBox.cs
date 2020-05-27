using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class HitBox : MonoBehaviour, IHitBox
{
    BoxCollider hitCollider;
    
    private void Awake()
    {
        hitCollider = GetComponent<BoxCollider>();
    }

    public void CheckCollision(Action<Collider> onCollide)
    {
        if (onCollide == null) return;
        Collider[] overlapColliders = Physics.OverlapBox(hitCollider.gameObject.transform.position + hitCollider.center, hitCollider.bounds.extents, hitCollider.gameObject.transform.rotation);
        for (int i = 0; i < overlapColliders.Length; i ++)
        {
            if (hitCollider != overlapColliders[i])
            {
                onCollide?.Invoke(overlapColliders[i]);
            }
        }
    }
}
