using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AugustEngine.Input
{
    /// <summary>
    /// Broadcasts PlayerInputManager events across a <see cref="PlayerJoinChannel"/>
    /// </summary>
    public class PlayerJoinBroadcaster : MonoBehaviour
    {
        [SerializeField] private PlayerJoinChannel channel;
        [SerializeField] GameObject inputPrefab;

        private PlayerInputManager inputManager = null;

        private PlayerInputManager PlayerInputManager
        {
            get
            {
                if (inputManager == null)
                {
                    // Try to get the component
                    if (!gameObject.TryGetComponent<PlayerInputManager>(out inputManager))
                    {
                        // Create component  
                        inputManager = gameObject.AddComponent<PlayerInputManager>();
                    }
                    // Mark to send events
                    inputManager.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
                    inputManager.playerPrefab = inputPrefab;

                    // Register events
                    inputManager.onPlayerJoined += (channel.PlayerJoined);
                    inputManager.onPlayerLeft += (channel.PlayerLeft);
                }
                return inputManager;
            }
            set
            {
                if (value == null)
                {
                    inputManager.onPlayerJoined -= (channel.PlayerJoined);
                    inputManager.onPlayerLeft -= (channel.PlayerLeft);
                }
            }
            
        }

        private void Awake()
        {
            DontDestroyOnLoad(this);
            if (PlayerInputManager != null)
            {
                channel.Initialize();
            }
        }
        private void OnDestroy()
        {
            PlayerInputManager = null;
        }
    }
}