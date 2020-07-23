using UnityEngine;
using UnityEditor;
using Zenject;

public class BattlerEventSetInstaller : ScriptableObjectInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IBattlerEventSet>().To<BattlerEventSet>().AsSingle();
    }
}