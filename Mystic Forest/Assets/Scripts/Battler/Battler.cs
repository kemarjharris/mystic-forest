using UnityEngine;
using System.Collections;
using System;

public class Battler : MonoBehaviour, IBattler
{
    public IMixAnimator animator = null;
    public Transform hitPoint = null;
    IHitBox hitBox;
    BattlePhysicsZ physics = null;
    SpriteRenderer sprite;
    public float jumpForce = 8;
    public float jumpHorizontalForce;
    public float speed = 10;
    public bool inCombat;

    private void Awake()
    {
        animator = GetComponent<MixAnimator>();
        sprite = GetComponent<SpriteRenderer>();
        hitBox = GetComponentInChildren<IHitBox>();
        physics = GetComponent<BattlePhysicsZ>();
        hitPoint.transform.position = new VectorZ(transform.position.x, transform.position.y);
    }

    public void FixedUpdate()
    {

        if (Input.GetKeyDown("j"))
        {
            inCombat = !inCombat;
        }

        if (!physics.IsGrounded) // fall
        {
            Vector3 currentVelocity = physics.GetVelocity();
            physics.SetVelocity(new VectorZ(currentVelocity.x, 0), currentVelocity.y + (Physics.gravity.y * Time.fixedDeltaTime));
        }

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
            physics.SetVelocity(new VectorZ(horizontal, vertical) * speed, 0);
        }
    }

    public void CombatFixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        if (physics.IsGrounded)
        {
            if (Input.GetAxis("Vertical") > 0)
            {
                // jump
                if (horizontal > 0) horizontal = 1;
                else if (horizontal < 0) horizontal = -1;
                physics.SetVelocity(new VectorZ(jumpHorizontalForce * horizontal, 0), jumpForce);
            } else
            {
                // move
                physics.SetVelocity(new VectorZ(horizontal * speed, 0), 0);
            }
        } 

    }

    public IEnumerator Jump(VectorZ hVel, float vVel)
    {
        do
        {
            physics.SetVelocity(hVel, vVel);
            vVel += Time.fixedDeltaTime * Physics.gravity.y;
            yield return new WaitForFixedUpdate();
        } while (!IsGrounded);
    }

    public void Play(IPlayableAnim animation) => animator.Play(animation);
    public void FinishCombat() => animator.Stop(); // stops playing combat animations
    public void CheckCollision(Action<Collider> onCollide) => hitBox.CheckCollision(onCollide);
    Transform IBattler.hitPoint => hitPoint;

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
