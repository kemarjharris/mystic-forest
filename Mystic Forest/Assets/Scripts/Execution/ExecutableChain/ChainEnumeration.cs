using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChainEnumeration
{
    private List<IExecutable> instances;

    public virtual IEnumerator<IExecutable> GetEnumerator(ExecutableSO[] attacks, bool aerial)
    {
        return GetCustomizableEnumerator(attacks, aerial);
    }

    IEnumerator<IExecutable> LoopEnumerator(ExecutableSO[] attacks, bool aerial)
    {
        instances = new List<IExecutable>();
        for (int i = 0; i < attacks.Length; i++)
        {
            if (aerial) attacks[i].isAerial = true;
            instances.Add(attacks[i].CreateExecutable());
        }
        return instances.GetEnumerator();
    }



    public virtual ICustomizableEnumerator<IExecutable> GetCustomizableEnumerator(ExecutableSO[] sOs, bool aerial)
    {
        return new CustomizableEnumerator<IExecutable>(LoopEnumerator(sOs, aerial));
    }
}
