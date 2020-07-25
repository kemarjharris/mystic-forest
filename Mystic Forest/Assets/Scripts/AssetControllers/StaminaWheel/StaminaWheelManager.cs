using UnityEngine;
using System.Collections;
using Zenject;
using System.Collections.Generic;

public class StaminaWheelManager : MonoBehaviour
{
    public GameObject staminaWheelPrefab;
    public float scaling;
    PlayerSwitcher switcher;
    Dictionary<IPlayer, StaminaWheel> wheels;

    [Inject]
    public void Construct(PlayerSwitcher switcher)
    {
        this.switcher = switcher;
    }

    private void Awake()
    {
        GameObjectContext[] contexts = FindObjectsOfType<GameObjectContext>();
        wheels = new Dictionary<IPlayer, StaminaWheel>();
        for (int i = 0; i < contexts.Length; i++)
        {
            IPlayer player = contexts[i].gameObject.GetComponent<IPlayer>();
            if (player != null)
            {
                GameObject staminaWheel = contexts[i].Container.InstantiatePrefab(staminaWheelPrefab);
                wheels.Add(player, staminaWheel.GetComponent<StaminaWheel>());
                PlaceStaminaWheel(player.transform, staminaWheel.transform);
            }
        }

        switcher.onPlayerSwitched += ActivateWheel;
    }

    private void OnDestroy()
    {
        switcher.onPlayerSwitched -= ActivateWheel;
    }

    void ActivateWheel(IPlayer player)
    {
        foreach (var item in wheels)
        {
            item.Value.gameObject.SetActive(item.Key == player);
        }
    }

    void PlaceStaminaWheel(Transform playerTransform, Transform staminaWheelTransform)
    {
        staminaWheelTransform.localScale = new Vector3(scaling, scaling, scaling);
        staminaWheelTransform.SetParent(transform);
    }
}
