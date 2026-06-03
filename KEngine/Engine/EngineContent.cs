using SFML.Graphics;

namespace KEngine
{
    internal class EngineContent
    {
        private const string CONTENT_PATH = "..\\Content\\";

        public static Font GameFont;

        public static void Load()
        {
            GameFont = new Font(CONTENT_PATH + "\\Fonts\\press-start-2p-regular.ttf");
        }
    }
}