namespace AugustEngine.Procedural.Chunks
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using AugustEngine.Vectors;
    public class ChunkManager : MonoBehaviour
    {
        [SerializeField] private uint pointsPerUnit;
        public uint PointsPerUnit { get => pointsPerUnit; }
        [SerializeField] private uint unitsPerChunk;
        public uint UnitsPerChunk { get => unitsPerChunk; }
        [SerializeField] NoiseGeneration.NoiseGenerator heightMap;
        [SerializeField] Material chunkMaterial;
        private Queue<Chunk> destroyQueue;
        private Dictionary<Vector2Int, Chunk> activeChunks;

        private void OnEnable()
        {
            if (pointsPerUnit == 0 || unitsPerChunk == 0)
            {
                Debug.LogError(this + " expects more than 0 points per unit (currently " + pointsPerUnit +
                    ") and units per chunk (currently" + unitsPerChunk + ")!");
            }
            destroyQueue = new Queue<Chunk>();
            activeChunks = new Dictionary<Vector2Int, Chunk>();

           // InfiniteWorldTransform.GlobalOffsetChange += GenerateChunkRadius;

           
        }
        [SerializeField] Transform buildAround;
        private void FixedUpdate()
        {
            GenerateChunkRadius(player.position);
            GenerateChunkRadius(player.position + Vector3.forward * UnitsPerChunk);
            GenerateChunkRadius(player.position + Vector3.back * UnitsPerChunk);
            GenerateChunkRadius(player.position + Vector3.left * UnitsPerChunk);
            GenerateChunkRadius(player.position + Vector3.right * UnitsPerChunk);
        }

        private void GenerateChunkRadius(Vector3Double _off)
        {
            var _loc = new Vector2Int((int) ((long) _off.x / (long)UnitsPerChunk),
                (int) ((long)_off.z / (long)UnitsPerChunk));


            CreateNewChunk(_loc + new Vector2Int(0, 0));
            
            CreateNewChunk(_loc + new Vector2Int(1, 0));
            CreateNewChunk(_loc + new Vector2Int(1, 1));
            CreateNewChunk(_loc + new Vector2Int(1, -1));

            CreateNewChunk(_loc + new Vector2Int(-1, 0));
            CreateNewChunk(_loc + new Vector2Int(-1, 1));
            CreateNewChunk(_loc + new Vector2Int(-1, -1));


            CreateNewChunk(_loc + new Vector2Int(0, 1));
            CreateNewChunk(_loc + new Vector2Int(1, 1));
            CreateNewChunk(_loc + new Vector2Int(-1, 1));


            CreateNewChunk(_loc + new Vector2Int(0, -1));
            CreateNewChunk(_loc + new Vector2Int(1, -1));
            CreateNewChunk(_loc + new Vector2Int(-1, -1));
        }
        private void OnDisable()
        {
            //mark all for destruction
            foreach (Chunk _c in activeChunks.Values)
            {
                _c.MarkForDestroy();
            }
            ClearDestroyQueue();
        }
        private void CreateNewChunk(Vector2Int ChunkPosition)
        {
            //check that we don't already have this chunk active
            if (activeChunks.ContainsKey(ChunkPosition))
            {
                //Debug.LogWarning("Tried to generate Chunk " + ChunkPosition + ", which is still active!");
                return;
            }

            var _go = new GameObject("Chunk " + ChunkPosition);
            _go.transform.parent = this.transform;

            _go.AddComponent<InfiniteWorldTransform>();
            _go.layer = gameObject.layer;
           
            var _c = _go.AddComponent<Chunk>();
            _c.material = chunkMaterial;
            //listen for state changes
            _c.OnStateChange += ChunkStateChange;

            //add to active chunks
            activeChunks.Add(ChunkPosition, _c);


            var _chunkJob = new Chunk.GenerateChunkJob();

            _chunkJob.ChunkPos = ChunkPosition;
            _chunkJob.ppu = pointsPerUnit;
            _chunkJob.upc = unitsPerChunk;
            _chunkJob.heightNoiseData = heightMap.data;

            _c.Generate(_chunkJob);
        }


        private void ChunkStateChange(Chunk caller, Chunk.ChunkState state)
        {
            if (state == Chunk.ChunkState.Destroying)
            {
                //schedule to destroy
                destroyQueue.Enqueue(caller);
            }
        }



        private void ClearDestroyQueue()
        {
            //remove each chunk and stop listening for state changes
            while (destroyQueue.Count != 0)
            {
                var _chunk = 
                destroyQueue.Dequeue();

                //clean up any data on this chunk
                _chunk.OnStateChange -= ChunkStateChange;
                activeChunks.Remove(_chunk.ChunkPosition);

                //destory it
                Destroy(_chunk);
            }
        }
    }
}
