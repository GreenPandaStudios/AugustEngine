using UnityEngine;
using UnityEngine.InputSystem;

namespace AugustEngine.Input
{
    public class InputBroadcaster : MonoBehaviour
    {
        [SerializeField] InputChannel channel;
        [SerializeField] PlayerInput playerInput;

        // Start is called before the first frame update
        void OnEnable()
        {
            channel.ConstructEvents(playerInput.currentActionMap);
        }
    }
}