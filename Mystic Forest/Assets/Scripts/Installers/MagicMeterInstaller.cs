using UnityEngine;
using System.Collections;
using Zenject;

public class MagicMeterInstaller : MonoInstaller
{

    public GameObject prefab;

    public override void InstallBindings()
    {
        Container.Bind<IMagicMeter>().FromComponentInNewPrefab(prefab).AsSingle();
    }
}
