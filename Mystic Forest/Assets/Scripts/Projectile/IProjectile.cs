using UnityEngine;
using System.Collections;

public interface IProjectile
{
    void CheckCollision(System.Action<Collider> onCollide);

    GameObject gameObject { get; }
}
