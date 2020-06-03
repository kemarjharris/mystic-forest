using UnityEngine;
using UnityEditor;
using System;

public class PlayerController : MonoBehaviour, IPlayerController
{
    public BattlerSpeed speeds;
    IPlayerController neutral;
    IPlayerController combat;
    bool inCombat;

    private void Awake()
    {

        IBattler battler = GetComponent<IBattler>();
        BattlerPhysicsZ physics = GetComponent<BattlerPhysicsZ>();
        GameObject moduleGO = GameObject.Find("Execution Module");
        IExecutionModule module;
        if (moduleGO == null)
        {
            module = new GameObject("Execution Module").AddComponent<ExecutionModule>();
        } else
        {
            module = moduleGO.GetComponent<IExecutionModule>();
        }

        neutral = new NeutralController(physics, speeds);
        combat = new CombatController(battler, physics, module, speeds);
        
    }

    public void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (inCombat)
            {
                FinishCombat();
            }
            else
            {
                StartCombat();
            }
        }
        if (inCombat) {
            combat.Update();
        } else
        {
            neutral.Update();
        }
    }

    public void FinishCombat()
    {
        
        combat.OnDisable();
        inCombat = false;
        neutral.OnEnable();
    }

    public void StartCombat()
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

    public void OnEnable() { }

    public void OnDisable() { }
}