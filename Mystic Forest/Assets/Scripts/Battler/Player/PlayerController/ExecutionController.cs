﻿using UnityEngine;
using System.Collections;
using Zenject;

[RequireComponent(typeof(Battler))]
public class ExecutionController : MonoBehaviour
{
    /// Dependencies 
    // Execution Module
    IExecutionModule module;
    // The interface for tracking combos
    IComboCounter comboCounter;
    // Unity Services
    IUnityInputService inputService;
    IUnityTimeService timeService;
    // Concrete gameobject
    ITargeter target;
    // The amount of time it takes to cast a skill
    public float skillTimeOut = 1;

    /// State
    float timeOut;
    // The battler being controlled
    public IBattler battler;
    
    // Definition of when to use assault
    public RangeSO closeRange;
    public ExecutableChainSO assault;

    [Inject]
    public void Construct(IUnityInputService inputService, IUnityTimeService timeService, IExecutionModule module, IComboCounter comboCounter, ITargeter targeter)
    {
        this.inputService = inputService;
        this.timeService = timeService;
        this.module = module;
        this.comboCounter = comboCounter;
        this.target = targeter;
    }

    private void Awake()
    {
        battler = GetComponent<Battler>();
        battler.eventSet.onPlayerSwitchedIn += Enable;
        battler.eventSet.onPlayerSwitchedOut += LowerComboFlag;
        battler.eventSet.onPlayerSwitchedOut += Disable;
    }

    private void OnDestroy()
    {
        battler.eventSet.onPlayerSwitchedIn -= Enable;
        battler.eventSet.onPlayerSwitchedOut -= LowerComboFlag;
        battler.eventSet.onPlayerSwitchedOut -= Disable;
    }

    private void Update()
    { 
        if (battler.executionState.combatState == CombatState.NOT_ATTACKING)
        {
            // not fighting
            if (!battler.executionState.comboing)
            {
                if (inputService.GetKeyDown("j"))
                {
                    if (battler.IsGrounded && !closeRange.BattlerInRange(battler.transform))
                    {
                        module.StartExecution(assault, battler, target.targetSet);
                    }
                    else
                    {
                        module.StartExecution(battler.ChainSet, battler, target.targetSet);
                    }
                } else if (inputService.GetKeyDown("k"))
                {
                    timeOut = skillTimeOut;
                    if (!battler.executionState.selectingSkill)
                    {
                        StartCoroutine(SelectSkill());
                    }
                }
            } else // fighting
            {
                if (inputService.GetKeyDown("j"))
                {
                    module.StartExecution(battler.ChainSet, battler, target.targetSet);
                }
            }
        }
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
        comboCounter.onCountIncremented += RaiseComboFlag;
        comboCounter.onComboFinished += LowerComboFlag;
    }

    void TearDownComboEvents()
    {
        comboCounter.onCountIncremented -= RaiseComboFlag;
        comboCounter.onComboFinished -= LowerComboFlag;
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
    }

    void Enable() => enabled = true;
    void Disable() => enabled = false;


    IEnumerator SelectSkill()
    {
        
        battler.executionState.selectingSkill = true;
        GetComponentInChildren<SpriteRenderer>().color = Color.yellow;
        module.StartExecution(battler.ChainSet, battler, target.targetSet);

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

    private void OnDrawGizmosSelected()
    {
        // closeRange.Draw(transform);
    }
}
