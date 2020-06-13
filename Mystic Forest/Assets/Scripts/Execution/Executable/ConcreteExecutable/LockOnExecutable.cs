using UnityEngine;
using UnityEditor;

public class LockOnExecutable : Executable
{

    public GameObject lockOnPrefab;
    public ExecutionEvent onStartLockOn;
    public ExecutionEvent onTargetSelected;
    public float lockOnDuration;

    private static LockOn lockOn;
    private static GameObject lockOnGameObject;
    private static GameObject target;
    private float timeLockOnStarted;
    bool lockOnNextFrame;

    IUnityAxisService axisService = new UnityAxisService();
    IUnityInputService inputService = new UnityInputService();
    IUnityTimeService timeService = new UnityTimeService();

    public override void OnInput(string input, IBattler battler, ITargetSet targets)
    {
        if (lockOnNextFrame)
        {
            target = lockOn.NextToLockOnTo();
            lockOnNextFrame = false;
        }
        // Wait to start locking on
        if (!lockOnGameObject.activeSelf)
        {
            if (inputService.GetKeyDown("j"))
            {
                StartLockingOn(battler);
                state.triggered = true;
                onStartLockOn.OnExecute(battler, targets);
                timeLockOnStarted = timeService.unscaledTime;
            }
        } else if (timeService.unscaledTime - timeLockOnStarted <= lockOnDuration)
        {
            if (inputService.GetKeyUp("j"))
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
                
            } else if (DirectionalInputDown.InputOnAxisDown(true) > 0)
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
        lockOn.gameObject.transform.SetParent(battler.gameObject.transform);
        lockOn.gameObject.transform.localPosition = Vector3.zero;
        lockOn.gameObject.transform.localScale = new Vector3(3.2f, 0.16f, 2.25f);
        lockOnNextFrame = true;
    }

    private void StopLockingOn()
    {
        lockOnGameObject.SetActive(false);
    }

    public override void OnStart()
    {
        state = new ExecutableState();
        onTargetSelected.setOnCancellableEvent(() => state.cancellable = true);
        onTargetSelected.setOnFinishEvent(() => state.finished = true);
        if (lockOnGameObject == null)
        {
            lockOnGameObject = Object.Instantiate(lockOnPrefab);
            lockOn = lockOnGameObject.GetComponent<LockOn>();
            lockOn.rule = (Collider collider) => collider.gameObject.tag == "Battler";
        }
        lockOn.gameObject.SetActive(false);
    }
}
 