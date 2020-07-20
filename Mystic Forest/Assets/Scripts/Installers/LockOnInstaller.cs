using UnityEngine;
using System.Collections;
using Zenject;

public class LockOnInstaller : MonoInstaller
{

    public GameObject lockOnPrefab;

    public override void InstallBindings()
    {
        Container.Bind<LockOn>().FromComponentInNewPrefab(lockOnPrefab).AsTransient();
    }
}
