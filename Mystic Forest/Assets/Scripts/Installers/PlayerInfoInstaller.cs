using UnityEngine;
using UnityEditor;
using Zenject;

[CreateAssetMenu]
public class PlayerInfoInstaller : ScriptableObjectInstaller
{

    public GameObject playerInfoPrefab;

    public override void InstallBindings ()
    {
        Container.InstantiatePrefab(playerInfoPrefab);
    }
}