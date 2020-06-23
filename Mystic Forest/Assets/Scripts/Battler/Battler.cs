using UnityEngine;
using System.Collections;
using System;

public class Battler : MonoBehaviour, IBattler
{
    public IMixAnimator animator = null;
    public Transform hitPoint = null;
    IPlayerController controller;
    IHitBox hitBox;
    protected IBattlerPhysics physics = null;
    
    SpriteRenderer sprite;
    public ExecutableChainSetSOImpl chainSet;

    protected void Awake()
    {
        animator = GetComponentInChildren<MixAnimator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        controller = GetComponent<IPlayerController>();
        sprite.transform.forward = Camera.main.transform.forward;
        eventSet = new BattlerEventSet();

        hitBox = GetComponentInChildren<IHitBox>();
        physics = GetComponent<BattlerPhysics>();
        if (physics == null)
        {
            physics = gameObject.AddComponent<BattlerPhysics>();
        }
        hitPoint.transform.position = new Vector3(transform.position.x, transform.position.y, 0);
       
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
        if (attack.hasKnockBack) physics.SetVelocity(attack.force);
        // FreezeFrame(attack.freezeTime);
        StartCoroutine(FlashRed());
    }

    void Flinch(IAttack attack)
    {

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
                duration -= Time.deltaTime;
                yield return null;
            }

            animator.Unpause();
            physics.freeze = false;
            onUnfreeze?.Invoke();
        }
        // add gravity and play animation
        StartCoroutine(waitToUnfreeze());
    }

    public void SetVelocity(Vector3 velocity) => physics.SetVelocity(velocity);

    public bool IsFrozen => physics.freeze;

    public IBattlerEventSet eventSet { get; set; }
}
