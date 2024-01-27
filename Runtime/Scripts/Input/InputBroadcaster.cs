using UnityEngine;
using UnityEngine.InputSystem;

namespace AugustEngine.Input
{
    public class InputBroadcaster : MonoBehaviour
    {
        [SerializeField] InputChannel channel;
        [SerializeField] PlayerInput playerInput;

        // Start is called before the first frame update
        void Awake()
        {
            channel.ConstructEvents(playerInput.currentActionMap);
        }

        /// <summary>
        /// Componentless constructor
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="playerInput"></param>
        public static void Initialize(InputChannel channel, PlayerInput playerInput)
        {
            channel.ConstructEvents(playerInput.currentActionMap);
        }
    }
}