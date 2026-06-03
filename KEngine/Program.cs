using KEngine.Map;

namespace KEngine
{
    internal class Program
    {
        //Engine basic settings
        private static EngineWindow window;
        private static EngineInput input;

        //Map & particle system
        private static OptimizationSystem optimizationSystem;
        private static ParticleSystem particleSystem;
        private static SaveLoadSystem saveLoadSystem;

        private static uint frameCount = 0;
        private static ushort particleID = 1;
        private static bool isPaused, showingInstruction;

        public static ushort ParticleID { get { return particleID; } }

        static void Main(string[] args)
        {
            InitEngine();

            while (window.IsOpen())
            {
                input.MouseInput(particleID);
                window.Update();

                if (showingInstruction)
                {
                    window.Instruction.DisplayedString = "Автор программы: Минязев Тимур\nУправление: ЛКМ для рисования, ПКМ для стирания\n[ - уменьшить кисть\n] - увеличить кисть\nМаксимальный размер кисти - 200\nЧтобы удалять объекты используйте клавишу Z,\nа чтобы разместить ЛКМ";
                }
                else
                {
                    window.Instruction.DisplayedString = "";
                }

                if (!isPaused)
                {
                    window.PauseText.DisplayedString = "";

                    particleSystem.UpdateObjects();
                    optimizationSystem.UpdateChunks(frameCount);
                    
                }
                else
                {
                    window.PauseText.DisplayedString = "Paused";
                }

                //If uint reached its maximal value
                if (frameCount <= 4294967295)
                {
                    frameCount++;
                }
                else
                {
                    frameCount = 0;
                }

                window.Display();
            }
        }

        private static void InitEngine()
        {
            EngineContent.Load();

            particleSystem = new ParticleSystem();
            optimizationSystem = new OptimizationSystem();

            saveLoadSystem = new SaveLoadSystem(particleSystem);
            input = new EngineInput(saveLoadSystem);

            window = new EngineWindow(OptimizationSystem.GetMapWidth(), OptimizationSystem.GetMapHeight(), input, particleSystem);

            particleSystem.LoadOptimizationSystemAndMap(optimizationSystem);
            optimizationSystem.LoadParticleSystem(particleSystem);
            input.LoadWindowAndParticleSystem(window, particleSystem);
        }


        public static void TogglePause()
        {
            isPaused = !isPaused;
        }
        public static void ToggleInstruction()
        {
            showingInstruction = !showingInstruction;
        }
        public static void ChangeParticle(ushort ParticleID)
        {
            particleID = ParticleID;
        }
    }
}