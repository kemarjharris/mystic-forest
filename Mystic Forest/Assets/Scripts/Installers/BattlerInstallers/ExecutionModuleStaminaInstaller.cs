using UnityEngine;
using UnityEditor;
using Zenject;

public class ExecutionModuleStaminaInstaller : ScriptableObjectInstaller
{
    public GameObject executionModulePrefab;
    [InjectOptional] IExecutionModule module = null;
    public float dcpResetTime;

    public override void InstallBindings()
    {
        if (module == null)
        {
            Container.Bind<IExecutionModule>().To<StaminaExecutionModule>().FromComponentInNewPrefab(executionModulePrefab).AsSingle();
            Container.Bind<IChainExecutor>().To<ChainExecutorLinkImpl>().AsSingle();
            Container.Bind<IDirectionCommandPicker<IExecutableChain>>().To<DirectionCommandPicker<IExecutableChain>>().AsSingle().WithArguments(dcpResetTime).WhenInjectedInto<IExecutionModule>();
        }
        else
        {
            Container.Bind<IExecutionModule>().FromInstance(module).AsSingle().WhenInjectedInto<ExecutionModuleInstaller>();
        }


    }
}