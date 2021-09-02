using UnityEngine;
using System.Collections;
using Zenject;

public class MagicMeter : MonoBehaviour, IMagicMeter
{
    public ValueBar bar;
    BoundedValue<float> value;
    public MagicMeterSettings settings;
    public float Value { get => value.Value; set => this.value.Value = value; }
    public ValueBarFlash flash;

    public bool decreaseMana = true;

    private void Start()
    {
        value = new BoundedValue<float>(0, 0, settings.maxMana);
        if (bar != null) bar.Construct(value);

        GameObject[] battlers = GameObject.FindGameObjectsWithTag("Battler");
        for (int i = 0; i < battlers.Length; i++)
        {
            Battler b = battlers[i].GetComponent<Battler>();
            b.eventSet.onBattlerHit += OnBattlerHit;
        }
    }

    public bool CheckMana(float mana)
    {
        bool enough = Value >= mana;
        if (!enough) flash.Flash();
        return enough;
    }

    public void OnBattlerHit(IBattler battler)
    {
        value.Value += settings.incrementBy;
    }

    public void DecreaseMana(float mana)
    {
        Value -= mana;
    }

    private void Update()
    {
        if (decreaseMana) value.Value -= Time.deltaTime * settings.decreasePerSecond;
    }

}
