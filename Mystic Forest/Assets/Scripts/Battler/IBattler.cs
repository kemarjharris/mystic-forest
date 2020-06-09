using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattler
{
    GameObject gameObject { get; }
    Transform hitPoint { get; }

    void Play(IPlayableAnim animation);

    void StopCombatAnimation();

    void CheckCollision(System.Action<Collider> onCollide);

    void GetAttacked(IAttack attack);

    IExecutableChainSet ChainSet { get; }
    void FreezeFrame(float duration, System.Action onUnfreeze = null);

    bool IsGrounded { get; }
    bool IsFrozen { get; }

    void JumpIn(IBattler targetBattler);
}
