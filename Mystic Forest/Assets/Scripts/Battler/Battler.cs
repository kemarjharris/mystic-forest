using UnityEngine;
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
        float freezeTime = 0.15f;
        FreezeFrame(freezeTime, () => physics.AddForce(attack.force, attack.verticalForce));
        StartCoroutine(FlashRed());
    }

    public void FreezeFrame(float duration, Action onUnfreeze = null)
    {
        // suspend in air and pause animation
        animator.Pause();
        physics.freeze = true;
        // wait for duration
        IEnumerator waitToUnfreeze()
        {
            
            while (duration > 0)
            {
                duration -= Time.fixedDeltaTime;
                yield return null;
            }

            animator.Unpause();
            physics.freeze = false;
            onUnfreeze?.Invoke();
        }
        // add gravity and play animation
        StartCoroutine(waitToUnfreeze());
    }
}
