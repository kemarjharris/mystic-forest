using UnityEngine;
using System.Collections;

public class ExecutionController : MonoBehaviour
{
    public GameObject lockOnPrefab;
    private LockOn lockOn;

    // Used for execution
    IExecutionModule module;
    // The battler being controlled
    IBattler battler;
    // Currently targeted enemy
    ITargetSet target;
    public float skillTimeOut;
    float timeOut;
    public RangeSO closeRange;
    public ExecutableChainSO assault;

    private void Awake()
    {

        lockOn = Instantiate(lockOnPrefab).GetComponent<LockOn>();
        target = NewTargetSet();
        lockOn.onLockOn += delegate (GameObject t)
        {
            if (t != null && t.transform != target.GetTarget())
            {
                target.SetTarget(t.transform);
            }
        };
        lockOn.rule = (Collider collider) => collider.gameObject.tag == "Battler" && battler.transform != collider.transform;
        lockOn.transform.SetParent(transform);
        lockOn.transform.localPosition = Vector3.zero;
        module = GameObject.FindGameObjectWithTag("Execution Module").GetComponent<IExecutionModule>();
        battler = GetComponent<Battler>();
    }

    private void Update()
    {
        if (battler.executionState.combatState != CombatState.ATTACKING)
        {
            // not fighting
            if (!battler.executionState.comboing)
            {

                if (Input.GetKeyDown("j"))
                {
                    if (battler.IsGrounded && !closeRange.BattlerInRange(battler.transform))
                    {
                        module.StartExecution(assault, battler, target);
                    }
                    else
                    {
                        module.StartExecution(battler.ChainSet, battler, target);
                    }
                } else if (Input.GetKeyDown("k"))
                {
                    timeOut = skillTimeOut;
                    if (!battler.executionState.selectingSkill)
                    {
                        StartCoroutine(SelectSkill());
                    }
                }
            } else // fighting
            {
                if (Input.GetKeyDown("j"))
                {
                    module.StartExecution(battler.ChainSet, battler, target);
                }
            }

            if (Input.GetKeyDown("l"))
            {
                GameObject targ = lockOn.NextToLockOnTo();
            }
            else if (Input.GetKeyDown("q"))
            {
                lockOn.RemoveTarget();
                target.SetTarget(null);
            }
        }
    }

    ITargetSet NewTargetSet()
    {
        target = new EventTargetSet(delegate (Transform t) {
            if (lockOn.GetTarget() != t)
            {
                lockOn.SetTarget(t);
            }
        });
        return target;
    }

    public void OnEnable()
    {
        module.OnNewChainLoaded.AddAction(OnNewChainLoaded);
        module.OnChainCancellable.AddAction(OnChainCancellable);
        module.OnChainFinished.AddAction(OnChainFinished);
        SetUpComboEvents();
    }

    public void OnDisable()
    {
        module.OnNewChainLoaded.RemoveAction(OnNewChainLoaded);
        module.OnChainCancellable.RemoveAction(OnChainCancellable);
        module.OnChainFinished.RemoveAction(OnChainFinished);
        TearDownComboEvents();
    }

    void OnNewChainLoaded(ICustomizableEnumerator<IExecutable> obj)
    {
        battler.executionState.combatState = CombatState.ATTACKING;
        battler.executionState.selectingSkill = false;

    }
    void OnChainCancellable() => battler.executionState.combatState = CombatState.ABLE_TO_CANCEL_ATTACK;
    void OnChainFinished()
    {
        battler.StopCombatAnimation();
        battler.executionState.combatState = CombatState.NOT_ATTACKING;
    }

    void SetUpComboEvents()
    {
        GameObject counter = GameObject.FindGameObjectWithTag("Combo Counter");
        if (counter != null)
        {
            ComboCounter comboCounter = counter.GetComponent<ComboCounter>();
            comboCounter.onCountIncremented += RaiseComboFlag;
            comboCounter.onComboFinished += LowerComboFlag;
        }

    }

    void TearDownComboEvents()
    {
        GameObject counter = GameObject.FindGameObjectWithTag("Combo Counter");
        if (counter != null)
        {
            ComboCounter comboCounter = counter.GetComponent<ComboCounter>();
            comboCounter.onCountIncremented -= RaiseComboFlag;
            comboCounter.onComboFinished -= LowerComboFlag;
        }
    }

    void RaiseComboFlag(int i)
    {
        GetComponentInChildren<SpriteRenderer>().color = Color.cyan;
        battler.executionState.comboing = true;
    }

    void LowerComboFlag()
    {
        GetComponentInChildren<SpriteRenderer>().color = Color.white;
        battler.executionState.comboing = false;
        lockOn.RemoveTarget();
        target = NewTargetSet();
    }


    IEnumerator SelectSkill()
    {
        
        battler.executionState.selectingSkill = true;
        GetComponentInChildren<SpriteRenderer>().color = Color.yellow;
        module.StartExecution(battler.ChainSet, battler, target);

        float fdt = Time.fixedDeltaTime;
        Time.timeScale = 0.1f;
        Time.fixedDeltaTime = Time.timeScale * fdt;

        while (timeOut >= 0 && battler.executionState.selectingSkill)
        {
            timeOut -= Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = 1;
        Time.fixedDeltaTime = fdt;

        // timed out
        if (timeOut < 0)
        {
            battler.executionState.selectingSkill = false;
            module.StopExecution();
        }

        GetComponentInChildren<SpriteRenderer>().color = Color.white;
    }
}
