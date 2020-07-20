using UnityEngine;
using System.Collections;
using Zenject;

public class UnityServiceInstaller : MonoInstaller
{

    [InjectOptional] public IUnityInputService inputService = new UnityInputService();
    [InjectOptional] public IUnityAxisService axisService = new UnityAxisService();
    [InjectOptional] public IUnityTimeService timeService = new UnityTimeService();

    
    public void Construct(IUnityAxisService axisService, IUnityInputService inputService, IUnityTimeService timeService)
    {
        this.inputService = inputService;
        this.axisService = axisService;
        this.timeService = timeService;
    }
    

    public override void InstallBindings()
    {
        Container.Bind<IUnityAxisService>().FromInstance(axisService).AsSingle().WhenNotInjectedInto<UnityServiceInstaller>();
        Container.Bind<IUnityInputService>().FromInstance(inputService).AsSingle().WhenNotInjectedInto<UnityServiceInstaller>();
        Container.Bind<IUnityTimeService>().FromInstance(timeService).AsSingle().WhenNotInjectedInto<UnityServiceInstaller>();
    }
}
