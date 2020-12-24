using UnityEngine;
using System.Collections;
using Zenject;

public class BattlefieldInstaller : MonoInstaller
{
    public LaneBattlefield field;

    public override void InstallBindings()
    {
        Container.Bind<LaneBattlefield>().To<LaneBattlefield>().FromInstance(field).AsSingle();
    }
}
