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
    class Win : GameWindow
    {
        SpriteFont gameFont;
        ContentManager content;




        String RunMode = "Start";


        //String Mode;



        TextboxManager textManager;


        public Win()
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
            textManager.AddTextBox(new TextBox("LOSE", "You Fail", new Vector2(1024 / 2, 768 / 2), 50, 5, true));
            Mode = "Run";
        }

        public void Run(GameTime gametime)
        {
            if (textManager.textboxes.Count == 0)
            {
                WindowManager.AddScreen(new Gameplay(WindowManager.CameraPosition, WindowManager.AspectRatio));
                WindowManager.removeScreen(this);
            }





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


            WindowManager.SpriteBatch.Begin();

            textManager.Draw(WindowManager.SpriteBatch, gameFont);

            WindowManager.SpriteBatch.End();


        }




    }
}
