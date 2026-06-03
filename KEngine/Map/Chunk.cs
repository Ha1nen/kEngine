namespace KEngine.Map
{
    internal class Chunk
    {
        private int posX, posY;
        private bool shouldStep, shouldStepInNextFrame;

        public int PosX { get { return posX; } set { posX = value; } }
        public int PosY { get { return posY; } set { posY = value; } }

        public bool ShouldStep { get { return shouldStep; } set { shouldStep = value; } }
        public bool ShouldStepInNextframe { get { return shouldStepInNextFrame; } set { shouldStepInNextFrame = value; } }

        public Chunk(int x, int y)
        {
            posX = x;
            posY = y;
            shouldStep = true;
            shouldStepInNextFrame = true;
        }
    }
}