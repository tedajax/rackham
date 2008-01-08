#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace Tanks
{
    class TitleScreen : GameWindow
    {
        SpriteFont gameFont;
        ContentManager content;


        
        
        String RunMode = "Start";
        

        //String Mode;



        TextboxManager textManager;
       

        public TitleScreen(Boolean name)
        {
            Mode = "Load";
         
            textManager = new TextboxManager();
            
        }


        public override void LoadGraphicsContent(bool loadAllContent)
        {
            
            if (loadAllContent)
            {
                if (content == null) content = new ContentManager(WindowManager.Game.Services);

                gameFont = content.Load<SpriteFont>("Content\\SpriteFont1");

              
                

            }
        }

        public override void UnloadGraphicsContent(bool unloadAllContent)
        {
            if (unloadAllContent)
            {
                content.Unload();
            }
           
        }
        public override void Update(GameTime gametime)
        {
            if (Mode == "Run") Run(gametime);
            if (Mode == "Load") Load(gametime);
            if (Mode == "Die") Die(gametime);

            
        }

        public void Load(GameTime gametime)
        {
            int i = 0;
            RunMode = "MainRun";
            WindowManager.StartLoadNewProfile(0, true);
             if (i == 0) Mode = "Run";
            
        }

        public void Run(GameTime gametime)
        {
            if (textManager.textboxes.Count == 0)
            {
                WindowManager.AddScreen(new Gameplay());
                WindowManager.removeScreen(this);
            }
          
            /*if (RunMode == "Start")
            {
                if (WindowManager.GamePlayers.Count==0)
                {
                    if (textManager.textboxes.Count==0)
                    {
                        WindowManager.StartLoadNewProfile();
                    }

                }
                else
                {
                    if (textManager.textboxes[0].ToString() == "FindProfile")
                    {
                        textManager.textboxes[0].Mode = "Die";
                    }
                    if (textManager.textboxes.Count == 0)
                    {
                        Mode = "MainRun";
                    }
                }
            }*/
            
            
            //WindowManager.NewState = Keyboard.GetState();
            

            textManager.Update(gametime.ElapsedGameTime, WindowManager.NewState);
           // WindowManager.OldState = WindowManager.NewState;

        }

        public void Die(GameTime gametime)
        {
            int i = 0;
            
            if (i == 0)
            {
                UnloadGraphicsContent(true);
                WindowManager.removeScreen(this);
            }

        }
                

        public override void Draw(GameTime gameTime)
        {
           // WindowManager.GraphicsDevice.Clear(Color.Black);


            //DrawModel(TestModel);

            

            WindowManager.SpriteBatch.Begin();

            textManager.Draw(WindowManager.SpriteBatch, gameFont);
                        
            WindowManager.SpriteBatch.End();

            
        }


   

    }
}
