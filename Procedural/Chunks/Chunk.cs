namespace AugustEngine.Procedural.Chunks
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Unity.Jobs;
    using Unity.Collections;
    public class Chunk : MonoBehaviour
    {
        public enum ChunkState
        {
            Generating,
            Destroying,
            Ready,
            Disabled
        }
        public Action<Chunk, ChunkState> OnStateChange;
        private Vector2Int chunkPosition;
        public Vector2Int ChunkPosition { get => chunkPosition; private set { chunkPosition = value; } }

        private ChunkState state = ChunkState.Disabled;
        public ChunkState State { get => state;
            private set
            {
                state = value;
                OnStateChange?.Invoke(this, value);
            }
        }
        public bool generatePhysics = true;

        private MeshFilter filter;
        public MeshFilter Filter { get => filter; }
        private MeshRenderer meshRenderer;
        public MeshRenderer Renderer { get => meshRenderer; }
        private MeshCollider meshCollider;
        public Material material;
        private void OnEnable()
        {

            State = ChunkState.Ready;
        }

        /// <summary>
        /// Schedules a job for a chunk to generate,
        /// returns true if the job was scheduled succesfully, false otherwise
        /// <see cref="State"/> changing to ready when job is finished
        /// </summary>
        /// <returns></returns>
        public bool Generate(GenerateChunkJob chunkJob)
        {
            if (State != ChunkState.Ready)
            {
                return false;
            }

            state = ChunkState.Generating;
            
            //add a mesh filter, renderer, and collider
            if (!filter) 
                filter = gameObject.AddComponent<MeshFilter>();
            
            if (!meshRenderer)
                meshRenderer = gameObject.AddComponent<MeshRenderer>();
                
            
            meshRenderer.sharedMaterial = material;

            if (generatePhysics && !meshCollider) 
                meshCollider = gameObject.AddComponent<MeshCollider>();
            if (!generatePhysics && meshCollider)
                Destroy(meshCollider);

            //set the chunk position based on the job
            ChunkPosition = chunkJob.ChunkPos;
            

            //schedule a job to generate the chunk
            StartCoroutine(GenerateCoroutine(chunkJob));
            
            return true;
        }

        IEnumerator GenerateCoroutine(GenerateChunkJob chunkJob)
        {
            var width = (int)chunkJob.ppc + 1;

            //allocate space for the vertices
            NativeArray<Vector3> verts = new NativeArray<Vector3>((width) * (width), Allocator.Persistent);
            chunkJob.verts = verts;
            chunkJob._pos = transform.position;

            var handler = chunkJob.Schedule();

            //wait for the job to finish, and mark as ready when done
            while (!handler.IsCompleted) yield return null;
            handler.Complete();

            

            var _m = new Mesh();
            _m.SetVertices(verts);
            int[] tris = new int[6 * (verts.Length - 2 * (width - 1)-1)];

            verts.Dispose();
            int j = 0;
            int i = 0;
            while (j < tris.Length)
            {
                if ((i+1) % width != 0)
                {
                    tris[j++] = (i);
                    tris[j++] = (i + 1);
                    tris[j++] = (i + width);


                    tris[j++] = (i + 1);
                    tris[j++] = (i + 1 + width);
                    tris[j++] = (i + width);
                }
                
                i++;
            }


            _m.triangles = tris;
            filter.mesh = _m;

            //wait before recalculating normals

            filter.mesh.RecalculateNormals();

            if (generatePhysics)
            {
                meshCollider.sharedMesh = filter.mesh;
            }
            



            State = ChunkState.Ready;
        }

        public bool MarkForDestroy()
        {
            Debug.Log(this.gameObject.name + " marking for destroy, current state is " + State);
            if (State != ChunkState.Ready)
            {
                return false;
            }
            State = ChunkState.Destroying;
            return true;
        }

        /// <summary>
        /// Marks this chunk is inactive and disables the gameobject
        /// </summary>
        public void Deactivate()
        {
            gameObject.SetActive(false);
            State = ChunkState.Disabled;
        }
        public struct GenerateChunkJob : IJob
        {
            public Vector2Int ChunkPos;
            public Vector3 _pos;
            public int ppc;
            public int upc;

            int width;

            /// <summary>
            /// The actual vertex data
            /// </summary>
            public NativeArray<Vector3> verts;
            /// <summary>
            /// Temperature data
            /// </summary>
           // public NativeArray<float> temps;
            public NoiseGeneration.NoiseData heightNoiseData;
            public NoiseGeneration.NoiseData tempNoiseData;
            public void Execute()
            {

                width = ppc + 1;
                //calculate up front because GlobalOffset May Change

                var _offset = new Vector2(
                    (ChunkPos.x * upc) - _pos.x,
                    (ChunkPos.y * upc) - _pos.z
                );

                //for each vertex in the mesh
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < width; y++)
                    {
                        //calculate the point we are evaluating
                        var _point = _offset + new Vector2(x * upc/ppc, y * upc/ppc);


                        //set the height of the vertex based on the provided location point
                        verts[y + x*(width)] = new Vector3(
                        _point.x,
                        HeightMapNoise.EvaluatePoint(
                            heightNoiseData,
                            _point.x,
                            _point.y),
                        _point.y);
                    }
           
                
                }
            }
            
        }
    }
}