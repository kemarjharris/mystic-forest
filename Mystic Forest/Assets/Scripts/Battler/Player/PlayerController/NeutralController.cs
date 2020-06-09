﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralController : IPlayerController
{
    IBattlerPhysics physics;
    BattlerSpeed speeds;
    float horizontal;
    float vertical;
    public IUnityAxisService service;
    private static ClosestLockOn lockOn;
    private Transform battlerTransform = null;
    private GameObject lockedOn;
    public float timeToHoldForLockOn = 0.2f;
    private float currentTime;


     

    public NeutralController(Transform battlerTransform, BattlerPhysics physics, BattlerSpeed speeds)
    {
        if (service == null) service = new UnityAxisService();
        if (lockOn == null) {
            lockOn = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Miscellaneous/Closest Lock On Area")).GetComponent<ClosestLockOn>();
            lockOn.rule = (Collider collider) => collider.gameObject.tag == "Battler";
            lockOn.enabled = false;
        }
        this.physics = physics;
        this.speeds = speeds;
        this.battlerTransform = battlerTransform;
    }

    public void Update()
    {
        horizontal = service.GetAxis("Horizontal");
        vertical = service.GetAxis("Vertical");

        if (Input.GetKey("z"))
        {
            // Wait until time for leap in
            currentTime += Time.deltaTime;
            // Scan for leap in
            if (currentTime >= timeToHoldForLockOn && !lockOn.scan)
            {
                lockOn.scan = true;
            }
        } else if (Input.GetKeyUp("z"))
        {
            if (currentTime < timeToHoldForLockOn)
            {
                // switch to combat mode

            } else 
            {
                
                if (lockOn.lockedOn != null)
                {
                    lockedOn = lockOn.lockedOn;
                    Debug.Log(lockedOn.name);
                    // leap in at lockon on
                }
                lockOn.scan = false;
                lockedOn = null;
            }
            currentTime = 0;
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
        lockOn.gameObject.transform.localScale = new Vector3(2.2f, 0.16f, 1.8f);
    }

    public void OnDisable()
    {
        lockOn.gameObject.SetActive(false);
    }
}
