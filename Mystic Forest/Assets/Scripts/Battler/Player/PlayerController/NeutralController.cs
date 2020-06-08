using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralController : IPlayerController
{
    IBattlerPhysics physics;
    BattlerSpeed speeds;
    float horizontal;
    float vertical;
    public IUnityAxisService service;
    private static LockOn lockOn;
    private Transform battlerTransform;



    public NeutralController(Transform battlerTransform, BattlerPhysics physics, BattlerSpeed speeds)
    {
        if (service == null) service = new UnityAxisService();
        if (lockOn == null) {
            lockOn = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Miscellaneous/Lock On Area")).GetComponent<LockOn>();
            lockOn.rule = (Collider collider) => collider.gameObject.tag == "Battler";
        }
        this.physics = physics;
        this.speeds = speeds;
        this.battlerTransform = battlerTransform;
    }

    public void Update()
    {
        horizontal = service.GetAxis("Horizontal");
        vertical = service.GetAxis("Vertical");

        if (Input.GetKeyDown("l"))
        {
            lockOn.NextToLockOnTo();
        }
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
        lockOn.gameObject.SetActive(true);
        lockOn.gameObject.transform.SetParent(battlerTransform);
        lockOn.gameObject.transform.localPosition = Vector3.zero;
        lockOn.gameObject.transform.localScale = new Vector3(1.8f, 0.16f, 1.3f);
    }

    public void OnDisable()
    {
        lockOn.gameObject.SetActive(false);
    }
}
