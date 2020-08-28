using UnityEngine;
using System.Collections;
using Zenject;
using System.Collections.Generic;

public class PlayerInfoManager : MonoBehaviour
{
    public GameObject playerInfoPrefab;
    readonly int maxControllers = 4;
    public float scaling;
    [Range(0, 1)] public float yPos;
    private Dictionary<IPlayer, PlayerInfo> infos;
    private PlayerSwitcher switcher;

    [Inject]
    public void Construct(PlayerSwitcher switcher)
    {
        this.switcher = switcher;
    }

    private void Awake()
    {
        GameObjectContext[] contexts = FindObjectsOfType<GameObjectContext>();
        int numberPlayerInfoOnScreen = 0;
        infos = new Dictionary<IPlayer, PlayerInfo>();
        for (int i = 0; i < contexts.Length; i++)
        {
            IPlayer player = contexts[i].gameObject.GetComponent<IPlayer>();
            if (player != null && numberPlayerInfoOnScreen < maxControllers)
            {
                GameObject playerInfoGameObject = contexts[i].Container.InstantiatePrefab(playerInfoPrefab);
                infos.Add(player, playerInfoGameObject.GetComponent<PlayerInfo>());
                PlacePlayerInfo(playerInfoGameObject, numberPlayerInfoOnScreen);
                numberPlayerInfoOnScreen++;
            }
        }

        switcher.onPlayerSwitched += HighlightActive;
    }

    private void OnDestroy()
    {
        switcher.onPlayerSwitched -= HighlightActive;
    }

    void HighlightActive(IPlayer player)
    {
        foreach (var item in infos)
        {
            item.Value.SetActive(item.Key == player);
        }
    }

    void PlacePlayerInfo(GameObject playerInfoGameObject, int numOfPlayerInfoOnScreen)
    {
        playerInfoGameObject.transform.SetParent(transform);
        ViewportPosition vp = playerInfoGameObject.AddComponent<ViewportPosition>();
        vp.xPos = (((float)numOfPlayerInfoOnScreen * 2) + 1) / ((float)maxControllers * 2);
        vp.yPos = yPos;
        vp.transform.localScale = new Vector3(scaling, scaling, scaling);
    }
}
