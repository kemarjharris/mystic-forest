using UnityEngine;
using System.Collections;
using Zenject;

public class MagicExecutionModuleInstaller : ScriptableObjectInstaller
{

    public GameObject executionModulePrefab;
    [InjectOptional] IExecutionModule module = null;
    public float dcpResetTime;

    public override void InstallBindings()
    {
        if (module == null)
        {
            Container.Bind<IExecutionModule>().To<MagicExecutionModule>().FromComponentInNewPrefab(executionModulePrefab).AsSingle();
            Container.Bind<IChainExecutor>().To<ChainExecutorLinkImpl>().AsSingle();
            Container.Bind<IDirectionCommandPicker<IExecutableChain>>().To<DirectionCommandPicker<IExecutableChain>>().AsSingle().WithArguments(dcpResetTime).WhenInjectedInto<IExecutionModule>();
            Container.Bind<IStaminaController>().FromComponentInChildren().AsSingle();
        }
        else
        {
            Container.Bind<IExecutionModule>().FromInstance(module).AsSingle().WhenInjectedInto<ExecutionModuleInstaller>();
        }
    }
}
