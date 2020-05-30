using UnityEngine;
using System.Collections;
using System;

public class Battler : MonoBehaviour, IBattler
{
    public IMixAnimator animator = null;
    public Transform hitPoint = null;
    public BattlerSpeed speeds;
    IHitBox hitBox;
    BattlePhysicsZ physics = null;
    SpriteRenderer sprite;
    public CombatState state { get; set; }
    

    public bool inCombat;

    private void Awake()
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

    public void Update()
    {

        Debug.Log(state);

        if (Input.GetKeyDown("j"))
        {
            if (inCombat)
            {
                FinishCombat();
            } else {
                StartCombat();
            }
        }
    }

    public void FixedUpdate()
    {
        if (inCombat)
        {
            CombatFixedUpdate();
        } else
        {
            NeutralFixedUpdate();
        }
    }

    public void NeutralFixedUpdate()
    {
        if (physics.IsGrounded)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            physics.SetVelocity(new VectorZ(horizontal, vertical) * speeds.speed, 0);
        }
    }

    public void CombatFixedUpdate()
    {
        // no input during combat
        if (state == CombatState.ATTACKING) return;

        if (physics.IsGrounded)
        {
            float horizontal = Input.GetAxis("Horizontal");
            // jump when attack is cancellable, jump cancel
            if (state != CombatState.ATTACKING && Input.GetAxis("Vertical") > 0)
            {
                // jump
                if (horizontal > 0) horizontal = 1;
                else if (horizontal < 0) horizontal = -1;
                physics.SetVelocity(new VectorZ(speeds.jumpHorizontalForce * horizontal, 0), speeds.jumpForce);
            } else if (state == CombatState.NOT_ATTACKING)
            {
                // move
                physics.SetVelocity(new VectorZ(horizontal * speeds.speed, 0), 0);
            }
        } 

    }

    public void Play(IPlayableAnim animation) => animator.Play(animation);
    
    public void CheckCollision(Action<Collider> onCollide) => hitBox.CheckCollision(onCollide);
    Transform IBattler.hitPoint => hitPoint;


    public void FinishCombat()
    {
        
        physics.lockZ = false;
        inCombat = false;
    }

    public void FinishAttacking()
    {
        animator.Stop(); // stops playing combat animations
    }

    public void StartCombat()
    {
        physics.lockZ = true;
        inCombat = true;
    }

    public bool IsGrounded => physics.IsGrounded;

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
