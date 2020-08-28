using UnityEngine;
using System.Collections;
using Zenject;

public class MagicMeter : MonoBehaviour
{
    public ValueBar bar;
    BoundedValue<float> value;
    public MagicMeterSettings settings;

    private void Awake()
    {
        value = new BoundedValue<float>(0, 0, settings.maxMana);
        bar.Construct(value);

        GameObject[] battlers = GameObject.FindGameObjectsWithTag("Battler");
        for (int i = 0; i < battlers.Length; i++)
        {
            Battler b = battlers[i].GetComponent<Battler>();
            b.eventSet.onBattlerHit += (battler) => value.Value += settings.incrementBy;
        }
    }

    private void Update()
    {
        value.Value -= Time.deltaTime * settings.decreasePerSecond;
    }

}
