using UnityEngine;
using System.Collections;
using Zenject;

public class ComboCounterInstaller : MonoInstaller
{
    public GameObject comboCounterPrefab = null;

    public override void InstallBindings()
    {
        Container.Bind<IComboCounter>().To<ComboCounter>().FromComponentInNewPrefab(comboCounterPrefab).AsSingle();
    }
}
