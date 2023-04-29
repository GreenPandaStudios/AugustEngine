using UnityEngine;
namespace AugustEngine.MeshTiler
{


    [CreateAssetMenu(fileName = "Mesh Tile", menuName = "August Engine/Mesh Tiler/Mesh Tile", order = 1)]
    public class MeshTile : ScriptableObject
    {
        [SerializeField] public GameObject TilePrefab;
        [SerializeField] public TileMatrix MatrixKey;
        [System.Serializable]
        public class TileMatrix
        {
            public bool TopLeft;
            public bool TopRight;
            public bool BottomLeft;
            public bool BottomRight;

            public override string ToString()
            {
                return new Vector4(TopLeft ? 1 : 0,
                                    TopRight ? 1 : 0,
                                    BottomLeft ? 1 : 0,
                                     BottomRight ? 1 : 0
                                   ).ToString();
            }
        }
    }
}