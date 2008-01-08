#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;
using XNAExtras;
#endregion

namespace Tanks
{
    class Gameplay : GameWindow
    {
        SpriteFont gameFont;
        ContentManager content;

        


   


        //String Mode;

        //This is the Collision Manager that handles doing any collision and letting the objects have permission to move
        Collision CollisionManager = new Collision();

        //Create the First player object;
        List<Player> PlayerList;
        
        //Create the Player Model which is used by the player class(es)
        Model PlayerModel;
        
        //Create the Bullet and the Bullet Handler
        Model BulletModel;
        Model EnemyModel;
        List<Bullet> BulletClass = new List<Bullet>();

        //Create the Swarm and the Swarm handlers
        Swarm Swarm;
        List<Enemy> enemies = new List<Enemy>();

        //Position of the Camera in world space, for our view matrix
        static float CameraY = 80.0f;
        static Vector3 cameraPosition = new Vector3(0.0f, 80.0f, .1f);

        //Aspect ratio to use for the projection matrix
        static float aspectRatio = 640.0f / 480.0f;


        //A Text Manager so we can display text to the screen in a cool fashion
        TextboxManager textManager;

        public Gameplay()
        {
            Mode = "Run";
            textManager = new TextboxManager();
        }

        public override void Init()
        {
           
        }

        public void Initialize()
        {
            //Create 3 New Players
            Player Player1 = new Player(new Vector2(10, 0), Keys.D1, 2.5f);
            Player Player2 = new Player(new Vector2(550, 0), Keys.D2, 2.5f);
            Player Player3 = new Player(new Vector2(10, 450), Keys.D3, 2.5f);
            Player1.Type = 1;
            Player2.Type = 2;
            Player3.Type = 3;
            //Initialize the List that holds all the players
            PlayerList = new List<Player>();
            //Insert the players into the list for later access
            PlayerList.Add(Player1);
            PlayerList.Add(Player2);
            PlayerList.Add(Player3);
            int i = 0;
            foreach (Player p in PlayerList)
            {
                p.LinkedProfile = WindowManager.GamePlayers[i];
                i++;
            }
        }

        public override void LoadGraphicsContent(bool LoadAllContent)
        {
            Initialize();
            if (LoadAllContent)
            {
                if (content == null) content = new ContentManager(WindowManager.Game.Services);

                gameFont = content.Load<SpriteFont>("Content\\SpriteFont1");
                foreach (Player p in PlayerList)
                {
                    p.Model = content.Load<Model>("Models\\Tank");
                }
                BulletModel = content.Load<Model>("Models\\Sphere");
                EnemyModel = content.Load<Model>("Models\\cone");



            }
            int count = 0;
            for (int i = 0; i < 10; i++)
            {
                enemies.Add(new Enemy(new Vector2(20f, (float)(count * 1.1f)), Vector2.Zero, .001f, EnemyModel));
                count++;
            }

            Swarm = new Swarm(new Vector2(0f, 0f), Vector2.Zero, enemies);
            
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


        }

        public void Run(GameTime gameTime)
        {
            //Updates the Keyboard
            KeyboardState KeyState = WindowManager.NewState;
            bool KeyReleased;
             //Update the Keyboard
            if (KeyState.GetPressedKeys().Length == 0)
            {
                KeyReleased = true;
            }
            else
            {
                KeyReleased = false;
            }

            //Escape Provision
            
            //Allows Player 1 to Respawn himself (Needs update/simplification)
            if (KeyState.IsKeyDown(Keys.D1) && PlayerList[0].getReadyState() == 6)
            {
                Player Player1 = PlayerList[0];
                Collision.AllGamePlayObjects.Remove(Player1);
                Player1 = new Player(new Vector2(10, 0), Keys.D1, 2.5f);
                Player1.Model = PlayerModel;
                Player1.Type = 1;
            }
           
            //Allows Camera to zoom in and out
            if (KeyState.IsKeyDown(Keys.Subtract))
                CameraY -= 5;
            if (KeyState.IsKeyDown(Keys.D0))
                CameraY += 5;

            //Sets the CameraY
            cameraPosition.Y = CameraY;
            
            //Updates Each Player
            foreach (Player p in PlayerList)
            {
                p.Update(KeyState, KeyReleased, gameTime, CollisionManager, BulletClass);
            }

            //Updates the Swarm
            Swarm.Update(gameTime, PlayerList);

            //Updates the Enemies
            foreach (Enemy e in enemies)
                e.Update(gameTime, CollisionManager, PlayerList);

            //Updates the Collision Manager
            CollisionManager.Update(gameTime.ElapsedGameTime.Milliseconds);

            //Removes Bullets that are too far out of the screen (this needs to be moved somewhere else!)
            for (int i = 0; i < BulletClass.Count; ++i)
            {

                if (Math.Abs(BulletClass[i].Position.X) > 500f || Math.Abs(BulletClass[i].Position.Y) > 500f)
                {
                    Collision.KillList.Add(BulletClass[i]);
                    BulletClass[i].killme = true;
                }
                if (BulletClass[i].killme == true)
                {
                    Bullet O = BulletClass[i];
                    BulletClass.Remove(O);
                    O = null;

                }
            }
         

            base.Update(gameTime);
           
        }

        public override void Draw(GameTime gameTime)
        {
            //Draw Each Bullet onto the screen
            foreach (Bullet x in BulletClass)
            {
                if (x != null) x.Draw(BulletModel, cameraPosition, aspectRatio);
            }

            //Draw Each Enemy onto the screen
            foreach (Enemy e in enemies)
                e.Draw(cameraPosition, aspectRatio);

            //Draw each player onto the screen
            foreach (Player p in PlayerList)
            {
                p.Draw(cameraPosition, aspectRatio, gameFont, WindowManager.SpriteBatch);
            }

           
        }


        
    }
}
