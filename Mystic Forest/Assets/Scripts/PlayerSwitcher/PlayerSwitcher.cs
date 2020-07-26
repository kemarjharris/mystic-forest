using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zenject;

public class PlayerSwitcher : MonoBehaviour
{
    List<IPlayer> players;
    IPlayer activePlayer;
    KeyCode[] keyCodes;
    public System.Action<IPlayer> onPlayerSwitched;

    [Inject]
    public void Construct(List<IPlayer> players)
    {
        this.players = players;
    }

    public void Start()
    {
        if (players.Count > 0)
        {
            activePlayer = players[0];
            NotifyPlayers(activePlayer);
        }
    }

    private void Awake()
    {
        keyCodes = new KeyCode[] {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9,
        };
    }

    public void Update()
    {
        int numberPressed = NumberPressed();
        
        if (numberPressed > 0)
        {
            int listPosition = numberPressed - 1;
            if (listPosition < players.Count && activePlayer.executionState.combatState == CombatState.NOT_ATTACKING)
            {
                IPlayer selectedPlayer = players[listPosition];
                if (selectedPlayer != activePlayer)
                {
                        activePlayer = selectedPlayer;
                        NotifyPlayers(selectedPlayer);
                    
                }
            }
        }
    }

    public int NumberPressed()
    {
        for(int i = 0; i < keyCodes.Length; i ++ ){
            if (Input.GetKeyDown(keyCodes[i]))
            {
                int numberPressed = i + 1;
                return numberPressed;
            }
        }
        return 0;
    }

    public void NotifyPlayers(IPlayer switchedInPlayer)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (switchedInPlayer == players[i])
            {
                players[i].eventSet.onPlayerSwitchedIn?.Invoke();
                onPlayerSwitched?.Invoke(switchedInPlayer);
            } else
            {
                players[i].eventSet.onPlayerSwitchedOut?.Invoke();
            }
        }
    }
}
