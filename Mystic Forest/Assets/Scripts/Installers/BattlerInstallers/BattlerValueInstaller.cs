using UnityEngine;
using System.Collections;
using Zenject;

[CreateAssetMenu]
public class BattlerValueInstaller : ScriptableObjectInstaller
{
    public BoundedFloat stamina;

    public override void InstallBindings()
    {
        BoundedFloat stamina = new BoundedFloat(this.stamina.Value, this.stamina.MinValue, this.stamina.MaxValue);
        Container.Bind<BoundedValue<float>>().FromInstance(stamina).AsSingle().When(x => x.MemberName == "stamina");
    }
}
