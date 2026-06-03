using System;
using System.Collections.Generic;

namespace KEngine.Map
{
    [System.Serializable]
    internal class ParticleSystem
    {
        private const float GRAVITY_FACTOR = 2;
        private Random rnd = new Random();

        //Map setup
        private OptimizationSystem optimizationSystem;
        private Particle[,] particleMap;
        //---------

        //Defining particles and their ID's
        public enum ParticleType { SOLID, LIQUID, GAS, PHYS_SOLID, PLASMA };
        [System.Serializable]
        public struct Particle
        {
            private ushort id;
            private int friability;
            private bool isMoving;
            private byte r, g, b, mass;
            private ParticleType type;

            public ushort ID { get { return id; } set { id = value; } }
            public int Friability { get { return friability; } }
            public bool IsMoving { get { return isMoving; } set { isMoving = value; } }
            public byte Mass { get { return mass; } }
            public ParticleType Type {  get {  return type;  }  }


            public byte R { get { return r; } }
            public byte G { get { return g; } }
            public byte B { get { return b; } }


            public Particle(ushort id, byte mass, int friability, bool isMoving, ParticleType type, byte r, byte g, byte b)
            {
                this.id = id;
                this.friability = friability;
                this.isMoving = isMoving;
                this.type = type;
                this.mass = mass;

                this.r = r;
                this.g = g;
                this.b = b;
            }

            public static bool operator ==(Particle particle1, Particle particle2)
            {
                return particle1.id == particle2.id;
            }
            public static bool operator !=(Particle particle1, Particle particle2)
            {
                return !(particle1.id == particle2.id);
            }
        }

        

        public Particle pEmpty = new Particle(0, 0, 0, false, ParticleType.SOLID, 0, 0, 0);
        public Particle pSand = new Particle(1, 1, 10, true, ParticleType.PHYS_SOLID, 200, 200, 0);
        public Particle pWater = new Particle(2, 1, 25, true, ParticleType.LIQUID, 50, 50, 225);
        public Particle pStone = new Particle(3, 0, 0, false, ParticleType.SOLID, 75, 75, 75);
        public Particle pPebble = new Particle(4, 3, 3, true, ParticleType.PHYS_SOLID, 130, 130, 130);
        public Particle pSteam = new Particle(5, 1, 35, true, ParticleType.GAS, 200, 200, 200);
        public Particle pSmoke = new Particle(6, 1, 35, true, ParticleType.GAS, 45, 45, 45);
        public Particle pWood = new Particle(7, 1, 0, false, ParticleType.SOLID, 200, 100, 50);
        public Particle pFire = new Particle(8, 1, 35, true, ParticleType.PLASMA, 255, 150, 0);
        public Particle pBurnedWood = new Particle(9, 3, 3, true, ParticleType.PHYS_SOLID, 40, 20, 10);
        public Particle pObject = new Particle(10, 3, 3, true, ParticleType.SOLID, 75, 75, 75);
        //--------------------------------

        //Defining  objects
        [System.Serializable]
        public struct Object
        {
            private int x, y, length, height, id, speed;
            private bool isMoving;

            public int X { get => x; set => x = value; }
            public int Y { get => y; set => y = value; }
            public int Length { get => length; }
            public int Height { get => height; }
            public int Speed { get => speed; set => speed = value; }
            public int ObjectID { get => id; }
            public bool IsMoving { get => isMoving; set => isMoving = value; }

            public Object(int id, int x, int y, int length, int height, int speed)
            {
                this.x = x;
                this.y = y;
                this.length = length;
                this.height = height;
                this.id = id;
                this.speed = speed;

                isMoving = true;
            }
        }

        private List<Object> objectList = new List<Object>();

        public void CreateObject(int x, int y)
        {
            Object newObject = new Object(objectList.Count, x, y, 24, 16, 10);
            objectList.Add(newObject);
        }
        public void DeleteObject()
        {
            if (objectList.Count > 0)
            {
                for (int y = objectList[objectList.Count - 1].Y + objectList[objectList.Count - 1].Height; y > objectList[objectList.Count - 1].Y; y--)
                {
                    for (int x = objectList[objectList.Count - 1].X; x < objectList[objectList.Count - 1].X + objectList[objectList.Count - 1].Length; x++)
                    {
                        particleMap[y, x] = pEmpty;
                    }
                }
                objectList.RemoveAt(objectList.Count - 1);
            }
        }
        public void UpdateObjects()
        {
            for (int obj = 0; obj < objectList.Count; obj++)
            {
                Object curObject = objectList[obj];

                int moveY = 0;
                bool foundBlocker = false;
                for (int y = curObject.Y + curObject.Height + 1; y < curObject.Y + curObject.Height + curObject.Speed; y++)
                {
                    for (int x = curObject.X; x < curObject.X + curObject.Length; x++)
                    {
                        if (!IsTileFree(x, y))
                        {
                            if (InBounds(x, y))
                            {
                                Particle curParticle = particleMap[y, x];
                                if (curParticle.Type == ParticleType.LIQUID || curParticle.Type == ParticleType.GAS || curParticle.Type == ParticleType.PLASMA)
                                {
                                    if (IsTileFree(x, curObject.Y - moveY))
                                    {
                                        particleMap[curObject.Y - moveY, x] = curParticle;
                                        int distance = rnd.Next(1, curParticle.Friability * (int)GRAVITY_FACTOR);

                                        MoveTile(x, curObject.Y - moveY, x, curObject.Y - moveY - distance, curParticle);
                                        optimizationSystem.ActivateChunk(x, curObject.Y - moveY);
                                    }
                                }
                                else
                                {
                                    foundBlocker = true;
                                    break;
                                }
                            }
                            else
                            {
                                foundBlocker = true;
                                break;
                            }                            
                        }

                    }
                    if (foundBlocker)
                    {
                        break;
                    }
                    else
                    {
                        moveY++;
                    }
                }

                if (moveY > 0)
                {
                    //Erasing object, prepearing it to move.
                    if (InBounds(curObject.X + curObject.Length, curObject.Y + curObject.Height) && InBounds(curObject.X, curObject.Y))
                    {
                        for (int y = curObject.Y + curObject.Height; y > curObject.Y; y--)
                        {
                            for (int x = curObject.X; x < curObject.X + curObject.Length; x++)
                            {
                                particleMap[y, x] = pEmpty;
                            }
                        }
                    }

                    //Moving
                    curObject.Y += moveY;

                    //Applying movement
                    if (InBounds(curObject.X + curObject.Length, curObject.Y + curObject.Height) && InBounds(curObject.X, curObject.Y))
                    {
                        for (int y = curObject.Y + curObject.Height; y > curObject.Y; y--)
                        {
                            for (int x = curObject.X; x < curObject.X + curObject.Length; x++)
                            {
                                particleMap[y, x] = pObject;
                                optimizationSystem.ActivateChunk(x, y);
                            }
                        }
                    }

                    objectList[obj] = curObject;
                }
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            }
        }

        public int GetObjectsCount()
        {
            return objectList.Count;
        }
        public Object GetObject(int index)
        {
            return objectList[index];
        }

        private void DeleteAllObjects()
        {
            if (objectList.Count > 0)
            {
                for (int y = objectList[objectList.Count - 1].Y + objectList[objectList.Count - 1].Height; y > objectList[objectList.Count - 1].Y; y--)
                {
                    for (int x = objectList[objectList.Count - 1].X; x < objectList[objectList.Count - 1].X + objectList[objectList.Count - 1].Length; x++)
                    {
                        particleMap[y, x] = pEmpty;
                    }
                }
            }
            objectList.Clear();
        }

        //--------------------------------

        public void LoadOptimizationSystemAndMap(OptimizationSystem optimizationSystem)
        {
            this.optimizationSystem = optimizationSystem;

            particleMap = new Particle[OptimizationSystem.GetMapHeight(), OptimizationSystem.GetMapWidth()];

            for (uint x = 0; x < OptimizationSystem.GetMapWidth(); x++)
            {
                for (uint y = 0; y < OptimizationSystem.GetMapHeight(); y++)
                {
                    particleMap[y, x] = pEmpty;
                }
            }
        }

        public void CleanField()
        {
            for (int x =  0; x < OptimizationSystem.GetMapWidth(); x++)
            {
                for (int y = 0; y < OptimizationSystem.GetMapHeight(); y++)
                {
                    particleMap[y, x] = pEmpty;
                }
            }

            DeleteAllObjects();
        }

        //Basic operations with particles
        public Particle GetTileAt(int x, int y)
        {
            if (InBounds(x, y))
            {
                return particleMap[y, x];
            }
            return pEmpty;
        }

        private bool InBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && y < OptimizationSystem.GetMapHeight() && x < OptimizationSystem.GetMapWidth();
        }

        private bool IsTileFree(int x, int y)
        {
            return InBounds(x, y) && particleMap[y, x] == pEmpty;
        }

        private void MoveTile(int fromX, int fromY, int toX, int toY, Particle particle)
        {
            int moveX = 0;
            int moveY = 0;

            bool fromXisLessThanToX = fromX <= toX;
            bool fromYisLessThanToY = fromY <= toY;

            
            if (fromY != toY)
            {
                for (int y = fromY; fromYisLessThanToY ? y < toY : y > toY; y += fromYisLessThanToY ? 1 : -1)
                {
                    if (IsTileFree(fromX, fromYisLessThanToY ? y + 1 : y - 1))
                    {
                        moveY += fromYisLessThanToY ? 1 : -1;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else if (fromX != toX)
            {
                for (int x = fromX; fromXisLessThanToX ? x < toX : x > toX; x += fromXisLessThanToX ? 1 : -1)
                {
                    if (IsTileFree(fromXisLessThanToX ? x + 1 : x - 1, fromY))
                    {
                        moveX += fromXisLessThanToX ? 1 : -1;
                    }
                    else
                    {
                        break;
                    }
                }
            }



            if (IsTileFree(fromX + moveX, fromY + moveY))
            {
                particleMap[fromY, fromX] = pEmpty;
                particleMap[fromY + moveY, fromX + moveX] = particle;
                particleMap[fromY + moveY, fromX + moveX].IsMoving = true;
            }
            if (InBounds(fromX + moveX, fromY + moveY))
            {
                optimizationSystem.CheckForNeighborChunks(fromX + moveX, fromY + moveY);
                optimizationSystem.ActivateChunk(fromX + moveX, fromY + moveY);
            }

        }
        //--------------------------------


        public void SetTileAtBrush(int x, int y, Particle particle)
        {
            if (IsTileFree(x, y))
            {
                particleMap[y, x] = particle;
                optimizationSystem.ActivateChunk(x, y);
            }
        }
        public void SetTileAtErise(int x, int y)
        {
            if (InBounds(x, y) && particleMap[y, x] != pObject)
            {
                particleMap[y, x] = pEmpty;
                optimizationSystem.ActivateChunk(x, y);
            }
        }

        public void SetTileAt(int x, int y, Particle particle)
        {
            if (InBounds(x, y))
            {
                particleMap[y, x] = particle;
                optimizationSystem.ActivateChunk(x, y);
            }
        }

        public void UpdateTile(int x, int y)
        {
            Particle curParticle = particleMap[y, x];
            bool isFirstCheckLeft = rnd.Next(0, 101) >= 50 ? true : false;


            //UPDATING TILE
            if (curParticle.Type == ParticleType.PHYS_SOLID)
            {
                int distance = rnd.Next(1, curParticle.Friability);

                optimizationSystem.CheckForNeighborChunks(x, y);


                //Checking tile for neighbors
                for (int checkingX = x; isFirstCheckLeft ? checkingX != x + 1 : checkingX != x - 1; checkingX = isFirstCheckLeft ? (checkingX == x ? x - 1 : x + 1) : (checkingX == x ? x + 1 : x - 1))
                {
                    if (InBounds(checkingX, y + 1) && (particleMap[y + 1, checkingX].Type == ParticleType.LIQUID || particleMap[y + 1, checkingX].Type == ParticleType.GAS || particleMap[y + 1, checkingX].Type == ParticleType.PLASMA))
                    {
                        Particle curCheckingParticle = particleMap[y, x];

                        particleMap[y, x] = particleMap[y + 1, checkingX];
                        particleMap[y + 1, checkingX] = curCheckingParticle;

                        y++;
                        x = checkingX;
                        break;
                    }
                }
                ////////////////////////


                //Moving tile
                if (IsTileFree(x, y + 1))
                {
                    particleMap[y, x].IsMoving = true;
                    MoveTile(x, y, x, y + (distance * (int)GRAVITY_FACTOR * curParticle.Mass), curParticle);
                }
                else if (isFirstCheckLeft)
                {
                    if (IsTileFree(x - 1, y + 1))
                    {
                        particleMap[y, x] = pEmpty;
                        MoveTile(x - 1, y + 1, x - distance, y + 1, curParticle);
                    }
                    else if (IsTileFree(x + 1, y + 1))
                    {
                        particleMap[y, x] = pEmpty;
                        MoveTile(x + 1, y + 1, x + distance, y + 1, curParticle);
                    }
                    else
                    {
                        particleMap[y, x].IsMoving = false;
                    }
                }
                else
                {
                    if (IsTileFree(x + 1, y + 1))
                    {
                        particleMap[y, x] = pEmpty;
                        MoveTile(x + 1, y + 1, x + distance, y + 1, curParticle);
                    }
                    else if (IsTileFree(x - 1, y + 1))
                    {
                        particleMap[y, x] = pEmpty;
                        MoveTile(x - 1, y + 1, x - distance, y + 1, curParticle);
                    }
                    else
                    {
                        particleMap[y, x].IsMoving = false;
                    }
                }
            }
            else if (curParticle.Type == ParticleType.LIQUID)
            {
                int distance = rnd.Next(1, curParticle.Friability);

                optimizationSystem.CheckForNeighborChunks(x, y);

                //Checking tile for neighbors
                if (InBounds(x, y + 1) && (particleMap[y + 1, x].Type == ParticleType.GAS || particleMap[y + 1, x].Type == ParticleType.PLASMA))
                {
                    Particle curCheckingParticle = particleMap[y, x];

                    particleMap[y, x] = particleMap[y + 1, x];
                    particleMap[y + 1, x] = curCheckingParticle;

                    y++;
                }
                ////////////////////////

                if (IsTileFree(x, y + 1))
                {
                    particleMap[y, x].IsMoving = true;
                    MoveTile(x, y, x, y + (distance * (int)GRAVITY_FACTOR * curParticle.Mass), curParticle);
                }
                else if (isFirstCheckLeft)
                {
                    if (IsTileFree(x - 1, y))
                    {
                        MoveTile(x, y, x - distance, y, curParticle);
                    }
                    else if (IsTileFree(x + 1, y))
                    {
                        MoveTile(x, y, x + distance, y, curParticle);
                    }
                    else
                    {
                        particleMap[y, x].IsMoving = false;
                    }
                }
                else
                {
                    if (IsTileFree(x + 1, y))
                    {
                        MoveTile(x, y, x + distance, y, curParticle);
                    }
                    else if (IsTileFree(x - 1, y))
                    {
                        MoveTile(x, y, x - distance, y, curParticle);
                    }
                    else
                    {
                        particleMap[y, x].IsMoving = false;
                    }
                }
            }
            else if (curParticle.Type == ParticleType.GAS)
            {
                int distance = rnd.Next(1, curParticle.Friability);

                optimizationSystem.CheckForNeighborChunks(x, y);

                //Checking tile for neighbors
                if (InBounds(x, y + 1) && particleMap[y, x] == pSmoke && particleMap[y + 1, x] == pSteam)
                {
                    Particle curCheckingParticle = particleMap[y, x];

                    particleMap[y, x] = particleMap[y + 1, x];
                    particleMap[y + 1, x] = curCheckingParticle;

                    y++;
                }
                ////////////////////////

                if (IsTileFree(x, y - 1))
                {
                    particleMap[y, x].IsMoving = true;
                    MoveTile(x, y, x, y - distance, curParticle);
                }
                else if (isFirstCheckLeft)
                {
                    if (IsTileFree(x - 1, y))
                    {
                        MoveTile(x, y, x - distance, y, curParticle);
                    }
                    else if (IsTileFree(x + 1, y))
                    {
                        MoveTile(x, y, x + distance, y, curParticle);
                    }
                    else
                    {
                        particleMap[y, x].IsMoving = false;
                    }
                }
                else
                {
                    if (IsTileFree(x + 1, y))
                    {
                        MoveTile(x, y, x + distance, y, curParticle);
                    }
                    else if (IsTileFree(x - 1, y))
                    {
                        MoveTile(x, y, x - distance, y, curParticle);
                    }
                    else
                    {
                        particleMap[y, x].IsMoving = false;
                    }
                }
            }
            else if (curParticle.Type == ParticleType.PLASMA)
            {
                int distance = rnd.Next(1, curParticle.Friability);

                optimizationSystem.CheckForNeighborChunks(x, y);


                //Checking tile for neighbors
                for (int checkingX = x - 1; checkingX <= x + 1; checkingX++)
                {
                    for (int checkingY = y - 1; checkingY <= y + 1; checkingY++)
                    {
                        if (InBounds(checkingX, checkingY))
                        {
                            if (particleMap[checkingY, checkingX] == pWater)
                            {
                                particleMap[checkingY, checkingX] = pSteam;
                                particleMap[y, x] = pEmpty;
                                return;
                            }
                            else if (particleMap[checkingY, checkingX] == pWood)
                            {
                                bool willIginite = rnd.Next(0, 100) >= 50 ? true : false;
                                bool willMakeCoal = rnd.Next(0, 100) >= 70 ? true : false;

                                if (willIginite)
                                {
                                    if (willMakeCoal)
                                    {
                                        particleMap[checkingY, checkingX] = pBurnedWood;
                                        particleMap[y, x] = pSmoke;

                                    }
                                    else
                                    {
                                        particleMap[checkingY, checkingX] = pFire;
                                        particleMap[y, x] = pSmoke;
                                    }
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }
                    }
                }

                if (InBounds(x, y + 1) && particleMap[y + 1, x].Type == ParticleType.GAS)
                {
                    Particle curCheckingParticle = particleMap[y, x];

                    particleMap[y, x] = particleMap[y + 1, x];
                    particleMap[y + 1, x] = curCheckingParticle;

                    y++;
                }

                bool willPutOut = rnd.Next(0, 100) >= 75 ? true : false;
                if (willPutOut)
                {
                    particleMap[y, x] = pSmoke;
                    return;
                }
                ////////////////////////

                if (IsTileFree(x, y - 1))
                {
                    particleMap[y, x].IsMoving = true;
                    MoveTile(x, y, x, y - distance, curParticle);
                }
                else if (isFirstCheckLeft)
                {
                    if (IsTileFree(x - 1, y))
                    {
                        MoveTile(x, y, x - distance, y, curParticle);
                    }
                    else if (IsTileFree(x + 1, y))
                    {
                        MoveTile(x, y, x + distance, y, curParticle);
                    }
                    else
                    {
                        particleMap[y, x].IsMoving = false;
                    }
                }
                else
                {
                    if (IsTileFree(x + 1, y))
                    {
                        MoveTile(x, y, x + distance, y, curParticle);
                    }
                    else if (IsTileFree(x - 1, y))
                    {
                        MoveTile(x, y, x - distance, y, curParticle);
                    }
                    else
                    {
                        particleMap[y, x].IsMoving = false;
                    }
                }
            }
            /////////////////////////////////////////////////////////////////////
        }
        public bool IsTileActive(int x, int y)
        {
            return particleMap[y, x].IsMoving;
        }


        public void Load(Save save)
        {
            CleanField();

            for (int x = 0; x < OptimizationSystem.GetMapWidth(); x++)
            {
                for (int y = 0; y < OptimizationSystem.GetMapHeight();  y++)
                {
                    particleMap[y, x] = save.ParticleMap[y, x];
                    optimizationSystem.ActivateChunk(x, y);
                }
            }
            for (int i = 0; i < save.ObjectsX.Count; i++)
            {
                CreateObject(save.ObjectsX[i], save.ObjectsY[i]);
            }

        }
    }
}