using UnityEngine;
using System.Collections;

public class Battler : MonoBehaviour, IBattler
{
    public IMixAnimator animator = null;
    public HitBox hitBox;

    private void Start()
    {
        animator = GetComponent<MixAnimator>();
    }

    public void SetOnCollide(System.Action<Collider2D> onCollide) => hitBox.SetOnCollide(onCollide);
    public void CheckCollision() => hitBox.CheckCollision();
    public void Play(IPlayableAnim animation) => animator.Play(animation);
    public void FinishCombat() => animator.Stop(); // stops playing combat animations
}
