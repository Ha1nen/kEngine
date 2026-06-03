using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEngine.Map
{
    internal class MapSystem
    {
        private const byte CHUNK_SIZE = 128;
        private const byte MAP_HEIGHT = 7;
        private const byte MAP_WIDTH = 9;

        public struct Chunk
        {
            private int posX, posY;

            public int PosX { get { return posX; } set { posX = value; } }
            public int PosY { get { return posY; } set { posY = value; } }

            public Chunk(int x, int y)
            {
                posX = x;
                posY = y;
            }
        }

        private int[,] map = new int[CHUNK_SIZE * MAP_HEIGHT, CHUNK_SIZE * MAP_WIDTH];
        private Chunk[,] chunks = new Chunk[MAP_HEIGHT, MAP_WIDTH];
        private List<Chunk> activeChunks = new List<Chunk>();

        public void InitializeChunks()
        {
            for (int x = 0; x < MAP_WIDTH; x++)
            {
                for (int y = 0; y < MAP_HEIGHT; y++)
                {
                    chunks[y, x] = new Chunk(x + 1, y + 1);
                    activeChunks.Add(chunks[y, x]);
                }
            }
        }

        public void UpdateChunks(uint frameCount)
        {
            bool frameCountEven = frameCount % 2 == 0;

            //Update active chunks
            foreach (Chunk chunk in activeChunks)
            {
                for (int y = chunk.PosY * CHUNK_SIZE - 1; y >= 0; y--)
                {
                    for (int x = frameCountEven ? (chunk.PosX - 1) * CHUNK_SIZE : chunk.PosX * CHUNK_SIZE - 1; frameCountEven ? x < chunk.PosX * CHUNK_SIZE : x >= (chunk.PosX - 1) * CHUNK_SIZE; x += frameCountEven ? 1 : -1)
                    {

                    }
                }
                for (int y = chunk.PosY; y < chunk.PosY * CHUNK_SIZE; y++)
                {
                    for (int x = frameCountEven ? (chunk.PosX - 1) * CHUNK_SIZE : chunk.PosX * CHUNK_SIZE - 1; frameCountEven ? x < chunk.PosX * CHUNK_SIZE : x >= (chunk.PosX - 1) * CHUNK_SIZE; x += frameCountEven ? 1 : -1)
                    {

                    }
                }
            }
        }

        public ushort GetMapWidth()
        {
            return CHUNK_SIZE * MAP_WIDTH;
        }
        public ushort GetMapHeight()
        {
            return CHUNK_SIZE * MAP_HEIGHT;
        }
    }
}
