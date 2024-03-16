using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AugustEngine.Visual
{
    /// <summary>
    /// A Local Material build up and tears down a material that can be modified locally
    /// </summary>
    public class LocalMaterial : MonoBehaviour
    {
        [SerializeField] Material material;
        [SerializeField] List<Renderer> meshRenderers;


        private Material _localMat;
        public Material Material
        {
            get
            {
                if (_localMat == null)
                {
                    // Make a new material to work with and modify
                    _localMat = new Material(material);

                    // Apply new Material
                    ApplyToMeshRenderers(_localMat);
                }
                return _localMat;
            }
        }
        private void OnDisable()
        {
            if (_localMat != null)
            {
                // Revert to previous material
                ApplyToMeshRenderers(material);

                Destroy(_localMat);
                _localMat = null;
            }
            
        }


        private void ApplyToMeshRenderers(Material material)
        {
            foreach (Renderer renderer in meshRenderers)
            {
                renderer.material = material;
            }
        }

        public bool Visible
        {
            set
            {
                foreach (Renderer renderer in meshRenderers)
                {
                    renderer.enabled = value;
                }
            }
        }
    }
}
