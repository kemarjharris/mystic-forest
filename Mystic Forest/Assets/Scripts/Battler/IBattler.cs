using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattler
{
    GameObject gameObject { get; }
    Transform hitPoint { get; }

    void Play(IPlayableAnim animation);

    void FinishCombat();

    void CheckCollision(System.Action<Collider> onCollide);

    void GetAttacked();
}
