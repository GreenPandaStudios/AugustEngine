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
                    if (TileSet.TryGetMesh(key, out var tile))
                    {
                        // if we have a tile to place, place it here
                        GameObject newTile = Instantiate(tile, tileMesh.transform);


                        // Defalate the tile if we need to before placing it
                        CheckAndDeflate(currentPosition, newTile, key);


                        newTile.transform.position = currentPosition;
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

        /// <summary>
        /// Checks if a tileMatrix is an edge and needs deflating
        /// Deflates the mesh if it does
        /// </summary>
        /// <param name="currentPosition">Bottom left point on the Grid</param>
        /// <param name="deflateObject">The game object we will me deflating</param>
        /// <param name="tileMatrix">The tile matrix defining this tile, used to check if it is an edge</param>
        private void CheckAndDeflate(Vector2 currentPosition, GameObject deflateObject, MeshTile.TileMatrix tileMatrix)
        {
            if (tileMatrix.Equals(MeshTile.TileMatrix.BottomEdge))
            {
                // Before doing any math, see if we can find a mesh to deform
                MeshFilter mesh = GetMeshFilter(deflateObject);
                
                // Break out if not found
                if (mesh == null)
                {
                    return;
                }


                // We will be delfating downward
                Vector3 deflateDirection = Vector3.down;
                // The source point will be the bottom of the tile, which is the current poisition's y value
                Vector3 sourcePoint = new Vector3(0, currentPosition.y, 0);

                // The deflate amount will be taken from the middle top of this grid position downward
                 if  (GetDeflateAmount(new Vector3(currentPosition.x + GridSize / 2,
                    currentPosition.y + GridSize,
                    0),
                   deflateDirection, out float deflateAmount))
                {                  
                    // We are ready to delfate
                    DeflateMesh(mesh,deflateDirection, sourcePoint, deflateAmount);
                }
                else 
                {
                    // log a warning and exit
                    Debug.LogError($"We could not delfate edge mesh: <{tileMatrix}>. Please check that your colliders interesect with the z plane");
                    return;
                }
            }
            else if (tileMatrix.Equals(MeshTile.TileMatrix.TopEdge))
            {
                // Before doing any math, see if we can find a mesh to deform
                MeshFilter mesh = GetMeshFilter(deflateObject);

                // Break out if not found
                if (mesh == null)
                {
                    return;
                }


                // We will be delfating upward
                Vector3 deflateDirection = Vector3.up;

                // The source point will be the top of the tile, which is the current poisition's y value + grid size
                Vector3 sourcePoint = new Vector3(0, currentPosition.y, 0);

                // The deflate amount will be taken from the middle bottom of this grid position upward
                if  (GetDeflateAmount(new Vector3(currentPosition.x + GridSize / 2,
                    currentPosition.y,
                    0),
                   deflateDirection, out float deflateAmount))
                {
                    // We are ready to delfate
                    DeflateMesh(mesh, deflateDirection, sourcePoint, deflateAmount);
                }
                else
                {
                    // log a warning and exit
                    Debug.LogError($"We could not delfate edge mesh: <{tileMatrix}>. Please check that your colliders interesect with the z plane");
                    return;
                }
            }
            else if (tileMatrix.Equals(MeshTile.TileMatrix.RightEdge))
            {
                // Before doing any math, see if we can find a mesh to deform
                MeshFilter mesh = GetMeshFilter(deflateObject);

                // Break out if not found
                if (mesh == null)
                {
                    return;
                }


                // We will be delfating right
                Vector3 deflateDirection = Vector3.right;

                // The source point will be the right of the tile, which is the current poisition's x value + grid size
                Vector3 sourcePoint = new Vector3(currentPosition.x + GridSize, 0, 0);

                // The deflate amount will be taken from the middle left of this grid right upward
                if (GetDeflateAmount(new Vector3(
                                    currentPosition.x,
                                    currentPosition.y + GridSize/2,
                                    0),
                   deflateDirection, out float deflateAmount))
                {
                    // We are ready to delfate
                    DeflateMesh(mesh, deflateDirection, sourcePoint, deflateAmount);
                }
                else
                {
                    // log a warning and exit
                    Debug.LogError($"We could not delfate edge mesh: <{tileMatrix}>. Please check that your colliders interesect with the z plane");
                    return;
                }
            }
            else if (tileMatrix.Equals(MeshTile.TileMatrix.LeftEdge))
            {
                // Before doing any math, see if we can find a mesh to deform
                MeshFilter mesh = GetMeshFilter(deflateObject);

                // Break out if not found
                if (mesh == null)
                {
                    return;
                }


                // We will be delfating left
                Vector3 deflateDirection = Vector3.left;

                // The source point will be the left of the tile, which is the current poisition's x value
                Vector3 sourcePoint = new Vector3(currentPosition.x, 0, 0);

                // The deflate amount will be taken from the middle right of this grid right upward
                if (GetDeflateAmount(new Vector3(
                                    currentPosition.x + GridSize,
                                    currentPosition.y + GridSize / 2,
                                    0),
                   deflateDirection, out float deflateAmount))
                {
                    // We are ready to delfate
                    DeflateMesh(mesh, deflateDirection, sourcePoint, deflateAmount);
                }
                else
                {
                    // log a warning and exit
                    Debug.LogError($"We could not delfate edge mesh: <{tileMatrix}>. Please check that your colliders interesect with the z plane");
                    return;
                }
            }
        }

        private MeshFilter GetMeshFilter(GameObject gameObject)
        {
            MeshFilter mesh;
            mesh = gameObject.GetComponent<MeshFilter>();

            // look in the children if not found
            if (mesh == null)
            {
                mesh = gameObject.GetComponentInChildren<MeshFilter>();
            }
            return mesh;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh">The mesh to deflate</param>
        /// <param name="deflateDirection">The direction we are delfating</param>
        /// <param name="sourcePoint">The point on the mesh</param>
        /// <param name="deflateAmount">How much we are deflating the mesh</param>
        private void DeflateMesh(MeshFilter meshFilter, Vector3 deflateDirection, Vector3 sourcePoint, float deflateAmount)
        {

            //We need to remember the localPosition offset so the deflate math works, then we can put it back
            var lclPosOff = meshFilter.transform.localPosition;
            meshFilter.transform.localPosition = Vector3.zero;



            // Copy over the mesh data
            // Duplicate the mesh
            Mesh mesh = new();
            Mesh originalMesh = meshFilter.sharedMesh;
            mesh.name = "DeformedMesh";
            mesh.vertices = originalMesh.vertices;
            mesh.triangles = originalMesh.triangles;
            mesh.normals = originalMesh.normals;
            mesh.uv = originalMesh.uv;

            // Project the source point onto the deflate direction
            // So we can get a distance accourding to the source direction
            sourcePoint = Vector3.Project(meshFilter.transform.TransformPoint(sourcePoint), deflateDirection);
            deflateDirection = deflateDirection.normalized;

            // loop through all the verticies of the mesh and defalte it the amount in the direction
            // Linearly taper off the delfate according to the distance from the source point

            Vector3[] verts = mesh.vertices;
            
            for (int i = 0; i < verts.Length; i++)
            {
                var _vert = meshFilter.transform.TransformPoint(verts[i]);
                float weight = Mathf.Clamp(
                    (Vector3.Distance(Vector3.Project(_vert, deflateDirection),
                    sourcePoint))
                    ,0f
                    ,1f) / GridSize;

                var offsetScalar = weight * deflateAmount;

                verts[i] += offsetScalar * deflateDirection;
                verts[i] = meshFilter.transform.InverseTransformPoint(verts[i]);
            }
            mesh.SetVertices(verts);
            mesh.RecalculateNormals();

            meshFilter.sharedMesh = mesh;

            meshFilter.transform.localPosition = lclPosOff;
        }



        private bool GetDeflateAmount(Vector3 delfateSourcePoint, Vector3 deflateDirection, out float deflateAmount)
        {
            if (UnityEngine.Physics.Raycast(
                   delfateSourcePoint,
                   deflateDirection,
                    out var info,
                    GridSize,
                    PlaceTilesOnWhat,
                    QueryTriggerInteraction.UseGlobal

                    ))
            {
                deflateAmount = info.distance;
                return true;
            }
            deflateAmount = 0f;
            return false;
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