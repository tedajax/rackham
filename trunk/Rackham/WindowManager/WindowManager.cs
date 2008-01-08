#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using System.Xml.Serialization;
#endregion

namespace Tanks
{
    
    public class WindowManager : DrawableGameComponent
    {

        public static SignedInGamer CurrentGamer;

        public static GamePlayer[] GamePlayers;

        List<GameWindow> windows = new List<GameWindow>();

        List<GameWindow> windowstoUpdate = new List<GameWindow>();

        ContentManager content;

        //GraphicsDevice graphicsDevice;
        SpriteBatch spriteBatch;
        SpriteFont gameFont;

        IGraphicsDeviceService graphicsDeviceService;

        public KeyboardState OldState;
        public KeyboardState NewState;

        public bool LoadNewProfile;
        public bool DisplayMessage;
        public int GamerNumToLoad;
        private int SelectedItem = 0;
        private bool EditThisItem = false;
        private Keys[] KeysForPlayer;

        private TextboxManager WindowManagerTextBoxes;

        
        #region exposestuff

        new public Game Game
        {
            get { return base.Game; }
        }

        new public GraphicsDevice GraphicsDevice
        {
            get { return base.GraphicsDevice; }
            
        }

        public ContentManager Content
        {
            get { return content; }
        }

        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }




#endregion

        public WindowManager(Game game)
            : base(game)
        {
            content = new ContentManager(game.Services);

            graphicsDeviceService = (IGraphicsDeviceService)game.Services.GetService(
                                                        typeof(IGraphicsDeviceService));

            if (graphicsDeviceService == null)
                throw new InvalidOperationException("No graphics device service.");

            OldState = Keyboard.GetState();

            LoadNewProfile = false;
            CurrentGamer = null;
            GamePlayers = new GamePlayer[4];
            WindowManagerTextBoxes = new TextboxManager();
        }

        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            if (loadAllContent)
            {
                spriteBatch = new SpriteBatch(GraphicsDevice);
                gameFont = content.Load<SpriteFont>("Content\\SpriteFont1");
            }

            foreach (GameWindow window in windows)
            {
                window.LoadGraphicsContent(loadAllContent);
            }
            
        }

        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            if (unloadAllContent)
            {
                Content.Unload();
            }

            foreach (GameWindow window in windows)
            {
                window.UnloadGraphicsContent(unloadAllContent);
            }
        }

        public String LoadProfileMode = "Load";

        public void StartLoadNewProfile(int Number, bool DisplayMessage)
        {
            LoadNewProfile = true;
            GamerNumToLoad = Number;
            LoadProfileMode = "Preload";
            WindowManagerTextBoxes.textboxes  = new List<TextBox>();
            this.DisplayMessage = DisplayMessage;
        }


        public void LoadNewProfileUpdate()
        {
            if (LoadProfileMode == "Preload")
            {
                bool CreateProfile = false;
                if (Gamer.SignedInGamers.Count == 0) CreateProfile = true;
                if (CurrentGamer != null)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (GamePlayers[i] != null)
                        {
                            if (CurrentGamer.Equals(GamePlayers[i].Gamer))
                            {
                                CreateProfile = true;
                            }
                        }
                    }
                }
                if (CreateProfile == true)
                {
                    if (DisplayMessage == true)
                    {
                        WindowManagerTextBoxes.AddTextBox(new TextBox("Load", "Please select/Create a profile to use\nPress Enter to Continue", new Vector2(400, 300), 0, 5, true));
                        DisplayMessage = false;
                    }
                    if (WindowManagerTextBoxes.textboxes.Count == 0)
                    {
                        LoadProfileMode = "Load";
                        if (Guide.IsVisible == false) Guide.ShowSignIn(1, false);
                    }
                }
                else
                {

                    GamePlayer newplayer = new GamePlayer();
                    newplayer.Gamer = Gamer.SignedInGamers[0];
                    GamePlayers[GamerNumToLoad] = newplayer;
                    LoadProfileMode = "CheckGameProfile";
                }
            }

            if (LoadProfileMode == "Load")
            {
                bool AlreadyExists = false;
                if (Gamer.SignedInGamers.Count > 0)
                {
                    SignedInGamer newgamer = Gamer.SignedInGamers[0];
                    foreach (GamePlayer g in GamePlayers)
                    {
                        if (g !=null && g.Gamer.Equals(newgamer))
                        {
                            AlreadyExists = true;
                        }
                    }
                }
                else
                {
                    AlreadyExists = true;
                }   
                if (AlreadyExists)
                {
                    if (Guide.IsVisible == false) Guide.ShowSignIn(1, false);
                }
                else
                {
                    LoadProfileMode = "Preload";
                }
            }
            if (LoadProfileMode == "CheckGameProfile")
            {
                if(File.Exists(GamePlayers[GamerNumToLoad].Gamer.ToString()+".PROFILE"))
                {
                    TextReader reader = new StreamReader(GamePlayers[GamerNumToLoad].Gamer.ToString()+".PROFILE");
                    XmlSerializer serializer = new XmlSerializer(typeof(ProfileSaveData));
                    
                    GamePlayers[GamerNumToLoad] = ProfileSaveData.SaveDatatoPlayer(GamePlayers[GamerNumToLoad],(ProfileSaveData)serializer.Deserialize(reader));
                 
                    /*
                    GamePlayers[GamerNumToLoad].LeftKey = GetKeyFromString(reader.ReadLine());
                    GamePlayers[GamerNumToLoad].RightKey = GetKeyFromString(reader.ReadLine());
                    GamePlayers[GamerNumToLoad].UpKey = GetKeyFromString(reader.ReadLine());
                    GamePlayers[GamerNumToLoad].DownKey = GetKeyFromString(reader.ReadLine());
                    GamePlayers[GamerNumToLoad].EnterKey = GetKeyFromString(reader.ReadLine());*/
                    LoadProfileMode = "Finish";
                }
                else
                {
                   
                    LoadProfileMode = "SetUp";

                    WindowManagerTextBoxes.AddTextBox(new TextBox("Controls", "Controls For " + GamePlayers[GamerNumToLoad].Gamer.ToString(), new Vector2(400, 100), 200, 5, true, "None"));
                    WindowManagerTextBoxes.AddTextBox(new TextBox("Move Right", "MoveRight", new Vector2(400, 120), 200, 5, true, "None"));
                    WindowManagerTextBoxes.AddTextBox(new TextBox("Move Left", "MoveLeft", new Vector2(400, 140), 200, 5, true, "None"));
                    WindowManagerTextBoxes.AddTextBox(new TextBox("Move Up", "MoveUp", new Vector2(400, 160), 200, 5, true, "None"));
                    WindowManagerTextBoxes.AddTextBox(new TextBox("Move Down", "Down", new Vector2(400, 180), 200, 5, true, "None"));
                    WindowManagerTextBoxes.AddTextBox(new TextBox("Enter", "Action Button", new Vector2(400, 200), 200, 5, true, "None"));
                    WindowManagerTextBoxes.AddTextBox(new TextBox("Save", "Save", new Vector2(400, 220), 200, 5, true, "None"));

                    WindowManagerTextBoxes.AddTextBox(new TextBox("Instructions", "Instructions Go Here", new Vector2(400, 500), 200, 5, true, "None"));
                    EditThisItem = false;
                    SelectedItem = 1;
                    KeysForPlayer = new Keys[WindowManagerTextBoxes.textboxes.Count - 1];

                }
            }
            if (LoadProfileMode == "SetUp")
            {
                foreach(TextBox T in WindowManagerTextBoxes.textboxes)
                {
                    T.setColor(Color.White);
                }
                WindowManagerTextBoxes.textboxes[1].Initialize("Right : "+KeysForPlayer[1], new Vector2(400, 120), 200, Color.White);
                WindowManagerTextBoxes.textboxes[2].Initialize("Left : " + KeysForPlayer[2], new Vector2(400, 140), 200, Color.White);
                WindowManagerTextBoxes.textboxes[3].Initialize("Up : " + KeysForPlayer[3], new Vector2(400, 160), 200, Color.White);
                WindowManagerTextBoxes.textboxes[4].Initialize("Down : " + KeysForPlayer[4], new Vector2(400, 180), 200, Color.White);
                WindowManagerTextBoxes.textboxes[5].Initialize("Action Button : " + KeysForPlayer[5], new Vector2(400, 200), 200, Color.White);

                if (EditThisItem == false)
                {
                    WindowManagerTextBoxes.textboxes[SelectedItem].setColor(Color.CadetBlue);
                    if (NewState.IsKeyDown(Keys.Down) && OldState.IsKeyUp(Keys.Down)) SelectedItem++;
                    if (NewState.IsKeyDown(Keys.Up) && OldState.IsKeyUp(Keys.Up)) SelectedItem--;
                    if (NewState.IsKeyDown(Keys.Enter) && OldState.IsKeyUp(Keys.Enter))
                    {

                        if (SelectedItem >= WindowManagerTextBoxes.textboxes.Count - 2)
                        {
                            LoadProfileMode = "Save";
                            GamePlayers[0].LeftKey = KeysForPlayer[2];
                            GamePlayers[0].RightKey = KeysForPlayer[1];
                            GamePlayers[0].UpKey = KeysForPlayer[3];
                            GamePlayers[0].DownKey = KeysForPlayer[4];
                            GamePlayers[0].EnterKey = KeysForPlayer[5];
                        }
                        else
                        {
                            EditThisItem = true;
                        }
                    }
                    if (SelectedItem == 0) SelectedItem = WindowManagerTextBoxes.textboxes.Count-2;
                    if (SelectedItem == WindowManagerTextBoxes.textboxes.Count-1) SelectedItem = 1;
                }
                else
                {
                    WindowManagerTextBoxes.textboxes[SelectedItem].setColor(Color.Firebrick);
                    if (NewState.GetPressedKeys().Length > 0)
                    {
                        Keys pressedKey = NewState.GetPressedKeys()[0];
                        if (OldState.IsKeyUp(pressedKey))
                        {
                            KeysForPlayer[SelectedItem] = pressedKey;
                            EditThisItem = false;
                        }
                    }
                  
                    
                }
            }
            if (LoadProfileMode == "Save")
            {
                //File.Create(GamePlayers[0].Gamer.ToString() + ".PROFILE");
                XmlSerializer serializer = new XmlSerializer(typeof(ProfileSaveData));
                
                TextWriter profilewriter = new StreamWriter(GamePlayers[GamerNumToLoad].Gamer.ToString() + ".PROFILE");
                
                serializer.Serialize(profilewriter,new ProfileSaveData(GamePlayers[GamerNumToLoad]));
                
    
                foreach (TextBox t in WindowManagerTextBoxes.textboxes) t.Mode = "Die";
                LoadProfileMode = "Finish";
                profilewriter.Close();
            }
            if (LoadProfileMode == "Finish")
            {
                GamePlayers[GamerNumToLoad].Loaded = true;
                if (WindowManagerTextBoxes.textboxes.Count == 0)
                {
                    WindowManagerTextBoxes.AddTextBox(new TextBox("Completed", "Profile "+GamePlayers[GamerNumToLoad].Gamer.ToString()+" was Loaded Successfully\nPress Enter to Continue", new Vector2(400, 300), 0, 5, true));
                }
                if (WindowManagerTextBoxes.textboxes[0].Name == "Completed" && WindowManagerTextBoxes.textboxes[0].Mode == "Die")
                {
                    LoadNewProfile = false;
                }
            }
        }

      


        public void LoadNewProfileDraw()
        {
            spriteBatch.Begin();
            foreach (TextBox t in WindowManagerTextBoxes.textboxes)
            {
                t.Draw(spriteBatch, gameFont);
            }
            spriteBatch.End();
        }


        public override void Update(GameTime gameTime)
        {
            //Check The Keyboard First
            NewState = Keyboard.GetState();

            if (LoadNewProfile) LoadNewProfileUpdate();
            WindowManagerTextBoxes.Update(gameTime.ElapsedGameTime, NewState);
            //Make sure the Windows Live Guide isn't displaying a message
            if (Guide.IsVisible == false && LoadNewProfile == false)
            {
               
                //Check to see who is the CurrentGamer
                if (Gamer.SignedInGamers.Count > 0) CurrentGamer = Gamer.SignedInGamers[0];

                //Figure out what windows to update

                windowstoUpdate.Clear();

                foreach (GameWindow window in windows)
                {
                    windowstoUpdate.Add(window);
                }

                //Update Each Window
                while (windowstoUpdate.Count > 0)
                {
                    GameWindow window = windowstoUpdate[windowstoUpdate.Count - 1];
                    windowstoUpdate.RemoveAt(windowstoUpdate.Count - 1);

                    window.Update(gameTime);
                }

               
            }
            //Save the Keyboard State
            OldState = NewState;
        }

        public override void Draw(GameTime gameTime)
        {
            graphicsDeviceService.GraphicsDevice.Clear(Color.Black);
            //spriteBatch.Begin();
            foreach(GameWindow window in windows)
            {
                window.Draw(gameTime);
                
            }
            if (LoadNewProfile) LoadNewProfileDraw();
            //spriteBatch.End();
        }


        public void AddScreen(GameWindow window)
        {
            window.WindowManager = this;

            if ((graphicsDeviceService != null) && (graphicsDeviceService.GraphicsDevice != null))
                window.LoadGraphicsContent(true);


            windows.Add(window);
            window.Init();
        }

        public void removeScreen(GameWindow window)
        {
            windows.Remove(window);
            window = null;
            GC.Collect();
            //GC.WaitForPendingFinalizers();
        }
        
        /// <summary>
        /// Finds if a Specific Window Exists
        /// </summary>
        /// <param name="Lookup">Name of Window to Look up (ToString)</param>
        /// <returns></returns>
        public Boolean windowExist(String Lookup)
        {
            Boolean exist = false;
            List<GameWindow> L = new List<GameWindow>();
            foreach(GameWindow w in windows)
            {
                L.Add(w);
            }
            while (exist == false && L.Count > 0)
            {
                if (L[L.Count - 1].ToString() == Lookup) exist = true;
                L.RemoveAt(L.Count - 1);
            }

            return exist;
        }

        /// <summary>
        /// Finds a Specific Window then Returns the Window
        /// </summary>
        /// <param name="Lookup">The Name of the Window to look up</param>
        /// <returns></returns>
        public GameWindow findWindow(String Lookup)
        {
            GameWindow exist = null;
            List<GameWindow> L = new List<GameWindow>();
            foreach (GameWindow w in windows)
            {
                L.Add(w);
            }
            while (exist == null && L.Count > 0)
            {
                if (L[L.Count - 1].ToString() == Lookup) exist = L[L.Count - 1];
                L.RemoveAt(L.Count - 1);
            }

            return exist;
        }

        


        


    }
}
