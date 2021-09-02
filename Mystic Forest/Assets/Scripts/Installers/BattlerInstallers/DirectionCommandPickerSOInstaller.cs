using UnityEngine;
using UnityEditor;
using Zenject;

// IMPORTANT: When using this, the thing its being injected into must have the set injected to 
public class DirectionCommandPickerSOInstaller : ScriptableObjectInstaller
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
        Container.Bind<IDirectionCommandPicker<IExecutableChain>>().FromInstance(picker);
    }
}