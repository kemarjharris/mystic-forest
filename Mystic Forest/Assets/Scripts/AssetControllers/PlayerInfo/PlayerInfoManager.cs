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
        Canvas canvas = GameObject.FindGameObjectWithTag("Main Canvas").GetComponent<Canvas>();
        int numberPlayerInfoOnScreen = 0;
        infos = new Dictionary<IPlayer, PlayerInfo>();
        for (int i = 0; i < contexts.Length; i++)
        {
            IPlayer player = contexts[i].gameObject.GetComponent<IPlayer>();
            if (player != null && numberPlayerInfoOnScreen < maxControllers)
            {
                GameObject playerInfoGameObject = contexts[i].Container.InstantiatePrefab(playerInfoPrefab);
                PlacePlayerInfo(playerInfoGameObject, numberPlayerInfoOnScreen, canvas);
                infos.Add(player, playerInfoGameObject.GetComponent<PlayerInfo>());
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

    void PlacePlayerInfo(GameObject playerInfoGameObject, int numOfPlayerInfoOnScreen, Canvas canvas)
    {
        Transform transform = playerInfoGameObject.transform;
        transform.SetParent(canvas.transform, false);
        CanvasPosition cp = playerInfoGameObject.AddComponent<CanvasPosition>();
        cp.SetCanvas(canvas);
        cp.xPos = (((float)numOfPlayerInfoOnScreen * 2) + 1) / ((float)maxControllers * 2);
        cp.yPos = yPos;
        cp.transform.localScale = new Vector3(scaling, scaling, scaling);
    }
}
