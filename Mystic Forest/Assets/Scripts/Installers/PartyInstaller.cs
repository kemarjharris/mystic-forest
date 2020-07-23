using UnityEngine;
using System.Collections;
using Zenject;
using System.Collections.Generic;

public class PartyInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Player[] players = FindObjectsOfType<Player>();
        List<IPlayer> playerList = new List<IPlayer>(players);
        Container.Bind<List<IPlayer>>().FromInstance(playerList).AsSingle();
    }
}
