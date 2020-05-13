using UnityEngine;
using System.Collections;

public class Battler : IBattler
{
    IExecutableChainSet executables;

    public void SetExecutables(IExecutableChainSet executables)
    {
        this.executables = executables;
    }
}
