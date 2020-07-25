using UnityEngine;
using UnityEditor;
using Zenject;
using System.Collections.Generic;

public class StaminaControllerInstaller : ScriptableObjectInstaller
{ 
    public override void InstallBindings()
    {
        Container.Bind<IStaminaController>().FromComponentInChildren().AsSingle();
    }
}