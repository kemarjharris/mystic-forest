using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SplitExecutable : Executable
{
    public float chargeTime;
    public float maxTime;
    float timeCharged;
    bool timeFlag;

    public ExecutionEvent keyDown;
    public ExecutionEvent earlyEvent;
    public ExecutionEvent lateEvent;

    public override void OnInput(string input, IBattler battler, ITargetSet targets)
    {
        if (Input.GetKeyDown("j"))
        {
            state.triggered = true;
            keyDown.OnExecute(battler, targets);
            SelectTarget(battler, targets);
        }
        if (!timeFlag && timeCharged > chargeTime)
        {
            battler.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.magenta;            
            timeFlag = true;
        }
        if (state.triggered && Input.GetKey("j"))
        {
            timeCharged += Time.deltaTime;
            if (timeCharged > maxTime)
            {
                LateEvent(battler, targets);
            }
        }
        else if (state.triggered && !state.fired && Input.GetKeyUp("j"))
        {
            if (!timeFlag)
            {
                EarlyEvent(battler, targets);
            }
            else
            {
                LateEvent(battler, targets);
            }
        }

    }

    void EarlyEvent(IBattler battler, ITargetSet targets)
    {
        lateEvent.pool.target = targets.GetTarget();
        state.fired = true;
        earlyEvent.OnExecute(battler, targets);
    }

    void LateEvent(IBattler battler, ITargetSet targets)
    {
        lateEvent.pool.target = targets.GetTarget();
        state.fired = true;
        battler.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
        lateEvent.OnExecute(battler, targets);
    }

    void SelectTarget(IBattler battler, ITargetSet targets)
    {
        if (targets.GetTarget() == null)
        {
            GameObject[] battlerGOs = GameObject.FindGameObjectsWithTag("Battler");
            List<IBattler> battlers = new List<IBattler>();
            for (int i = 0; i < battlerGOs.Length; i++)
            {
                IBattler b = battlerGOs[i].GetComponent<IBattler>();
                if (b != battler)
                {
                    battlers.Add(b);
                }
            }

            int sortByDistanceFromCenter(IBattler x, IBattler y)
            {
                float xDistance = Vector3.Distance(new Vector3(x.gameObject.transform.position.x, 0, x.gameObject.transform.position.z), battler.gameObject.transform.position);
                float yDistance = Vector3.Distance(new Vector3(y.gameObject.transform.position.x, 0, y.gameObject.transform.position.z), battler.gameObject.transform.position);
                return xDistance.CompareTo(yDistance);
            }
            battlers.Sort(sortByDistanceFromCenter);
            targets.SetTarget(battlers[0].gameObject.transform);
            lateEvent.pool.target = battlers[0].gameObject.transform;
        }
    }

    public override void OnStart()
    {
        state = new ExecutableState();
        timeCharged = 0;
        earlyEvent.setOnCancellableEvent(delegate () { state.cancellable = true; });
        earlyEvent.setOnFinishEvent(delegate () { state.finished = true; });
        lateEvent.setOnCancellableEvent(delegate () { state.cancellable = true; });
        lateEvent.setOnFinishEvent(delegate () { state.finished = true; });
    }
}
