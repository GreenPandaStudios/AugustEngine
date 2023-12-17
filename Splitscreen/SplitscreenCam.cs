using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AugustEngine.Splitscreen
{

    /// <summary>
    /// Automatically handles the adding and removing of cameras up to 4 players
    /// for split screen
    /// </summary>
    [RequireComponent(typeof(Camera))]

    public class SplitscreenCam : MonoBehaviour
    {
        private static List<SplitscreenCam> cameras = new List<SplitscreenCam>();
        private Camera mainCamera;
        /// <summary>
        /// The camera controlled by this SplitscreenCamera
        /// </summary>
        public Camera Camera { get {
                if (mainCamera == null)
                {
                    mainCamera = GetComponent<Camera>();
                }
                return mainCamera;
               }
        }
        private void Awake()
        {
            Camera.enabled = false;
        }
        private void OnEnable()
        {
            //add the camera to the list          
            AddCamera(this);
        }
        private void OnDisable()
        {
            //remove the camera from the list
            RemoveCamera(this);
        }

        private static void AddCamera(SplitscreenCam camera)
        {
            //do not add if there are more than 4 cameras
            if (cameras.Count >= 4)
            {
                Debug.LogWarning("There are 4 or more cameras present already. Cannot add another");
                return;
            }
            cameras.Add(camera);
            camera.Camera.enabled = true;
            RecalculateViewports();
        }
        private static void RemoveCamera(SplitscreenCam camera)
        {
            cameras.Remove(camera);
            camera.Camera.enabled = false;
            RecalculateViewports();
        }
        private static void RecalculateViewports()
        {
            int count = cameras.Count;
            if (count == 0) return;
            count = count > 4 ? 4 : count;
            switch (count)
            {
                case 1:
                    cameras[0].Camera.rect = new Rect(0, 0, 1, 1);
                    break;
                case 2:
                    cameras[0].Camera.rect = new Rect(0, 0, 1, 0.5f);
                    cameras[1].Camera.rect = new Rect(0, 0.5f, 1, 0.5f);
                    break;
                default:
                    cameras[0].Camera.rect = new Rect(0, 0, 0.5f, 0.5f);             
                    cameras[1].Camera.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                    cameras[2].Camera.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                    if (count <= 3) break;
                    cameras[3].Camera.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                    break;
            }
        }
    }
}
