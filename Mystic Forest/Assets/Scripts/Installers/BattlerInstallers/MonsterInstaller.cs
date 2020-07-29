using UnityEngine;
using UnityEditor;
using Zenject;

[CreateAssetMenu]
public class MonsterInstaller : ScriptableObjectInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IBattler>().To<Battler>().FromComponentOnRoot().AsSingle();
    }
}