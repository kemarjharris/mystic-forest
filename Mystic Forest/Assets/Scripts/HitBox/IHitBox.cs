using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitBox
{
    void SetOnCollide(System.Action<Collider2D> onCollide);

    void CheckCollision();
}
