using UnityEngine;
using UnityEditor;
using System;

public class PlayerController : MonoBehaviour, IMainPlayerController
{
    public BattlerSpeed speeds;
    public IPlayerController neutral;
    public IPlayerController combat;
    bool inCombat;

    private void Awake()
    {

        IBattler battler = GetComponent<IBattler>();
        BattlerPhysics physics = GetComponent<BattlerPhysics>();
        GameObject moduleGO = GameObject.Find("Execution Module");
        IExecutionModule module;
        if (moduleGO == null)
        {
            module = new GameObject("Execution Module").AddComponent<ExecutionModule>();
        } else
        {
            module = moduleGO.GetComponent<IExecutionModule>();
        }

        neutral = new NeutralController(battler, physics, speeds, this);
        combat = new CombatController(battler, physics, module, speeds, this);
        
    }

    public void Update()
    {
        if (inCombat) {
            combat.Update();
        } else
        {
            neutral.Update();
        }
    }

    public void SwapToNeutralMode()
    {
        
        combat.OnDisable();
        inCombat = false;
        neutral.OnEnable();
    }

    public void SwapToCombatMode()
    {
        neutral.OnDisable();
        inCombat = true;
        combat.OnEnable();
    }

    public void FixedUpdate() {
        if (inCombat)
        {
            combat.FixedUpdate();
        }else
        {
            neutral.FixedUpdate();
        }
    }

    public void OnEnable() {
        SwapToNeutralMode();
    }

    public void OnDisable() {
        if (inCombat)
        {
            combat.OnDisable();
        } else
        {
            neutral.OnDisable();
        }
    }
}