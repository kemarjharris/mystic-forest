using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattler
{
    GameObject gameObject { get; }

    void Play(IPlayableAnim animation);

    void CheckCollision();

    void SetOnCollide(System.Action<Collider2D> onCollide);
}
