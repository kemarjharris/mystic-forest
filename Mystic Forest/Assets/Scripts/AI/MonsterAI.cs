using UnityEngine;
using UnityEditor;
using Zenject;
using System.Collections.Generic;

public class MonsterAI : MonoBehaviour
{
    public IBattler monster;
    public TargetSelector targetSelector;
    public AutoExecutableChainSO chain;
    protected List<IPlayer> players;
    protected IAutoExecutionModule module;



    [Inject]
    public void Construct(IBattler monster, List<IPlayer> players, IAutoExecutionModule module)
    {
        this.monster = monster;
        this.players = players;
        this.module = module;
    }

    public void Update()
    {
        if (Input.GetKeyDown("u"))
        {
            ITargetSet target = targetSelector.SelectTarget(players);
            module.StartExecution(chain, monster);
        }
       
    }
}