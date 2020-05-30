﻿using UnityEngine;
using System.Collections;
using System;

public class Battler : MonoBehaviour, IBattler
{
    public IMixAnimator animator = null;
    public Transform hitPoint = null;
    IHitBox hitBox;
    protected BattlePhysicsZ physics = null;
    SpriteRenderer sprite;
    public ExecutableChainSetSOImpl chainSet;

    protected void Awake()
    {
        animator = GetComponent<MixAnimator>();
        sprite = GetComponent<SpriteRenderer>();
        hitBox = GetComponentInChildren<IHitBox>();
        physics = GetComponent<BattlePhysicsZ>();
        if (physics == null)
        {
            physics = gameObject.AddComponent<BattlePhysicsZ>();
        }
        hitPoint.transform.position = new VectorZ(transform.position.x, transform.position.y);
    }

    public void StopCombatAnimation() => animator.Stop();

    public void Play(IPlayableAnim animation) => animator.Play(animation);
    
    public void CheckCollision(Action<Collider> onCollide) => hitBox.CheckCollision(onCollide);

    Transform IBattler.hitPoint => hitPoint;

    public bool IsGrounded => physics.IsGrounded;

    public IExecutableChainSet ChainSet => chainSet;

    public void GetAttacked(IAttack attack)
    {
        Debug.Log(name + "was hit");
        IEnumerator FlashRed()
        {
            float frames = 5;
            for (int i = 0; i < 3; i ++)
            {
                sprite.color = Color.red;
                int j = 0;
                while (sprite.color != Color.white)
                {
                    yield return null;
                    sprite.color = Color.LerpUnclamped(Color.red, Color.white, j / frames);
                    j++;
                    
                }
            }
        }
        physics.SetVelocity(attack.force, attack.verticalForce);
        StartCoroutine(FlashRed());
    }
}
