using UnityEngine;
using System.Collections;

public class NeutralController : IPlayerController
{
    IBattlerPhysics physics;
    BattlerSpeed speeds;
    float horizontal;
    float vertical;
    public IUnityAxisService service;
    Animator animator;
    IBattler battler;


    public NeutralController(IBattler battler, BattlerPhysics physics, BattlerSpeed speeds, Animator animator)
    {

        if (service == null) service = new UnityAxisService();
        this.physics = physics;
        this.speeds = speeds;
        this.animator = animator;
        this.battler = battler;
    }

    public void Update()
    {
        float oldHorizontal = horizontal;
        horizontal = service.GetAxisRaw("Horizontal");
        vertical = service.GetAxisRaw("Vertical");
        Debug.Log(horizontal);
        Debug.Log(vertical);
        bool moveAnimationPlaying = animator.GetBool("Moving");
        if (Mathf.Abs(vertical) > 0.2f || Mathf.Abs(horizontal) > 0.2f)
        {
            animator.SetBool("Moving", true);
            SpriteRenderer renderer = battler.gameObject.GetComponentInChildren<SpriteRenderer>();
            if (horizontal < 0 && !renderer.flipX)
            {
                renderer.flipX = true;
            } else if (horizontal > 0 && renderer.flipX)
            {
                renderer.flipX = false;
            }
        }
        else
        {
            animator.SetBool("Moving", false);
        }
    }

    public void FixedUpdate()
    {
        if (physics.IsGrounded)
        {
            float hVel = Mathf.Abs(horizontal) >= 0.5f ? horizontal : 0;
            float vVel = Mathf.Abs(vertical) >= 0.5f ? vertical : 0;
            if (hVel != 0 || vVel != 0)
            {
                physics.SetVelocity(new Vector3(hVel, 0, vVel) * speeds.speed);
            }
        }
    }

    public void OnEnable()
    {
    }

    public void OnDisable()
    {

    }
}
