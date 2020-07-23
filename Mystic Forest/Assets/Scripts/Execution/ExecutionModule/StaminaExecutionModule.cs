using UnityEngine;
using System.Collections;
using Zenject;

public class StaminaExecutionModule : ExecutionModule
{
    IStaminaController controller;
    float cost;
    IBattlerEventSet eventSet;

    [Inject]
    public void Construct(IStaminaController controller, IBattlerEventSet eventSet)
    {
        this.controller = controller;
        this.eventSet = eventSet;
        eventSet.onEventExecuted += DecreaseStamina;
        eventSet.onComboFinished += RestoreStamina;
    }

    protected override void OnChainSelected(IExecutableChain chain)
    {
        linkerActive = false;
        current = chain;
        if (controller.stamina > 0)
        {
            cost = chain.staminaCost;
            ICustomizableEnumerator<IExecutable> enumerator = chain.GetCustomizableEnumerator();
            if (targetSet == null)
            {
                targetSet = new TargetSet();
            }
            executor.ExecuteChain(battler, targetSet, enumerator, () => OnNewChainLoaded.Invoke(enumerator));
        }
    }

    void DecreaseStamina()
    {
        controller.DecreaseStamina(cost);
        controller.StopRestoring();
        cost = 0;
    }

    void RestoreStamina()
    {
        controller.StartRestoring();
    }

    public void OnDestroy()
    {
        eventSet.onEventExecuted -= DecreaseStamina;
        eventSet.onEventExecuted -= RestoreStamina;
    }
}
