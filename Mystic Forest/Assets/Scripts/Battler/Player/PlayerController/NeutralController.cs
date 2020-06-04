using UnityEngine;
using System.Collections;

public class NeutralController : IPlayerController
{
    BattlerPhysicsZ physics;
    BattlerSpeed speeds;
    float horizontal;
    float vertical;
    public IUnityAxisService service;


    public NeutralController(BattlerPhysicsZ physics, BattlerSpeed speeds)
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
            physics.SetVelocity(new VectorZ(horizontal, vertical) * speeds.speed, 0);
        }
    }

    public void OnEnable()
    {
    }

    public void OnDisable()
    {

    }
}
