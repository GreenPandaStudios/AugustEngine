namespace AugustEngine.Procedural.Chunks
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using AugustEngine.Vectors;
    using System;
    public class ChunkManager : MonoBehaviour
    {
        [SerializeField] private int pointsPerChunk;
        public int PointsPerUnit { get => pointsPerChunk; }
        [SerializeField] private int unitsPerChunk;
        public int UnitsPerChunk { get => unitsPerChunk; }
        [SerializeField] NoiseGeneration.NoiseGenerator heightMap;
        [SerializeField] Material chunkMaterial;
        [SerializeField] Transform buildAround;
        [SerializeField] bool generatePhysics;
        [SerializeField] int chunkRadius;
        [SerializeField] int minChunkRadius;

        private Dictionary<Vector2Int, Chunk> activeChunks;
        private Queue<Chunk> inactiveChunks;

        public Action<Vector2Int> CenterChunkChanged;
        public Vector2Int CenterChunk
        {
            get => new Vector2Int(((int)buildAround.position.x / UnitsPerChunk),
           ((int)buildAround.position.z / UnitsPerChunk));
        }
        private Vector2Int lastChunk = new Vector2Int(1000000, 0);
        private void OnEnable()
        {
            if (pointsPerChunk < 1 || unitsPerChunk == 0)
            {
                Debug.LogError(this + " expects more than 1 points per chunk (currently " + pointsPerChunk +
                    ") and units per chunk (currently" + unitsPerChunk + ")!");
                this.enabled = false;
                return;
            }
            if (unitsPerChunk % pointsPerChunk != 0)
            {
                Debug.LogError(this + " expects the units per chunk to be divisible by the points per chunk!");
                this.enabled = false;
                return;
            }

            activeChunks = new Dictionary<Vector2Int, Chunk>();
            inactiveChunks = new Queue<Chunk>();
           
        }



        
        

        private void FixedUpdate()
        {
            var _c = CenterChunk;

            if (lastChunk != _c)
            {
                lastChunk = _c;

                //seed 5 chunks to generate
                CreateNewChunk(CenterChunk);
                CreateNewChunk(CenterChunk + new Vector2Int(-(int)chunkRadius, 0));
                
                CreateNewChunk(CenterChunk + new Vector2Int((int)chunkRadius, 0));
                CreateNewChunk(CenterChunk + new Vector2Int(0,-(int)chunkRadius));
                CreateNewChunk(CenterChunk + new Vector2Int(0, (int)chunkRadius));
                
                //let the chunks check if they need to despawn
                CenterChunkChanged?.Invoke(_c);
            }
        }

        private bool CreateNewChunk(Vector2Int ChunkPosition)
        {
            //check that we don't already have this chunk active
            if (activeChunks.ContainsKey(ChunkPosition))
            {
                //Debug.LogWarning("Tried to generate Chunk " + ChunkPosition + ", which is still active!")
                
                return false;
            }

            //check if this chunk is beyond the radius
            if (!InBounds(ChunkPosition)) return false;

            //check if we have any available chunks to use in the inactive queue
            Chunk _c;
            if (inactiveChunks.Count > 0)
            {
                //use an inactive chunk
                _c = inactiveChunks.Dequeue();
                _c.gameObject.SetActive(true);
                _c.gameObject.name = "Chunk " + ChunkPosition;
                _c.gameObject.layer = gameObject.layer;
            }
            else
            {
                //create a new chunk
                var _go = new GameObject("Chunk " + ChunkPosition);
                _go.transform.parent = this.transform;

                _go.AddComponent<InfiniteWorldTransform>();
                _go.layer = gameObject.layer;

                _c = _go.AddComponent<Chunk>();
            }


            _c.generatePhysics = generatePhysics;
            _c.material = chunkMaterial;
            
            //listen for state changes
            _c.OnStateChange += ChunkStateChange;
            ChunkStateChange(_c, _c.State);

            //listen for checking if chunks needs to be despawned
            CenterChunkChanged += loc => CheckForDespawn(_c);
            
            
            

            //add to active chunks
            activeChunks.Add(ChunkPosition, _c);


            var _chunkJob = new Chunk.GenerateChunkJob();

            _chunkJob.ChunkPos = ChunkPosition;
            _chunkJob.ppc = pointsPerChunk;
            _chunkJob.upc = unitsPerChunk;
            _chunkJob.heightNoiseData = heightMap.data;

            _c.Generate(_chunkJob);

            //Generate surrounding chunks too
            StartCoroutine(GenerateSurroundingChunks(ChunkPosition));
            return true;
        }
        IEnumerator GenerateSurroundingChunks(Vector2Int ChunkPosition)
        {
            //if a chunk will propogate the generation, return control after that frame
            //so the game doesn't hang. If it will not propogate, just continue

           
            if (CreateNewChunk(ChunkPosition + new Vector2Int(0, 1)))
                yield return null;
            if (CreateNewChunk(ChunkPosition + new Vector2Int(1, 0)))
                yield return null;
            if (CreateNewChunk(ChunkPosition + new Vector2Int(0, -1)))
                yield return null;
            if (CreateNewChunk(ChunkPosition + new Vector2Int(-1, 0)))
                yield return null;

        }

        private void ChunkStateChange(Chunk caller, Chunk.ChunkState state)
        {
            if (state == Chunk.ChunkState.Destroying)
            {
                Debug.Log(caller + " marked to be destroyed");
                //schedule to destroy



                DestroyChunk(caller);
            }
           
        }


        public bool InBounds(Vector2Int myLoc)
        {

            //beyond the max radius
            if (Mathf.Abs(lastChunk.x - myLoc.x) > chunkRadius ||
                Mathf.Abs(lastChunk.y - myLoc.y) > chunkRadius)
            {

                return false;
            }
            //inside the min radius
            else if (minChunkRadius > 0 && 
                (Mathf.Abs(lastChunk.x - myLoc.x) < minChunkRadius &&
                Mathf.Abs(lastChunk.y - myLoc.y) < minChunkRadius)){

                return false;

            }
            return true;
        }

        private void CheckForDespawn(Chunk _c)
        {
            if (!InBounds(_c.ChunkPosition)) _c.MarkForDestroy();
        }

        private void DestroyChunk(Chunk _chunk)
        {
            //clean up any data on this chunk
            _chunk.OnStateChange -= ChunkStateChange;
            CenterChunkChanged -= loc => CheckForDespawn(_chunk);

            activeChunks.Remove(_chunk.ChunkPosition);
            _chunk.Deactivate();
            inactiveChunks.Enqueue(_chunk);
        }
    }
}
