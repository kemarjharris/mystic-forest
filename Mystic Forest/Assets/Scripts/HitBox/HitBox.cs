using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class HitBox : MonoBehaviour, IHitBox
{
    BoxCollider2D hitCollider;
    
    private void Awake()
    {
        hitCollider = GetComponent<BoxCollider2D>();
    }

    public void CheckCollision(Action<Collider2D> onCollide)
    {
        if (onCollide == null) return;
        Collider2D[] overlapColliders = Physics2D.OverlapBoxAll(((Vector2) gameObject.transform.position) + hitCollider.offset, hitCollider.bounds.size, 0);
        for (int i = 0; i < overlapColliders.Length; i ++)
        {
            if (hitCollider != overlapColliders[i])
            {
                onCollide?.Invoke(overlapColliders[i]);
            }
        }
    }
}
