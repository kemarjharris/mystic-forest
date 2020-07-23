using UnityEngine;
using UnityEditor;
using Zenject;
using System.Collections.Generic;

public class StaminaControllerInstaller : ScriptableObjectInstaller
{ 
    public List<GameObject> observers;
    public BoundedFloat stamina;
    public float restorationPerSecond;
    public bool restore;

    public override void InstallBindings()
    {
        StaminaController controller = new StaminaController
        {
            observers = observers,
            stamina = stamina,
            restorationPerSecond = restorationPerSecond,
            restore = restore
        };
        controller.stamina.Value = controller.stamina.MaxValue;
        Container.BindInterfacesAndSelfTo<IStaminaController>().FromInstance(controller).AsSingle();
        // Container.Bind<IStaminaController>().FromInstance(controller).AsSingle();
    }
}