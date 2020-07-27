using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;

namespace Tests
{
    public class PlayerSwitcherTests
    {
        GameObject go;
        PlayerSwitcher switcher;
        List<IPlayer> players;
        IPlayer activePlayer;
        IUnityInputService service;

        public List<IPlayer> CreatePlayers(int numPlayers)
        {
            List<IPlayer> players = new List<IPlayer>();
            for (int i = 0; i < numPlayers; i++)
            {
                players.Add(Substitute.For<IPlayer>());
            }
            return players;
        }

        public void SetUpPlayerSwitcher(int numPlayers)
        {
            // gameobject for switcher
            go = new GameObject();
            switcher = go.AddComponent<PlayerSwitcher>();
            // prep service for input
            service = Substitute.For<IUnityInputService>();
            // set players
            players = CreatePlayers(numPlayers);
            switcher.Construct(players, service);
            // connect event to player
            switcher.onPlayerSwitched += (player) => activePlayer = player;
            // set up events
            switcher.Start();
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(go);
        }

        // active player is first player
        [Test]
        public void Start_FirstPlayerIsActivePlayer()
        {
            SetUpPlayerSwitcher(2);
            Assert.AreSame(players[0], activePlayer);
        }

        // switches player on correct input (2 players, 1 active, 2 selected, swaps to 2)
        [Test]
        public void SwitchPlayer_CorrectInput_ChangesActivePlayer()
        {
            SetUpPlayerSwitcher(2);
            IPlayer secondPlayer = players[1];

            IPlayer selected = null;
            service.GetKeyDown(KeyCode.Alpha2).Returns(true);
            switcher.onPlayerSwitched += (player) => selected = player;
            switcher.Update();
            Assert.AreSame(selected, players[1]);
        }

        // switch to already active player does not trigger on player switched event
        [Test]
        public void SwitchPlayer_SamePlayerSelected_DoesntTriggerSwitchEvent()
        {
            SetUpPlayerSwitcher(2);

            service.GetKeyDown(KeyCode.Alpha1).Returns(true);

            bool eventFired = false;
            switcher.onPlayerSwitched += (player) => eventFired = true;
            switcher.Update();

            Assert.False(eventFired);
        }

        // does not switch out of range (1 player, 2 pressed, nothing happens)
        [Test]
        public void SwitchPlayer_SwitchOutOfRange_DoesNotSwitchPlayer()
        {
            SetUpPlayerSwitcher(1);
            service.GetKeyDown(KeyCode.Alpha2).Returns(true);

            Assert.DoesNotThrow(() => switcher.Update());
        }

        // does not switch if active player is attacking
        [Test]
        public void SwitchPlayer_Attacking_DoesNotSwitchPlayer()
        {
            // player is attacking
            SetUpPlayerSwitcher(2);
            IExecutionState state = Substitute.For<IExecutionState>();
            state.combatState.Returns(CombatState.ATTACKING);
            players[0].executionState.Returns(state);
            // set up check for if the event fired
            bool eventFired = false;
            switcher.onPlayerSwitched += (player) => eventFired = true;
            // press key down
            service.GetKeyDown(KeyCode.Alpha2).Returns(true);
            switcher.Update();
            Assert.False(eventFired);
        }

    }
}
