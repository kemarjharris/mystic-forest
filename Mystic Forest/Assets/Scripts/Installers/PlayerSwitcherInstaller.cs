using UnityEngine;
using System.Collections;
using Zenject;

public class PlayerSwitcherInstaller : MonoInstaller
{
    public GameObject playerSwitcherPrefab;

    public override void InstallBindings()
    {
        Container.Bind<PlayerSwitcher>().FromComponentInNewPrefab(playerSwitcherPrefab).AsSingle();
    }

}
