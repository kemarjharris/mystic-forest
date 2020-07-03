using UnityEngine;
using UnityEditor;

public class AimExecutable : Executable
{
    public GameObject cursorPrefab = null;
    public float aimDuration = 0;
    public ExecutionEvent onStartAiming = null;
    public ExecutionEvent onTargetSelected = null;

    static GameObject cursorGameObject = null;
    static ICursor cursor;
    static IHitBox cursorHitBox = null;
    bool aiming = false;
    IUnityInputService inputService = new UnityInputService();
    IUnityTimeService timeService = new UnityTimeService();
    float timeStartedAiming = 0;

    public override void OnStart()
    {
        aiming = false;
        state = new ExecutableState();
        onTargetSelected.setOnCancellableEvent(() => state.cancellable = true);
        onTargetSelected.setOnFinishEvent(() => state.finished = true);
    }

    public override void OnInput(string input, IBattler battler, ITargetSet targets)
    {
        if (!aiming)
        {
            WaitForInput(input, battler, targets);
        } else if (timeService.unscaledTime - timeStartedAiming <= aimDuration)
        {
            Aim(input, battler, targets);
        } else
        {
            DespawnCursor();
            state.finished = true;
        }
    }

    public void WaitForInput(string input, IBattler battler, ITargetSet targets)
    {
        if (CorrectButton(input) && inputService.GetKeyDown(input))
        {
            // spawn cursor
            SpawnCursor(battler);
            // Fire key down event
            onStartAiming.OnExecute(battler, targets);
            // start timer
            timeStartedAiming = timeService.unscaledTime;
            state.triggered = true;
            aiming = true;
        }
    }

    public void SpawnCursor(IBattler battler)
    {
        if (cursorGameObject == null)
        {
            Vector3 battlerPos = battler.transform.position;
            cursorGameObject = Object.Instantiate(cursorPrefab, new Vector3(battlerPos.x, 0, battlerPos.z), Quaternion.identity);
            cursor = cursorGameObject.GetComponent<ICursor>();
            cursorGameObject.transform.rotation = Quaternion.Euler(90, 0, 0);
            cursorHitBox = cursorGameObject.GetComponentInChildren<IHitBox>();
        } else
        {
            cursorGameObject.SetActive(true);
            Vector3 battlerPos = battler.transform.position;
            cursorGameObject.transform.position = new Vector3(battlerPos.x, 0, battlerPos.z);
        }
        
    }

    public void DespawnCursor()
    {
        if (cursorGameObject != null) cursorGameObject.SetActive(false);
    }

    public void Aim(string input, IBattler battler, ITargetSet targets)
    {
        if (inputService.GetKey("w"))
        {
            cursor.Up();
        }
        if (inputService.GetKey("a"))
        {
            cursor.Left();
        }
        if (inputService.GetKey("s"))
        {
            cursor.Down();
        }
        if (inputService.GetKey("d"))
        {
            cursor.Right();
        }
        if (CorrectButton(input) && inputService.GetKeyUp(input))
        {
            // set target to place aimed
            bool specificTargetSelected = false;
            cursorHitBox.CheckCollision(delegate (Collider collider) {
                IBattler targeted = collider.gameObject.GetComponent<IBattler>();
                if (targeted == null || targeted == battler) return;
                // targets.SetTarget(targeted.transform);
                onTargetSelected.pool.target = targeted.transform;
                targets.SetTarget(targeted.transform);
                specificTargetSelected = true;
            });
            if (!specificTargetSelected)
            {
                onTargetSelected.pool.target = cursorGameObject.transform;
            }
            // If no specific target was selected, then the transform is on a point on the floor. This means that it is a floor point.
            onTargetSelected.pool.floorPoint = !specificTargetSelected;
            // despawn cursor
            DespawnCursor();
            // handle event
            onTargetSelected.OnExecute(battler, targets);
            state.fired = true;
        }
    }
}