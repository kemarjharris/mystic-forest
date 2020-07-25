using UnityEngine;
using System.Collections;
using Zenject;

[CreateAssetMenu]
public class PlayerInstaller : ScriptableObjectInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IPlayer>().To<Player>().FromComponentOnRoot().AsSingle();
    }
}
