using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(CircleCollider2D))]
public class HitCircle : MonoBehaviour, IHitBox
{ 
    CircleCollider2D hitCollider;

    private void Awake()
    {
        hitCollider = GetComponent<CircleCollider2D>();
    }

    public void CheckCollision(Action<Collider2D> onCollide)
    {
        if (onCollide == null) return;
        Vector2 pos = hitCollider.transform.position;
        Collider2D[] overlapColliders = Physics2D.OverlapCircleAll(pos + hitCollider.offset,
            hitCollider.radius * hitCollider.transform.localScale.magnitude);
        for (int i = 0; i < overlapColliders.Length; i++)
        {
            if (hitCollider != overlapColliders[i])
            {
                onCollide?.Invoke(overlapColliders[i]);
            }
        }
    }
}
