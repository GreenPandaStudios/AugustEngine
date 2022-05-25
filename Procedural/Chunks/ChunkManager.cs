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
                GenereateRegion(CenterChunk);
            }
        }

        private void GenereateRegion(Vector2Int center)
        {
           
            //loop through the square that is 2 larger than the radius
            for (int x = center.x - chunkRadius -  2 ; x < center.x + chunkRadius + 2; x++)
            {
                for (int y = center.y - chunkRadius- 2 ; y <center.y + chunkRadius + 2; y++)
                {
                    var _vec = new Vector2Int(x, y);
                    if (InBounds(_vec))
                    {
                        //generate
                        CreateNewChunk(_vec);
                    }
                    else
                    {
                        if (activeChunks.TryGetValue(_vec, out var _c)){
                            _c.MarkForDestroy();
                        }
                    }
                }
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
           // CenterChunkChanged += loc => CheckForDespawn(_c);
            
            
            

            //add to active chunks
            activeChunks.Add(ChunkPosition, _c);


            var _chunkJob = new Chunk.GenerateChunkJob();

            _chunkJob.ChunkPos = ChunkPosition;
            _chunkJob.ppc = pointsPerChunk;
            _chunkJob.upc = unitsPerChunk;
            _chunkJob.heightNoiseData = heightMap.data;

            _c.Generate(_chunkJob);

            //Generate surrounding chunks too
           // StartCoroutine(GenerateSurroundingChunks(ChunkPosition));
            return true;
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


        public bool InBounds(Vector2Int myLoc){

            var _dist = Vector2Int.Distance(myLoc, lastChunk);
            if (_dist > chunkRadius) return false;
            if (_dist < minChunkRadius) return false;
            return true;
        }

      
        private void DestroyChunk(Chunk _chunk)
        {
            //clean up any data on this chunk
            _chunk.OnStateChange -= ChunkStateChange;
            

            activeChunks.Remove(_chunk.ChunkPosition);
            _chunk.Deactivate();
            inactiveChunks.Enqueue(_chunk);
        }
    }
}
