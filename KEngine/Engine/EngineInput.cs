using SFML.Window;
using System;
using KEngine.Map;

namespace KEngine
{
    internal class EngineInput
    {
        private ParticleSystem particleSystem;
        private EngineWindow window;
        private SaveLoadSystem saveLoadSystem;

        private bool isMouseButtonHolding = false;
        private bool pressedLeft = false;
        private bool pressedRight = false;
        private int brushSize = 1;

        private int choosedID = 1;

        public int BrushSize { get { return brushSize; } }

        public EngineInput(SaveLoadSystem saveLoadSystem)
        {
            this.saveLoadSystem = saveLoadSystem;
        }

        public void GetKeyInputs(object sender, KeyEventArgs key)
        {
            switch (key.Code)
            {
                //Operations with program
                case Keyboard.Key.Escape:
                    ((Window)sender).Close();
                    break;
                case Keyboard.Key.Space:
                    Program.TogglePause();
                    break;
                case Keyboard.Key.S:
                    saveLoadSystem.Save();
                    break;
                case Keyboard.Key.L:
                    saveLoadSystem.Load();
                    break;
                case Keyboard.Key.I:
                    Program.ToggleInstruction();
                    break;
                //Operations with brushes and game field
                case Keyboard.Key.LBracket:
                    if (brushSize > 1)
                    {
                        brushSize--;
                    }
                    break;
                case Keyboard.Key.RBracket:
                    if (brushSize < 200)
                    {
                        brushSize++;
                    }                    
                    break;
                case Keyboard.Key.C:
                    particleSystem.CleanField();
                    break;
                //Choosing materials
                case Keyboard.Key.Num1:
                    Program.ChangeParticle(1);
                    choosedID = 1;
                    break;
                case Keyboard.Key.Num2:
                    Program.ChangeParticle(2);
                    choosedID = 2;
                    break;
                case Keyboard.Key.Num3:
                    Program.ChangeParticle(3);
                    choosedID = 3;
                    break;
                case Keyboard.Key.Num4:
                    Program.ChangeParticle(4);
                    choosedID = 4;
                    break;
                case Keyboard.Key.Num5:
                    Program.ChangeParticle(5);
                    choosedID = 5;
                    break;
                case Keyboard.Key.Num6:
                    Program.ChangeParticle(6);
                    choosedID = 6;
                    break;
                case Keyboard.Key.Num7:
                    Program.ChangeParticle(7);
                    choosedID = 7;
                    break;
                case Keyboard.Key.Num8:
                    Program.ChangeParticle(8);
                    choosedID = 8;
                    break;
                case Keyboard.Key.Num9:
                    Program.ChangeParticle(9);
                    choosedID = 9;
                    break;
                case Keyboard.Key.Num0:
                    Program.ChangeParticle(0);
                    choosedID = 0;
                    break;
                case Keyboard.Key.Z:
                    particleSystem.DeleteObject();
                    break;
            }
        }

        public void GetMouseInputPress(object sender, MouseButtonEventArgs key)
        {
            int mouseX = Mouse.GetPosition(window._Window).X / EngineWindow.TILE_SCALE;
            int mouseY = Mouse.GetPosition(window._Window).Y / EngineWindow.TILE_SCALE;

            if (key.Button == Mouse.Button.Left)
            {
                isMouseButtonHolding = true;
                pressedLeft = true;

                if (choosedID == 0)
                {
                    particleSystem.CreateObject(mouseX, mouseY);
                }
            }
            else if (key.Button == Mouse.Button.Right)
            {
                isMouseButtonHolding = true;
                pressedRight = true;
            }
        }
        public void GetMouseInputUnpress(object sender, MouseButtonEventArgs key)
        {
            if (key.Button == Mouse.Button.Left)
            {
                isMouseButtonHolding = false;
                pressedLeft = false;
            }
            else if (key.Button == Mouse.Button.Right)
            {
                isMouseButtonHolding = false;
                pressedRight = false;
            }
        }

        public void LoadWindowAndParticleSystem(EngineWindow window, ParticleSystem particleSystem)
        {
            this.window = window;
            this.particleSystem = particleSystem;
        }

        public void MouseInput(ushort id)
        {
            int mouseX = Mouse.GetPosition(window._Window).X / EngineWindow.TILE_SCALE;
            int mouseY = Mouse.GetPosition(window._Window).Y / EngineWindow.TILE_SCALE;

            if (isMouseButtonHolding && id != 0)
            {
                if (pressedLeft)
                {
                        for (int x = mouseX - brushSize; x <= mouseX + brushSize; x++)
                        {
                            for (int y = mouseY - brushSize; y <= mouseY + brushSize; y++)
                            {
                                if (Math.Pow(x - mouseX, 2) + Math.Pow(y - mouseY, 2) <= Math.Pow(brushSize, 2))
                                {
                                    switch (id)
                                    {
                                        case 1:
                                            particleSystem.SetTileAtBrush(x, y, particleSystem.pSand);
                                            break;
                                        case 2:
                                            particleSystem.SetTileAtBrush(x, y, particleSystem.pWater);
                                            break;
                                        case 3:
                                            particleSystem.SetTileAtBrush(x, y, particleSystem.pStone);
                                            break;
                                        case 4:
                                            particleSystem.SetTileAtBrush(x, y, particleSystem.pPebble);
                                            break;
                                        case 5:
                                            particleSystem.SetTileAtBrush(x, y, particleSystem.pSteam);
                                            break;
                                        case 6:
                                            particleSystem.SetTileAtBrush(x, y, particleSystem.pSmoke);
                                            break;
                                        case 7:
                                            particleSystem.SetTileAtBrush(x, y, particleSystem.pWood);
                                            break;
                                        case 8:
                                            particleSystem.SetTileAtBrush(x, y, particleSystem.pFire);
                                            break;
                                        case 9:
                                            particleSystem.SetTileAtBrush(x, y, particleSystem.pBurnedWood);
                                            break;
                                    }
                                }
                            }
                        }               
                }
                else if (pressedRight)
                {
                    for (int x = mouseX - brushSize; x <= mouseX + brushSize; x++)
                    {
                        for (int y = mouseY - brushSize; y <= mouseY + brushSize; y++)
                        {
                            if (Math.Pow(x - mouseX, 2) + Math.Pow(y - mouseY, 2) <= Math.Pow(brushSize, 2))
                            {
                                particleSystem.SetTileAtErise(x, y);
                            }
                        }
                    }
                }
            }
        }

    }
}