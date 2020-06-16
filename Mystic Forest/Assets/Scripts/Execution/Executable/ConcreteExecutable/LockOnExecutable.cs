using UnityEngine;
using UnityEditor;
using System.Collections;

public class LockOnExecutable : Executable
{

    public GameObject lockOnPrefab;
    public ExecutionEvent onStartLockOn;
    public ExecutionEvent onTargetSelected;
    public float lockOnDuration;

    public IUnityAxisService axisService = new UnityAxisService();
    public IUnityInputService inputService = new UnityInputService();
    public IUnityTimeService timeService = new UnityTimeService();

    private static LockOn lockOn;
    private static GameObject lockOnGameObject;
    private GameObject target;
    private float timeLockOnStarted;

    public override void OnInput(string input, IBattler battler, ITargetSet targets)
    {
        if (!CorrectButton(input)) return;
        // Wait to start locking on
        if (!lockOnGameObject.activeSelf)
        {
            if (inputService.GetKeyDown(input))
            {
                StartLockingOn(battler);
                state.triggered = true;
                target = lockOn.NextToLockOnTo();
                if (target != null)
                {
                    targets.SetTarget(target.transform);
                }
                onStartLockOn.OnExecute(battler, targets);
                timeLockOnStarted = timeService.unscaledTime;
            }
        } else if (state.triggered && timeService.unscaledTime - timeLockOnStarted <= lockOnDuration)
        {
            if (inputService.GetKeyUp(input))
            {
                if (target != null)
                {
                    targets.SetTarget(target.transform);
                    onTargetSelected.OnExecute(battler, targets);
                    state.fired = true;
                } else
                {
                    state.finished = true;
                }
                StopLockingOn();
                
            } else if (axisService.GetAxisDown("Horizontal") > 0)
            {
                target = lockOn.NextToLockOnTo();
            }
        } else
        {
            StopLockingOn();
            state.finished = true;
        }
    }

    private void StartLockingOn(IBattler battler)
    {
        lockOnGameObject.SetActive(true);
        lockOnGameObject.transform.SetParent(battler.gameObject.transform);
        lockOnGameObject.transform.localPosition = Vector3.zero;
        lockOnGameObject.transform.localScale = new Vector3(3.2f, 1f, 2.25f);
        lockOn.rule = (Collider collider) => collider.gameObject.tag == "Battler" && battler.gameObject != collider.gameObject;
    }

    private void StopLockingOn()
    {
        lockOnGameObject.SetActive(false);
    }

    public override void OnStart()
    {
        state = new ExecutableState();
        if (onStartLockOn == null || onTargetSelected == null || lockOnDuration <= 0)
        {
            throw new System.ArgumentException("Unacceptable value for Lock On Data On start");
        } 

        onTargetSelected.setOnCancellableEvent(() => state.cancellable = true);
        onTargetSelected.setOnFinishEvent(() => state.finished = true);
        if (lockOnGameObject == null)
        {
            lockOnGameObject = Object.Instantiate(lockOnPrefab);
            lockOn = lockOnGameObject.GetComponentInChildren<LockOn>();
        }
        lockOn.gameObject.SetActive(false);
    }

    /* for testing */

    public GameObject CurrentTarget() => target;

    public bool isLockingOn() => lockOnGameObject.activeSelf;

    public ExecutionEvent OnTargetSelectedEvent() => onTargetSelected;
}
 