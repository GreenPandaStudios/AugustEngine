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
            public override bool Equals(object obj)
            {
                if (obj is TileMatrix)
                {
                    return ((TileMatrix)obj).ToString() == this.ToString();
                }
                return false;
            }

            public static TileMatrix TopEdge
            {
                get{
                    return new TileMatrix()
                    {
                        TopLeft = true,
                        TopRight = true,
                        BottomLeft = false,
                        BottomRight = false,
                    };
                }
            }
            public static TileMatrix BottomEdge
            {
                get
                {
                    return new TileMatrix()
                    {
                        TopLeft = false,
                        TopRight = false,
                        BottomLeft = true,
                        BottomRight = true,
                    };
                }
            }
            public static TileMatrix RightEdge
            {
                get
                {
                    return new TileMatrix()
                    {
                        TopLeft = false,
                        TopRight = true,
                        BottomLeft = false,
                        BottomRight = true,
                    };
                }
            }
            public static TileMatrix LeftEdge
            {
                get
                {
                    return new TileMatrix()
                    {
                        TopLeft = true,
                        TopRight = false,
                        BottomLeft = true,
                        BottomRight = false,
                    };
                }
            }
        }
    }
}