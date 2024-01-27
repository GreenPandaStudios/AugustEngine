using AugustEngine.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AugustEngine.Characters
{

    /// <summary>
    /// Preserves an animation hierarchy between enabling and disabling
    /// </summary>
    public class PreserveHierarchy : MonoBehaviour
    {
        private struct TransformData
        {
            public Vector3 position;
            public Quaternion rotation;
        }
        private List<TransformData> transformDatas = new List<TransformData>();
        private int currentPointer = 0;


        private void Awake()
        {
            RememberData(transform);
        }
        private void OnDisable()
        {
            currentPointer = 0;
            SetData(transform);
        }
        private void RememberData(Transform transform)
        {
            transformDatas.Add(new TransformData { position = transform.localPosition, rotation = transform.localRotation });
            for (int i = 0; i < transform.childCount; i++)
            {
                RememberData(transform.GetChild(i));
            }
        }

        private void SetData(Transform tr) {
            TransformData data = transformDatas[currentPointer++];
            tr.localPosition = data.position;
            tr.localRotation = data.rotation;
            for (int i = 0; i < tr.childCount; i++)
            {
                SetData(tr.GetChild(i));
            }
        }
    }

}
