using UnityEngine;
using System.Collections;
using Zenject;

public class MagicExecutionModule : StaminaExecutionModule { 

    IMagicMeter meter;

    [Inject]
    public void Construct(IMagicMeter meter)
    {
        this.meter = meter;
    }

    protected override void OnChainSelected(IExecutableChain chain)
    {
        linkerActive = false;
        current = chain;

        if (chain is ISuper)
        {
            ISuper super = chain as ISuper;
            if (meter.Value >= super.manaCost)
            {
                meter.DecreaseMana(super.manaCost);
                base.OnChainSelected(chain);
            }
        } else
        {
            base.OnChainSelected(chain);
        }
    }
}
