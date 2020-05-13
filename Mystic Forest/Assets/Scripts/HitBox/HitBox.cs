using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class HitBox : MonoBehaviour, IHitBox
{
    CircleCollider2D hitCollider;
    Action<Collider2D> onCollide;

    private void Awake()
    {
        hitCollider = GetComponent<CircleCollider2D>();
    }

    public void CheckCollision()
    {
        if (onCollide == null) return;
        Collider2D[] overlapColliders = Physics2D.OverlapCircleAll(hitCollider.transform.position, hitCollider.radius);
        for (int i = 0; i < overlapColliders.Length; i ++)
        {
            if (hitCollider != overlapColliders[i])
            {
                onCollide?.Invoke(overlapColliders[i]);
            }
        }
    }

    public void SetOnCollide(Action<Collider2D> onCollide) => this.onCollide = onCollide;
}
