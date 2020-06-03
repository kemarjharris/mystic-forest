using UnityEngine;
using System.Collections;

public class NeutralController : IPlayerController
{
    BattlerPhysicsZ physics;
    BattlerSpeed speeds;
    float horizontal;
    float vertical;


    public NeutralController(BattlerPhysicsZ physics, BattlerSpeed speeds)
    {
        this.physics = physics;
        this.speeds = speeds;
    }

    public void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
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
