using UnityEngine;
using System.Collections;

public interface IProjectile
{
    void CheckCollision(System.Action<Collider2D> onCollide);

    GameObject gameObject { get; }
}
