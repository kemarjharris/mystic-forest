using UnityEngine;
using System.Collections;
using Zenject;
using System;

public class ExecutionModuleInstaller : MonoInstaller
{
    public float directionCommandPickerClearTime;
    public GameObject executionModulePrefab;
    [InjectOptional] IExecutionModule module = null;

    public override void InstallBindings()
    {
        if (module == null)
        {
            Container.Bind<IExecutionModule>().To<ExecutionModule>().FromComponentInNewPrefab(executionModulePrefab).AsSingle();
            Container.Bind<IChainExecutor>().To<ChainExecutorLinkImpl>().AsTransient();
            Container.Bind<IDirectionCommandPicker<IExecutableChain>>().To<DirectionCommandPicker<IExecutableChain>>().AsTransient().WithArguments(directionCommandPickerClearTime);
        } else
        {
            Container.Bind<IExecutionModule>().FromInstance(module).AsSingle().WhenInjectedInto<ExecutionModuleInstaller>();
        }

        
    }
}
