using UnityEngine;
using System.Collections;
using Zenject;

[CreateAssetMenu()]
public class TargeterInstaller : ScriptableObjectInstaller
{
    public GameObject targeter;

    public override void InstallBindings()
    {
        Container.Bind<ITargeter>().FromComponentInNewPrefab(targeter).AsSingle();
    }
}
