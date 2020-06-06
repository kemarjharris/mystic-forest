using UnityEngine;
using System.Collections;
using System;

public class PierceProjectile : MonoBehaviour, IProjectile
{
    IHitBox hitBox;

    private void Awake()
    {
        hitBox = GetComponentInParent<IHitBox>();
    }

    public void CheckCollision(Action<Collider> onCollide)
    {
        hitBox.CheckCollision(onCollide);
    }

    public void OnBecameInvisible()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }
}