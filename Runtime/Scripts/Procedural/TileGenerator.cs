
namespace AugustEngine.Procedural
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using UnityEngine;
    using Unity.Jobs;
    using Unity.Collections;

    public class TileGenerator : MonoBehaviour
    {
        /// <summary>
        /// An event that will cause all enabled tiles to recalculate
        /// </summary>
        public static Action RequestUpdateTile;

        [SerializeField] MeshFilter meshFilter;
        [SerializeField] MeshCollider meshCollider;
        [SerializeField] Transform origin;
        
        [SerializeField] NoiseGeneration.NoiseGenerator heightMap;
        
        
        /// <summary>
        /// Schedules a job to update the tile
        /// </summary>
        public void UpdateVerts()
        {
            
            Vector3[] verts = meshFilter.mesh.vertices;
            Vector2[] temps = new Vector2[verts.Length];

            NativeArray<Vector3> native_verts = new NativeArray<Vector3>(verts.Length, Allocator.TempJob);
            NativeArray<Vector2> native_locations = new NativeArray<Vector2>(verts.Length, Allocator.TempJob);
            NativeArray<float> native_temps = new NativeArray<float>(verts.Length, Allocator.TempJob);

            
            for (int i = 0; i < verts.Length; i++)
            {
                native_locations[i] =
                    new Vector2(origin.TransformPoint(verts[i]).x
                   , origin.TransformPoint(verts[i]).z
                   );

                native_verts[i] = verts[i];

            }

            UpdateTileJob heightMapJob = new UpdateTileJob();

            heightMapJob.verts = native_verts;
            heightMapJob.locations = native_locations;
            heightMapJob.temps = native_temps;

            heightMapJob.heightNoiseData = heightMap.data;
            heightMapJob.tempNoiseData = heightMap.data;
            heightMapJob.tempNoiseData.scale /= 100;
            heightMapJob.tempNoiseData.heightScale = 1f;

            JobHandle heightHandler = heightMapJob.Schedule();

            heightHandler.Complete();

            for (int i = 0; i < verts.Length; i++)
            {
                verts[i] = native_verts[i];
                temps[i] = new Vector2(0, native_temps[i]);
            }
            //dispose mem
            native_verts.Dispose();
            native_locations.Dispose();
            native_temps.Dispose();


            meshFilter.mesh.vertices = verts;
            meshFilter.mesh.uv2 = temps;
            meshFilter.mesh.RecalculateBounds();
            meshFilter.mesh.RecalculateNormals();
            if (meshCollider)
            {
                meshCollider.cookingOptions =
                    MeshColliderCookingOptions.WeldColocatedVertices
                    |
                    MeshColliderCookingOptions.EnableMeshCleaning;

                meshCollider.sharedMesh = meshFilter.mesh;
                meshCollider.sharedMesh.MarkDynamic();
                meshCollider.sharedMesh.Optimize();
            }
        }




        /// <summary>
        /// Job struct used for scheduling a tile update
        /// </summary>
        public struct UpdateTileJob : IJob
        {
        
            /// <summary>
            /// The locations to sample a point in the noise generators
            /// </summary>
            public NativeArray<Vector2> locations;
            /// <summary>
            /// The actual vertex data
            /// </summary>
            public NativeArray<Vector3> verts;
            /// <summary>
            /// Temperature data
            /// </summary>
            public NativeArray<float> temps;
            public NoiseGeneration.NoiseData heightNoiseData;
            public NoiseGeneration.NoiseData tempNoiseData;
            public void Execute()
            {
                //for each vertex in the mesh
                for (int i = 0; i < verts.Length; i++)
                {
                    //set the height of the vertex based on the provided location point
                    verts[i] = new Vector3(
                        verts[i].x, 
                        HeightMapNoise.EvaluatePoint(
                            heightNoiseData,
                            locations[i].x,
                            locations[i].y),
                        verts[i].z);
                    
                    //set the temperature based on the location data
                    temps[i] = HeightMapNoise.EvaluatePoint(tempNoiseData, locations[i].x, locations[i].y) + 
                        (float) InfiniteWorldTransform.GlobalOffset.y;
                }
            }
        }


        private void OnEnable()
        {
            //update verts when enabled
            UpdateVerts();
            //listen for calls to update this tile
            RequestUpdateTile += UpdateVerts;
        }
        private void OnDisable()
        {
            RequestUpdateTile -= UpdateVerts;
        }
    }
}