using UnityEngine;
using UnityEditor;

public class AimExecutable : Executable
{
    public GameObject cursorPrefab = null;
    public float aimDuration = 0;
    public ExecutionEvent onStartAiming = null;
    public ExecutionEvent onTargetSelected = null;

    GameObject cursorGameObject = null;
    ICursor cursor;
    IHitBox cursorHitBox = null;
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
        cursorGameObject = Object.Instantiate(cursorPrefab, battler.gameObject.transform.position, Quaternion.identity);
        cursor = cursorGameObject.GetComponent<ICursor>();
        cursorHitBox = cursorGameObject.GetComponent<IHitBox>();
    }

    public void DespawnCursor()
    {
        Object.Destroy(cursorGameObject);
    }

    public void Aim(string input, IBattler battler, ITargetSet targets)
    {
        if (inputService.GetKey("up"))
        {
            cursor.Up();
        }
        if (inputService.GetKey("left"))
        {
            cursor.Left();
        }
        if (inputService.GetKey("down"))
        {
            cursor.Down();
        }
        if (inputService.GetKey("right"))
        {
            cursor.Right();
        }
        if (CorrectButton(input) && inputService.GetKeyUp(input))
        {
            // set target to place aimed
            bool specificTargetSelected = false;
            cursorHitBox.CheckCollision(delegate (Collider2D collider) {
                IBattler targeted = collider.gameObject.GetComponent<IBattler>();
                if (targeted == null) return;
                targets.SetTarget(targeted.gameObject.transform);
                specificTargetSelected = true;
            });
            if (!specificTargetSelected)
            {
                targets.SetTarget(cursorGameObject.transform);
            }
            // despawn cursor
            DespawnCursor();
            // handle event
            onTargetSelected.OnExecute(battler, targets);
            state.fired = true;
        }
    }
}