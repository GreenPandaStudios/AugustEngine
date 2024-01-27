using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AugustEngine.Input
{
    [CreateAssetMenu(fileName = "Input Channel 1", menuName = "August Engine/Input/Player Join Channel", order = 1)]
    public class PlayerJoinChannel : ScriptableObject
    {
        [SerializeField] GameObject playerPrefab;
        [SerializeField] InputChannel[] playerInputChannels;
        [SerializeField] bool spawnOnJoin = true;

        public int MaxPlayers { get => playerInputChannels.Length; }

        [System.Serializable]
        internal class PlayerData
        {
            public InputChannel inputChannel;
            public PlayerInput PlayerInput;
            
        }

        private List<PlayerData> _players;

        internal void Initialize()
        {
            _players = new List<PlayerData>();
            for (int i = 0; i < MaxPlayers; i++)
            {
                _players.Add(null);
            }
        }

        private void CreatePlayer(int playerIndex)
        {
            if (playerInputChannels[playerIndex] == null)
            {

                Debug.LogWarning($"Tried creating player <{playerIndex}>, which does not yet exist");
                return;
            }

            // Disable Prefab first so we can call "Set input channel" before onenable
            playerPrefab.SetActive(false);
            GameObject _newPlayer = Instantiate(playerPrefab);
            playerPrefab.SetActive(true);

            List<INeedInputChannel> components = new List<INeedInputChannel>();
            _newPlayer.GetComponentsInChildren(components);
            
            foreach (INeedInputChannel component in components)
            {
                component.SetInputChannel(playerInputChannels[playerIndex]);
            }
            _newPlayer.SetActive(true);
            
        }


        internal void PlayerJoined(PlayerInput playerInput)
        {

            // Check if this input hasn't been removed
            if (GetPlayerIndex(playerInput) != -1)
            {
                return;
            }

            int totalJoinedPlayers = 0;
            for (int i = 0; i < _players.Count; i++) totalJoinedPlayers += _players[i] == null ? 0 : 1;

            // Check if we've reached the maximum number of players
            if (totalJoinedPlayers >= MaxPlayers)
            {
                Destroy(playerInput);
                return;
            }

            // Otherwise, remember this player
            int playerIndex = -1;
            while (_players[++playerIndex] != null) { }

            _players[playerIndex] = new PlayerData()
            {
                PlayerInput = playerInput,
                inputChannel = playerInputChannels[playerIndex]
            };

            // Remember this player
            DontDestroyOnLoad(playerInput.gameObject);
            InputBroadcaster.Initialize(playerInputChannels[playerIndex], playerInput);


            if (spawnOnJoin)
            {
                CreatePlayer(playerIndex);
            }
        }

        internal void PlayerLeft(PlayerInput playerInput)
        {
            // Leave all players around for now
        }

        /// <summary>
        /// Returns the player index for a given player input 
        /// </summary>
        /// <param name="playerInput"></param>
        /// <returns>-1 if not found</returns>
        public int GetPlayerIndex(PlayerInput playerInput)
        {
            if (_players == null) return -1;

            for (int i = 0; i < _players.Count; i++)
            {
                if (_players[i]?.PlayerInput == playerInput) return i;
            }
            return -1;
        }
        /// <summary>
        /// Returns the player index for a given player input channel
        /// </summary>
        /// <param name="playerInput"></param>
        /// <returns>-1 if not found</returns>
        public int GetPlayerIndex(InputChannel inputChannel)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                if (_players[i].inputChannel == inputChannel) return i;
            }
            return -1;
        }



        public interface INeedInputChannel {
            /// <summary>
            /// Called by <see cref="CreatePlayer(PlayerInput)"/>
            /// </summary>
            /// <param name="inputChannel"></param>
            public void SetInputChannel(InputChannel inputChannel);
        }
    }
}