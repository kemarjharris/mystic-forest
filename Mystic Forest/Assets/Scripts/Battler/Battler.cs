using UnityEngine;
using System.Collections;
using System;

public class Battler : MonoBehaviour, IBattler
{
    public IMixAnimator animator = null;
    public Transform hitPoint = null;
    IHitBox hitBox;
    SpriteRenderer sprite;
    

    private void Start()
    {
        animator = GetComponent<MixAnimator>();
        sprite = GetComponent<SpriteRenderer>();
        hitBox = GetComponentInChildren<HitPoint>();
        hitPoint.transform.position = new VectorZ(transform.position.x, transform.position.y);
    }

    public void Play(IPlayableAnim animation) => animator.Play(animation);
    public void FinishCombat() => animator.Stop(); // stops playing combat animations
    public void CheckCollision(Action<Collider> onCollide) => hitBox.CheckCollision(onCollide);
    Transform IBattler.hitPoint => hitPoint;

    public void GetAttacked()
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
        StartCoroutine(FlashRed());
    }
}
