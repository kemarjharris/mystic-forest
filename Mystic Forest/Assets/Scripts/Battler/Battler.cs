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

    float hitStun;

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
        executionState = new ExecutionState();
        ChainSet = new StateExecutableChainSetImpl(physics, executionState, chainSet);
        hitPoint.transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        
    }

    public void StopCombatAnimation() => animator.Stop();

    public void Play(IPlayableAnim animation) => animator.Play(animation);
    
    public void CheckCollision(Action<Collider> onCollide) => hitBox.CheckCollision(onCollide);

    Transform IBattler.hitPoint => hitPoint;

    public bool IsGrounded => physics.IsGrounded;

    public IExecutableChainSet ChainSet { get; private set; }

    public void GetAttacked(IAttack attack)
    {
        Debug.Log(name + "was hit");
        if (attack.hasKnockBack) physics.SetVelocity(attack.force);
        FreezeFrame(attack.freezeTime);
        Flinch(attack);
    }

    void Flinch(IAttack attack)
    {
        eventSet.onBattlerHit(this);
        IEnumerator flinch()
        {
            sprite.color = Color.red;
            for (; hitStun > 0; hitStun -= Time.deltaTime) yield return null;
            sprite.color = Color.white;
            eventSet.onBattlerRecovered(this);
        }

        float oldHitStun = hitStun;
        hitStun = attack.hitStun;
        if (oldHitStun <= 0)
        {
            StartCoroutine(flinch());
        }
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
    public ExecutionState executionState { get; private set; }
}
