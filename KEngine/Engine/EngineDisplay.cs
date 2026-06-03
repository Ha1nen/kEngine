using SFML.System;
using SFML.Graphics;
using SFML.Window;
using System;

namespace KEngine
{
    internal class EngineDisplay
    {
        private const ushort ADD_WINDOW_WIDTH = 248;
        private const ushort ADD_WINDOW_HEIGHT = 0;

        private RenderWindow window;

        private ushort gameFieldWidth, gameFieldHeight;
        private string title = "kEngine v 0.49.6124";
        private Color backColor = new Color(86, 100, 86);
        private Image mapImage;

        private Event isKeyDown;

        public RenderWindow _Window { get { return window; } }

        //UI
        public Text PauseText;
        private Text controls;

        public bool IsOpen()
        {
            return window.IsOpen;
        }

        public void Display()
        {
            for (uint x = 0; x < gameFieldWidth; x++)
            {
                for (uint y = 0; y < gameFieldHeight; y++)
                {
                }
            }

            Texture mapTexture = new Texture(mapImage);
            Sprite mapSprite = new Sprite(mapTexture);

            window.Draw(mapSprite);

            DrawUI();

            window.Display();
        }

        public void Update()
        {
            window.DispatchEvents();
            window.Clear(backColor);

            GetInputs();
        }

        public void Load(ushort gameFieldWidth, ushort gameFieldHeight, EngineContent content)
        {
            this.gameFieldWidth = gameFieldWidth;
            this.gameFieldHeight = gameFieldHeight;

            mapImage = new Image(gameFieldWidth, gameFieldHeight);
            window = new RenderWindow(new VideoMode((uint)gameFieldWidth + ADD_WINDOW_WIDTH, (uint)gameFieldHeight + ADD_WINDOW_HEIGHT), title, Styles.Close);
            window.Closed += (sender, e) => { ((Window)sender).Close(); };

            isKeyDown = new Event();

            PauseText = new Text("", EngineContent.GameFont, 45);
            PauseText.Position = new Vector2f(gameFieldWidth - 200, gameFieldHeight - 90);
            controls = new Text("Esc - Exit\ns - save\n1 - sand\n2 - water\n", EngineContent.GameFont, 24);
            controls.Position = new Vector2f(gameFieldWidth + 24, 24);
        }


        private void DrawUI()
        {
            window.Draw(PauseText);
            window.Draw(controls);
        }

        private void GetInputs()
        {
            if (isKeyDown.Type == EventType.KeyPressed)
            {
                Console.WriteLine("HUI");
                if (isKeyDown.Key.Code == Keyboard.Key.Escape)
                {

                }
                else if (isKeyDown.Key.Code == Keyboard.Key.Space)
                {
                    Program.TogglePause();

                }
            }
        }
    }
}
