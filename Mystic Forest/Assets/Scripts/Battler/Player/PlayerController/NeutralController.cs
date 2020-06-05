using UnityEngine;
using System.Collections;

public class NeutralController : IPlayerController
{
    IBattlerPhysics physics;
    BattlerSpeed speeds;
    float horizontal;
    float vertical;
    public IUnityAxisService service;


    public NeutralController(BattlerPhysics physics, BattlerSpeed speeds)
    {
        if (service == null) service = new UnityAxisService();
        this.physics = physics;
        this.speeds = speeds;
    }

    public void Update()
    {
        horizontal = service.GetAxis("Horizontal");
        vertical = service.GetAxis("Vertical");
    }

    public void FixedUpdate()
    {
        if (physics.IsGrounded)
        {
            physics.SetVelocity(new Vector3(horizontal, 0, vertical) * speeds.speed);
        }
    }

    public void OnEnable()
    {
    }

    public void OnDisable()
    {

    }
}
