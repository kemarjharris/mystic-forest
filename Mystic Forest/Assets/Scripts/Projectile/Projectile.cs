using UnityEngine;
using System.Collections;
using System;

// Projectile that flies straight and is destroyed it makes contact
public class Projectile : MonoBehaviour, IProjectile
{
    IHitBox hitBox;

    private void Awake()
    {
        hitBox = GetComponent<IHitBox>();
    }

    public void CheckCollision(Action<Collider2D> onCollide)
    {
        onCollide += OnCollide;
        hitBox.CheckCollision(onCollide);
        onCollide -= OnCollide;
    }

    void OnCollide(Collider2D obj)
    {
        Destroy(gameObject);
    }

    public void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
