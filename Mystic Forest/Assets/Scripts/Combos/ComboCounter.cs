using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ComboCounter : MonoBehaviour, IComboCounter
{
    HashSet<IBattler> inHitStun;
    int comboCount;
    public Action<int> onCountIncremented { get; set; }
    public Action onComboFinished { get; set; }

    public void Awake()
    {
        inHitStun = new HashSet<IBattler>();
    }

    public void Start()
    {
        SetUpEvents(AddComboEvents);
    }

    public void OnDestroy()
    {
        SetUpEvents(RemoveComboEvents);
    }

    public void OnBattlerHit(IBattler battler)
    {
        inHitStun.Add(battler);
        comboCount++;
        onCountIncremented?.Invoke(comboCount);
    }

    public void OnBattlerRecovered(IBattler battler)
    {
        inHitStun.Remove(battler);
        if (inHitStun.Count <= 0)
        {
            onComboFinished?.Invoke();
            comboCount = 0;
        }
    }

    void SetUpEvents(Action<IBattler> setupFunction)
    {
        GameObject[] battlers = GameObject.FindGameObjectsWithTag("Battler");
        for (int i = 0; i < battlers.Length; i++)
        {
            Battler b = battlers[i].GetComponent<Battler>();
            setupFunction(b);
        }
    }

    void AddComboEvents(IBattler battler)
    {
        battler.eventSet.onBattlerHit += OnBattlerHit;
        battler.eventSet.onBattlerRecovered += OnBattlerRecovered;
    }

    void RemoveComboEvents(IBattler battler)
    {
        battler.eventSet.onBattlerHit -= OnBattlerHit;
        battler.eventSet.onBattlerRecovered -= OnBattlerRecovered;
    }


}
