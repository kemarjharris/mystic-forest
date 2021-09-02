using UnityEngine;
using System.Collections;
using Zenject;


public class DirectionCommandPickerInstaller : MonoInstaller
{

    public float timeBeforeClearingInput;
    IUnityTimeService uts;
    IUnityInputService uis;

    [Inject] 
    public void Construct(IUnityInputService inputService, IUnityTimeService timeService)
    {
        uts = timeService;
        uis = inputService;
    }

    public override void InstallBindings()
    {

        DirectionCommandPicker<IExecutableChain> picker = new DirectionCommandPicker<IExecutableChain>(timeBeforeClearingInput);
        picker.Construct(uts, uis);
        Container.Bind<IDirectionCommandPicker<IExecutableChain>>().FromInstance(picker).AsSingle();
    }
}
