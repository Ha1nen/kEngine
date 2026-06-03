using System;
using System.Collections.Generic;

namespace KEngine.Map
{
    [System.Serializable]
    internal class OptimizationSystem
    {
        private const byte CHUNK_SIZE = 8;
        private const byte MAP_HEIGHT = 35;
        private const byte MAP_WIDTH = 50;

        private ParticleSystem particleSystem;

        private Chunk[,] chunks = new Chunk[MAP_HEIGHT, MAP_WIDTH];

        public OptimizationSystem()
        {
            for (int x = 0; x < MAP_WIDTH; x++)
            {
                for (int y = 0; y < MAP_HEIGHT; y++)
                {
                    chunks[y, x] = new Chunk(x, y);
                }
            }
        }
        public void LoadParticleSystem(ParticleSystem particleSystem)
        {
            this.particleSystem = particleSystem;
        }

        private List<Chunk> deactivatingChunks = new List<Chunk>();

        public void UpdateChunks(uint frameCount)
        {
            bool frameCountEven = frameCount % 2 == 0;
            for (int x = 0; x < MAP_WIDTH; x++)
            { 
                for (int y = 0; y < MAP_HEIGHT ; y++)
                {
                    if (chunks[y, x].ShouldStep == true)
                    {
                        if (UpdateChunkAndCheckForDeactivating(chunks[y, x], frameCountEven))
                        {
                            deactivatingChunks.Add(chunks[y, x]);
                        }
                    }
                }
            }

            for (int i = 0; i < deactivatingChunks.Count; i++)
            {
                deactivatingChunks[i].ShouldStep = deactivatingChunks[i].ShouldStepInNextframe;
                deactivatingChunks[i].ShouldStepInNextframe = false;
            }
            deactivatingChunks.Clear();
        }

        public void ActivateChunk(int x, int y)
        {
            x = (int)Math.Floor((double)(x / CHUNK_SIZE));
            y = (int)Math.Floor((double)(y / CHUNK_SIZE));

            if (IsChunkExist(x, y))
            {
                chunks[y, x].ShouldStep = true;
                chunks[y, x].ShouldStepInNextframe = true;
            }
        }

        private bool UpdateChunkAndCheckForDeactivating(Chunk chunk, bool frameCountEven)
        {
            bool willDeactivate = true;

            for (int y = ((chunk.PosY + 1) * CHUNK_SIZE) - 1; y >= chunk.PosY * CHUNK_SIZE; y--)
            {
                for (int x = frameCountEven ? chunk.PosX * CHUNK_SIZE : (chunk.PosX + 1) * CHUNK_SIZE - 1; frameCountEven ? x < (chunk.PosX + 1) * CHUNK_SIZE : x >= chunk.PosX * CHUNK_SIZE; x += frameCountEven ? 1 : -1)
                {
                    particleSystem.UpdateTile(x, y);
                }
            }



            //Checking chunk for not moving objects
            for (int y = ((chunk.PosY + 1) * CHUNK_SIZE) - 1; y >= chunk.PosY * CHUNK_SIZE; y--)
            {
                for (int x = frameCountEven ? chunk.PosX * CHUNK_SIZE : (chunk.PosX + 1) * CHUNK_SIZE - 1; frameCountEven ? x < (chunk.PosX + 1) * CHUNK_SIZE : x >= chunk.PosX * CHUNK_SIZE; x += frameCountEven ? 1 : -1)
                {
                    if (particleSystem.IsTileActive(x, y))
                    {
                        willDeactivate = false;
                        break;
                    }
                }
                if (!willDeactivate)
                {
                    break;
                }
            }

            return willDeactivate;
        }


        public void CheckForNeighborChunks(int x, int y)
        {
            if (InBoundsOfChunks(x, y))
            {
                if (x % CHUNK_SIZE == 0)
                {
                    ActivateChunk(x - 1, y);
                }
                else if (x % CHUNK_SIZE == CHUNK_SIZE - 1)
                {
                    ActivateChunk(x + 1, y);
                }

                if (y % CHUNK_SIZE == 0)
                {
                    ActivateChunk(x, y - 1);
                }
                else if (y % CHUNK_SIZE == CHUNK_SIZE - 1)
                {
                    ActivateChunk(x, y + 1);
                }
            }
        }
        public Chunk GetChunkAt(int x, int y)
        {
            x = (int)Math.Floor((double)(x / CHUNK_SIZE));
            y = (int)Math.Floor((double)(y / CHUNK_SIZE));

            if (IsChunkExist(x, y))
            {
                return chunks[y, x];
            }
            return chunks[0, 0];
        }

        private bool InBoundsOfChunks(int x, int y)
        {
            return x >= 0 && y >= 0 && x < MAP_WIDTH * CHUNK_SIZE && y < MAP_HEIGHT * CHUNK_SIZE;
        }
        private bool IsChunkExist(int x, int y)
        {
            return x >= 0 && y >= 0 && x < MAP_WIDTH && y < MAP_HEIGHT;
        }

        //_______________________________________________________________________________________________\\
        public static ushort GetMapWidth()
        {
            return CHUNK_SIZE * MAP_WIDTH;
        }
        public static ushort GetMapHeight()
        {
            return CHUNK_SIZE * MAP_HEIGHT;
        }
    }
}