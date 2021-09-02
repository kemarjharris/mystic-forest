using UnityEngine;
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
    // The amount of time it takes to cast a skill
    public float skillTimeOut = 1;

    /// State
    float timeOut;
    // The battler being controlled
    public IBattler battler;
    
    // Definition of when to use assault
    public RangeSO closeRange;
    public ExecutableChainSO assault;
    Transform target;
    IDirectionCommandPicker<IExecutableChain> picker;

    [Inject]
    public void Construct(IUnityInputService inputService, IUnityTimeService timeService, IExecutionModule module, IComboCounter comboCounter, IDirectionCommandPicker<IExecutableChain> picker)
    {
        this.inputService = inputService;
        this.timeService = timeService;
        this.module = module;
        this.comboCounter = comboCounter;
        this.picker = picker;
       // this.target = targeter;
    }

    private void Awake()
    {
        battler = GetComponent<Battler>();
        battler.eventSet.onPlayerSwitchedIn += Enable;
        battler.eventSet.onPlayerSwitchedOut += LowerComboFlag;
        battler.eventSet.onPlayerSwitchedOut += Disable;

        picker.Set(battler.ChainSet);

        battler.targetSet.onTargetChanged += delegate (Transform t)
        {
            if (t == null)
            {
                battler.executionState.selectingSkill = false;
            }
            else if (target == null) // selecting a target for the first time;
            {
                battler.executionState.selectingSkill = true;
            }
            target = t;
        };
    }

    private void OnDestroy()
    {
        battler.eventSet.onPlayerSwitchedIn -= Enable;
        battler.eventSet.onPlayerSwitchedOut -= LowerComboFlag;
        battler.eventSet.onPlayerSwitchedOut -= Disable;
    }

    private void Update()
    { 
        if (battler.executionState.combatState == CombatState.NOT_ATTACKING && !battler.executionState.comboing)
        {
            // try to select a chain if targeting
            IExecutableChain selectedChain = null;
            if (target != null)
            {
                selectedChain = picker.InputSelect();
            }

            // if the attack button was pressed or the player was trying to attack 
            if (inputService.GetKeyDown("j") || selectedChain != null)
            {
                // get the direction command of the chain if selected chain wasnt null
                IDirectionCommand c = null;
                if (selectedChain != null)
                {
                    c = selectedChain.GetDirectionCommand();
                }

                // normal attack or not targetting and assaault condition, assault
                if ((target == null || normalAttack(c)) && battler.IsGrounded && !closeRange.BattlerInRange(battler.transform))
                {
                    module.StartExecution(assault, battler);
                } else if (selectedChain != null)// execute whatever the chain was
                {
                    module.StartExecution(selectedChain, battler);
                }
            }
        }
    }

    bool normalAttack(IDirectionCommand c) => c != null && c.directions.Length == 0 && c.option == DirectionCommandButton.J;
     
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
        module.StartExecution(battler.ChainSet, battler);

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
