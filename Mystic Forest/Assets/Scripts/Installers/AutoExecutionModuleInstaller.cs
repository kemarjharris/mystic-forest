using UnityEngine;
using UnityEditor;
using Zenject;

public class AutoExecutionModuleInstaller : ScriptableObjectInstaller
{
    public GameObject autoExecutionModulePrefab;

    public override void InstallBindings()
    {
        Container.Bind<IAutoExecutionModule>().FromComponentInNewPrefab(autoExecutionModulePrefab).AsSingle();
    }
}