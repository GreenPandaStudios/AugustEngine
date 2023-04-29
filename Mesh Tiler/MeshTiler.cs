using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AugustEngine.MeshTiler {
    public class MeshTiler : MonoBehaviour
    {
        [Tooltip("The colliders we would like to tile over")]
        [SerializeField] List<Collider> TargetColliders;
        [SerializeField] LayerMask PlaceTilesOnWhat;
        [SerializeField] public float GridSize;
        [SerializeField] MeshTileset TileSet;
      
        private Vector2 SnapToGrid(Vector2 position)
        {
            return new Vector2(SnapToGrid(position.x), SnapToGrid(position.y));
        }
        private float SnapToGrid(float val)
        {
            return Mathf.RoundToInt(val / GridSize) * GridSize;
        }


        /// <summary>
        /// Returns the bounding box of all target colliders in the shape
        /// </summary>
        /// <returns>(Min x, Max x, Min y, Max y)</returns>
        private Vector4 GetBounds()
        {
            // Find the bounding box of all the colliders
            Vector4 bounds = new Vector4(Mathf.Infinity, -Mathf.Infinity, Mathf.Infinity, -Mathf.Infinity);
            foreach (Collider collider in TargetColliders)
            {
                bounds.x = Mathf.Min(collider.bounds.min.x, bounds.x);
                bounds.y = Mathf.Max(collider.bounds.max.x, bounds.y);
                bounds.z = Mathf.Min(collider.bounds.min.y, bounds.z);
                bounds.w = Mathf.Max(collider.bounds.max.y, bounds.w);
            }
            Debug.Log(bounds);
            return bounds;
        }

        private void PlaceTiles()
        {
            // Create a new GameObject to store the tiles on
            GameObject tileMesh = new GameObject("TileMesh");
            tileMesh.transform.parent = transform;

            // Starting from the bottom left of the bounding box, create the tiles
            Vector4 bounds = GetBounds();
            
            if (bounds.x > bounds.y)
            {
                // invalid bounds, break out
                return;
            }
            
            Vector2 currentPosition = SnapToGrid(new Vector2(bounds.x, bounds.z));

            // Update our tileset before consuming
            TileSet.UpdateDictionary();

            

            // Horizontal loop
            while (currentPosition.x <= SnapToGrid(bounds.y))
            {
                // Vertical loop
                while (currentPosition.y <= SnapToGrid(bounds.w))
                {
                    // Check this point
                    bool bottomLeft = IsColliding(currentPosition);
                    bool bottomRight = IsColliding(currentPosition + (Vector2.right * GridSize));
                    bool topLeft = IsColliding(currentPosition + (Vector2.up * GridSize));
                    bool topRight = IsColliding(currentPosition + (Vector2.one * GridSize));

                    // Construct the matrix key to pull the tile prefab out
                    MeshTile.TileMatrix key = new MeshTile.TileMatrix()
                    {
                        BottomLeft = bottomLeft,
                        BottomRight = bottomRight,
                        TopLeft = topLeft,
                        TopRight = topRight
                    };
                    Debug.Log(key);
                    if (TileSet.TryGetMesh(key, out var tile))
                    {
                        // if we have a tile to place, place it here
                        GameObject newTile = Instantiate(tile, tileMesh.transform);
                        newTile.transform.position = currentPosition;
                        Debug.Log(tile);
                        // scale it to the grid size
                        newTile.transform.localScale = Vector3.one * GridSize;
                    }

                    // Consider empty tile if nothing is defined

                    // Increment current position
                    currentPosition.y = SnapToGrid(GridSize + currentPosition.y);
                }

                // Increment current position
                currentPosition.x = SnapToGrid(GridSize + currentPosition.x);
                currentPosition.y = SnapToGrid(bounds.z);
            }
        }


        private bool IsColliding(Vector2 gridPosition)
        {
            Ray ray = new Ray(new Vector3(gridPosition.x, gridPosition.y, transform.position.z)
                - Vector3.forward * 100,
                Vector3.forward * 200);

            return UnityEngine.Physics.Raycast(ray, 200, PlaceTilesOnWhat);
        }

#if UNITY_EDITOR
        public bool generateTiles = false;

        //You can have multiple booleans here
        private void OnValidate()
        {
            if (generateTiles)
            {
                // Your function here
                PlaceTiles();

                //When its done set this bool to false
                //This is useful if you want to do some stuff only when clicking this "button"
                generateTiles = false;
            }
        }
#endif
    }
}