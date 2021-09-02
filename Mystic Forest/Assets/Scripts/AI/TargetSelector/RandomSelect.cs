using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class RandomSelect : TargetSelector
{
    public override ITargetSet SelectTarget(List<IPlayer> players)
    {
        int pos = Random.Range(0, players.Count - 1);
        ITargetSet targetSet = new EventTargetSet();
        targetSet.SetTarget(players[pos].transform);
        return targetSet;
    }
}