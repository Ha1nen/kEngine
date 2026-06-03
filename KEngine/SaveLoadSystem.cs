using KEngine.Map;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace KEngine
{
    internal class SaveLoadSystem
    {
        public ParticleSystem particleSystem;

        private string filePath = "save.dat";

        public SaveLoadSystem(ParticleSystem particleSystem)
        {
            this.particleSystem = particleSystem;
        }

        public void Save()
        {
            BinaryFormatter binaryFormater = new BinaryFormatter();
            FileStream fileStream = new FileStream(filePath, FileMode.Create);

            Save save = new Save();

            save.SaveMap(particleSystem);
            binaryFormater.Serialize(fileStream, save);
            fileStream.Close();
        }
        public void Load()
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            BinaryFormatter binaryFormater = new BinaryFormatter();
            FileStream fileStream = new FileStream(filePath, FileMode.Open);

            Save save = (Save)binaryFormater.Deserialize(fileStream);
            fileStream.Close();

            particleSystem.Load(save);
        }
    }

    [System.Serializable]
    internal class Save : ParticleSystem
    {
        public Particle[,] ParticleMap = new Particle[OptimizationSystem.GetMapHeight(), OptimizationSystem.GetMapWidth()];
        public List<int> ObjectsX = new List<int>();
        public List<int> ObjectsY = new List<int>();

        public void SaveMap(ParticleSystem particleSystem)
        {
            for (int x = 0; x < OptimizationSystem.GetMapWidth(); x++)
            {
                for (int y = 0; y < OptimizationSystem.GetMapHeight(); y++)
                {
                    if (particleSystem.GetTileAt(x, y) != particleSystem.pObject)
                    {
                        ParticleMap[y, x] = particleSystem.GetTileAt(x, y);
                    }
                }
            }

            for (int i = 0; i < particleSystem.GetObjectsCount(); i++)
            {
                ObjectsX.Add(particleSystem.GetObject(i).X);
                ObjectsY.Add(particleSystem.GetObject(i).Y);
            }
        }
    }
}