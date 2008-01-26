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
using System.Collections;
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

        //This is the Swarm Manager, Enemies need to be created then added into a swarm, the swarm must then me put into
        //this manager. The manger then updates everything and keeps it all working.
        SwarmManager SwarmManager = new SwarmManager();

        //Create the First player object;
        List<Player> PlayerList;
        
        //Create the Player Model which is used by the player class(es)
        Model PlayerModel;
        
        //Create the Bullet and the Bullet Handler
        Model BulletModel;
        Model EnemyModel;
        Model CollectItemModel;

        Texture2D bg;

        List<Bullet> BulletClass = new List<Bullet>();

        //Create the Swarm and the Swarm handlers
        Swarm Swarm;
        List<Enemy> enemies = new List<Enemy>();

        //Position of the Camera in world space, for our view matrix
        static float CameraY = 180.0f;
        static Vector3 cameraPosition;

        //Aspect ratio to use for the projection matrix
        static float aspectRatio;

        int numExplosionParticles;

        //A Text Manager so we can display text to the screen in a cool fashion
        TextboxManager textManager;

        BulletManager BulletManager = new BulletManager();

        //CollectablesManager CollectManager = new CollectablesManager();
        
        public Gameplay(Vector3 CP, float ar)
        {
            Mode = "Run";
            textManager = new TextboxManager();
            cameraPosition = CP;
            aspectRatio = ar;
        }

        public override void Init()
        {
           
        }

        public void Initialize()
        {
            //Create 3 New Players
            Player Player1 = new Player(new Vector2(10, 0), Keys.D1, 2.5f);
           // Player Player2 = new Player(new Vector2(550, 0), Keys.D2, 2.5f);
            //Player Player3 = new Player(new Vector2(10, 450), Keys.D3, 2.5f);
            Player1.Type = 1;
            //Player2.Type = 2;
            //Player3.Type = 3;
            //Initialize the List that holds all the players
            PlayerList = new List<Player>();
            //Insert the players into the list for later access
            PlayerList.Add(Player1);
            //PlayerList.Add(Player2);
            //PlayerList.Add(Player3);
            int i = 0;
            foreach (Player p in PlayerList)
            {
                p.LinkedProfile = WindowManager.GamePlayers[i];
                i++;
            }

            numExplosionParticles = 20;
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
                BulletModel = content.Load<Model>("Models\\bullet");
                EnemyModel = content.Load<Model>("Models\\cone");
                bg = content.Load<Texture2D>("Content\\earth_night");
                //CollectItemModel = content.Load<Model>("Models\\collect");
            }
            Random RANDOM = new Random();
            for (int j = 0; j < 7; j++)
            {
                Vector2 position = new Vector2(RANDOM.Next(-500, 500), RANDOM.Next(-500, 500));
                List<Enemy> EnemyList = new List<Enemy>();

                int count = 0;
                for (int i = 0; i < 15; i++)
                {
                    EnemyList.Add(new Enemy(position + new Vector2(0f, (float)(count * 1.1f)), Vector2.Zero, .001f, EnemyModel));
                    count++;
                }

                Swarm = new Swarm(position, Vector2.Zero, EnemyList);

                SwarmManager.addSwarm(Swarm);
            }
            Swarm newswarm = SwarmManager.getSwarm(0);
            newswarm.moveSwarm(newswarm.Position + new Vector2(0, 50));
            
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
            if (WindowManager.NewState.IsKeyDown(Keys.Escape))
                WindowManager.Game.Exit();

            if (Mode == "Run") Run(gametime);


        }

        public void Run(GameTime gameTime)
        {
            //Updates the Keyboard
            KeyboardState KeyState = WindowManager.NewState;
            KeyboardState OldState = WindowManager.OldState;
            bool KeyReleased;
             //Update the Keyboard
            if (OldState.GetPressedKeys().Length == 0)
            {
                KeyReleased = true;
            }
            else
            {
                KeyReleased = false;
            }

            //Escape Provision

            

            //Allows Player 1 to Respawn himself (Needs update/simplification)
            if (WindowManager.NewState.IsKeyDown(Keys.D1) && KeyReleased != false)
            {
                Player Player1 = PlayerList[0];
                Collision.AllGamePlayObjects.Remove(Player1);
                Player1 = new Player(new Vector2(10, 0), Keys.D1, 2.5f);
                Player1.Model = PlayerModel;
                Player1.Type = 1;
            }
           
            //Allows Camera to zoom in and out
            if (WindowManager.NewState.IsKeyDown(Keys.D9))
                CameraY -= 5;
            if (WindowManager.NewState.IsKeyDown(Keys.D0))
                CameraY += 5;

            //Sets the CameraY
            cameraPosition.Y = CameraY;
            WindowManager.cameraPosition.Y = cameraPosition.Y;
            
            //Updates Each Player
            foreach (Player p in PlayerList)
            {
                p.Update(WindowManager.NewState, KeyReleased, gameTime);
                if (p.getReadyState() == 6)
                    if (gameTime.TotalGameTime.CompareTo(p.OnFire) > 0)
                    {
                        for (int i = 0; i < 1; i++)
                            WindowManager.smokeParticle.AddParticle(Vector3FromVector2(p.Position), Vector3FromVector2(p.Velocity));//.explosionParticle.AddParticle(new Vector3(p.Position.X, 1f, p.Position.Y), Vector3FromVector2(p.Velocity));

                        p.OnFire = gameTime.TotalGameTime + new TimeSpan(0, 0, 0, 0, 100);
                    }
            }

            if (PlayerList[0].getReadyState() == 6)
            {
                cameraPosition = new Vector3(PlayerList[0].Position.X, CameraY, PlayerList[0].Position.Y - .1f);
            }

            

            if (KeyState.IsKeyDown(Keys.F1)&&!OldState.IsKeyDown(Keys.F1))
            {
                
            }
            //Updates the Swarm
            SwarmManager.Update(gameTime, PlayerList, BulletManager);
           

            if (WindowManager.NewState.IsKeyDown(Keys.Space) && WindowManager.OldState.IsKeyUp(Keys.Space))
            {
                for (int i = 0; i < 20; i++)
                    WindowManager.ExplosionParticleSystem.AddParticle(Vector3.Zero, Vector3.Zero);
            }

            //Updates the Collision Manager
            CollisionManager.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds);
            BulletManager.Update(gameTime, PlayerList[0]);
            //CollectManager.Update(gameTime);
            //Removes Bullets that are too far out of the screen (this needs to be moved somewhere else!)
            /*for (int i = BulletClass.Count - 1; i >= 0; i--)
            {

                
                if (BulletClass[i].killme == true)
                {
                    Bullet O = BulletClass[i];
                    if (isOnScreen(O.Position))
                    {
                        for (int x = 0; x < numExplosionParticles; x++)
                            WindowManager.ExplosionParticleSystem.AddParticle(new Vector3(O.Position.X, 0f, O.Position.Y), new Vector3(O.Velocity.X, 0f, O.Velocity.Y));
                    }
                    BulletClass.Remove(O);
                    O = null;

                }
            }*/
           
            /*if (KeyState.IsKeyDown(Keys.Left))
            {
                newswarm.moveSwarm(newswarm.Position + new Vector2(-1, 0));
            }
            if (KeyState.IsKeyDown(Keys.Right))
            {
                newswarm.moveSwarm(newswarm.Position + new Vector2(1, 0));
            }*/
            if (NewState.IsKeyDown(Keys.A)) screenadder += .1f;
            if (NewState.IsKeyDown(Keys.B)) screenadder -= .1f;
            base.Update(gameTime);
           
        }

        public override void Draw(GameTime gameTime)
        {
            WindowManager.SpriteBatch.Begin();
            WindowManager.SpriteBatch.Draw(bg,-(new Vector2(bg.Width,bg.Height)/2)- new Vector2(cameraPosition.X,cameraPosition.Z)/2, Color.White);
            WindowManager.SpriteBatch.End();

            //Draw Each Bullet onto the screen
            BulletManager.Draw(BulletModel, cameraPosition, aspectRatio);

            //Draw Each Enemy onto the screen
            SwarmManager.DrawSwarms(cameraPosition, aspectRatio);

            //Draw each player onto the screen
            foreach (Player p in PlayerList)
            {
                p.Draw(cameraPosition, aspectRatio, gameFont, WindowManager.SpriteBatch);
            }

            WindowManager.SpriteBatch.Begin();
            WindowManager.SpriteBatch.DrawString(gameFont, Collision.AllGamePlayObjects.Count.ToString(), new Vector2(0, 100), Color.White);
            WindowManager.SpriteBatch.End();

            //CollectManager.Draw(cameraPosition, aspectRatio);

            Model model = EnemyModel;

            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            //Draw the model, a model can have multiple meshes, so loop
            foreach (ModelMesh mesh in model.Meshes)
            {


                //This is where the mesh orientation is set, as well as our camera and projection
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index]
                     * Matrix.CreateTranslation(Vector3.Zero)
                     * Matrix.CreateScale(1.0f);

                    effect.View = Matrix.CreateLookAt(cameraPosition, new Vector3(cameraPosition.X, 0f, cameraPosition.Z - .1f), new Vector3(0, 1, 0));
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 10000.0f);
                }

                Matrix view = Matrix.CreateLookAt(cameraPosition, new Vector3(cameraPosition.X, 0f, cameraPosition.Z - .1f), new Vector3(0, 1, 0));
                Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 10000.0f);

                WindowManager.explosionParticle.SetCamera(view, projection);
                WindowManager.smokeParticle.SetCamera(view, projection);
                WindowManager.fireParticle.SetCamera(view, projection);

                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }

            WindowManager.SpriteBatch.Begin();
            WindowManager.SpriteBatch.DrawString(gameFont, CameraY.ToString()+Environment.NewLine+screenadder.ToString(), new Vector2(0, 40), Color.White);
            WindowManager.SpriteBatch.DrawString(gameFont, PlayerList[0].Position.ToString(), new Vector2(0, 200), Color.White);
            WindowManager.SpriteBatch.End();

        }
        //static float screenadder = 55;
        static float screenadder = 95;
        public static bool isOnScreen(Vector2 pos)
        {
            
            if (pos.X > cameraPosition.X + screenadder || pos.Y > cameraPosition.Z + screenadder || pos.X < cameraPosition.X - screenadder || pos.Y < cameraPosition.Z - screenadder)
                return false;
            else
                return true;
        }

        public Vector3 Vector3FromVector2(Vector2 vec2)
        {
            Vector3 vec3 = new Vector3(vec2.X, 0f, vec2.Y);
            return vec3;
        }
    }
}
