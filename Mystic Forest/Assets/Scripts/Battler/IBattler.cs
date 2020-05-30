using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattler
{
    GameObject gameObject { get; }
    Transform hitPoint { get; }

    void Play(IPlayableAnim animation);

    void StartCombat();

    void FinishCombat();

    void FinishAttacking();

    void CheckCollision(System.Action<Collider> onCollide);

    void GetAttacked(IAttack attack);

    bool IsGrounded { get; }

    CombatState state { get; set; }
}
