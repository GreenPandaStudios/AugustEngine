using UnityEngine;
using System.Collections.Generic;
namespace AugustEngine.MeshTiler
{


    [CreateAssetMenu(fileName = "Mesh Tileset", menuName = "August Engine/Mesh Tiler/Mesh Tileset", order = 1)]
    public class MeshTileset : ScriptableObject
    {
        [SerializeField] List<MeshTile> Tiles;
        private Dictionary<string, MeshTile> tileLookup;

        /// <summary>
        /// Retrieves the mesh from the tileset given the matrix key
        /// returns true if found, false if not found. <see cref="UpdateDictionary"/> should be called before using this
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public bool TryGetMesh(MeshTile.TileMatrix matrix, out GameObject tile)
        {
            tile = null;
            if (tileLookup == null)
            {           
                return false;
            }
            if (tileLookup.TryGetValue(matrix.ToString(), out var m)) tile = m.TilePrefab; 
            return tile ? true : false;
        }

        // This should be called by consumers before trying to pull out meshes
        public void UpdateDictionary()
        {
            // Construct the tile dictionary
            tileLookup = new Dictionary<string, MeshTile>();
            foreach (MeshTile tile in Tiles)
            {
                tileLookup.Add(tile.MatrixKey.ToString(), tile);
            }
        }
    }
}