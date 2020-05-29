using UnityEngine;
using System.Collections;
using System;

public class Tbattler : MonoBehaviour, IBattler
{
    public IMixAnimator animator = null;
    public Transform hitPoint = null;
    IHitBox hitBox;
    MeshPhysicsZ physics = null;
    SpriteRenderer sprite;

    public float jumpForce = 8;
    public float jumpHorizontalForce;

    private void Awake()
    {
        animator = GetComponent<MixAnimator>();
        sprite = GetComponent<SpriteRenderer>();
        hitBox = GetComponentInChildren<IHitBox>();
        physics = GetComponent<MeshPhysicsZ>();
    }

    public void FixedUpdate()
    {
        if (physics.IsGrounded)
        {
            float horizontal = Input.GetAxis("Horizontal");
            if (Input.GetKey("k"))
            {
                // Jump
                physics.SetVelocity(VectorZ.zero, 0);
                if (horizontal > 0) horizontal = 1;
                else if (horizontal < 0) horizontal = -1;

                StartCoroutine(Jump(new VectorZ(jumpHorizontalForce * horizontal, 0), jumpForce));

                //physics.AddForce(new VectorZ(jumpHorizontalForce * horizontal, 0), jumpForce);

            }
            else
            {
                physics.Move(horizontal, 0);
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
            for (int i = 0; i < 3; i++)
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
