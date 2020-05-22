using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitBox
{
    void CheckCollision(System.Action<Collider2D> onCollide);
}
