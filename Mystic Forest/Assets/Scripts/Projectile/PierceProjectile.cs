using UnityEngine;
using System.Collections;
using System;

public class PierceProjectile : MonoBehaviour, IProjectile
{
    IHitBox hitBox;

    private void Awake()
    {
        hitBox = GetComponent<IHitBox>();
    }

    public void CheckCollision(Action<Collider2D> onCollide)
    {
        hitBox.CheckCollision(onCollide);
    }

    public void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}