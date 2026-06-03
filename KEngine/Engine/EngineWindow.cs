using SFML.System;
using SFML.Graphics;
using SFML.Window;
using System;
using KEngine.Map;

namespace KEngine
{
    internal class EngineWindow
    {
        //Window
        public const byte TILE_SCALE = 2;
        private const ushort ADD_WINDOW_WIDTH = 248;
        private const ushort ADD_WINDOW_HEIGHT = 0;
        private const string TITLE = "kEngine v 1.0 Release";
        private Color backColor = new Color(86, 100, 86);

        private RenderWindow window;

        private ushort gameFieldWidth, gameFieldHeight;
        private Image mapImage;
        ////////

        private ParticleSystem particleSystem;
        private EngineInput input;

        public RenderWindow _Window { get { return window; } }

        //UI
        public Text PauseText, Instruction;
        private Text controls, brushSize;

        public EngineWindow(ushort gameFieldWidth, ushort gameFieldHeight, EngineInput input, ParticleSystem particleSystem)
        {
            this.particleSystem = particleSystem;
            this.input = input;

            this.gameFieldWidth = (ushort)(gameFieldWidth * TILE_SCALE);
            this.gameFieldHeight = (ushort)(gameFieldHeight * TILE_SCALE);

            mapImage = new Image(this.gameFieldWidth, this.gameFieldHeight);


            window = new RenderWindow(new VideoMode((uint)this.gameFieldWidth + ADD_WINDOW_WIDTH, (uint)this.gameFieldHeight + ADD_WINDOW_HEIGHT), TITLE, Styles.Close);
            window.Closed += (sender, e) => { ((Window)sender).Close(); };
            window.KeyPressed += new EventHandler<KeyEventArgs>(input.GetKeyInputs);
            window.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(input.GetMouseInputPress);
            window.MouseButtonReleased += new EventHandler<MouseButtonEventArgs>(input.GetMouseInputUnpress);

            Instruction = new Text("", EngineContent.GameFont, 12);
            Instruction.Position = new Vector2f(24, 24);
            PauseText = new Text("", EngineContent.GameFont, 24);
            PauseText.Position = new Vector2f(this.gameFieldWidth - 200, this.gameFieldHeight - 90);
            controls = new Text("esc - exit\nspace - \npause/unpause\nc - clean field\ns - save\nl - load\ni - instruction\n\n1 - sand\n2 - water\n3 - stone\n4 - pebble\n5 - steam\n6 - smoke\n7 - wood\n8 - fire\n9 - burned wood\n0 - object", EngineContent.GameFont, 12);
            controls.Position = new Vector2f(this.gameFieldWidth + 24, 24);
            brushSize = new Text("Brush size: " + input.BrushSize + "\nMaterial: " + Program.ParticleID, EngineContent.GameFont, 12);
            brushSize.Position = new Vector2f(this.gameFieldWidth + 24, this.gameFieldHeight - 48);
        }

        public bool IsOpen()
        {
            return window.IsOpen;
        }

        public void Display()
        {
            for (uint x = 0; x < gameFieldWidth; x += TILE_SCALE)
            {
                for (uint y = 0; y < gameFieldHeight; y += TILE_SCALE)
                {
                    Color curPixelColor = new Color(particleSystem.GetTileAt((int)x / TILE_SCALE, (int)y / TILE_SCALE).R, particleSystem.GetTileAt((int)x / TILE_SCALE, (int)y / TILE_SCALE).G, particleSystem.GetTileAt((int)x / TILE_SCALE, (int)y / TILE_SCALE).B);
                    

                    for (uint curPixelX = x; curPixelX < x + TILE_SCALE; curPixelX++)
                    {

                        for (uint curPixelY = y; curPixelY < y + TILE_SCALE; curPixelY++)
                        {
                            
                           mapImage.SetPixel(curPixelX, curPixelY, curPixelColor);
                        }
                    }
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
        }

        private void DrawUI()
        {
            brushSize.DisplayedString = "Brush size: " + input.BrushSize + "\nMaterial: " + Program.ParticleID;

            window.Draw(brushSize);
            window.Draw(PauseText);
            window.Draw(controls);
            window.Draw(Instruction);
        }
    }
}