using UnityEngine;
using System.Collections;
using Zenject;
using System;

public class Targeter : MonoBehaviour, ITargeter
{

    public LockOn lockOn;
    IUnityInputService inputService;
    IPlayer battler;

    public ITargetSet targetSet => battler.targetSet;

    public Action<Transform> onLockOn { get; set; }

    [Inject]
    public void Construct(IPlayer battler, IUnityInputService inputService)
    {
        this.battler = battler;
        this.inputService = inputService;

    }

    private void Start()
    {

        battler.eventSet.onPlayerSwitchedIn += TargeterOn;
        battler.eventSet.onPlayerSwitchedOut += TargeterOff;

        battler.targetSet.onTargetChanged += delegate (Transform t)
        {
            if (lockOn.GetTarget() != t)
            {
                lockOn.SetTarget(t);
            }
            battler.target = t;
        };

        SetUpLockOn();
    }

    public void TargeterOn() => enabled = true;

    public void TargeterOff() => enabled = false;

    void SetUpLockOn()
    {
        lockOn.onLockOn += delegate (GameObject t)
        {
            if (t != null && t.transform != targetSet.GetTarget())
            {
                targetSet.SetTarget(t.transform);
                battler.target = t.transform;
            }
        };
        lockOn.onLockOn += delegate(GameObject go)
        {
            if (onLockOn != null && go != null)
            {
                onLockOn(go.transform);
            }
        };
        lockOn.rule = (Collider collider) => collider.gameObject.tag == "Battler" && battler.transform != collider.transform;
        lockOn.transform.SetParent(transform);
        lockOn.transform.localPosition = Vector3.zero;
    }


    private void Update()
    {
        if (inputService.GetKeyDown("l"))
        {
            GameObject targ = lockOn.NextToLockOnTo();
        }
        else if (Input.GetKeyDown("q"))
        {
            lockOn.RemoveTarget();
            targetSet.SetTarget(null);
        }
    }
}
