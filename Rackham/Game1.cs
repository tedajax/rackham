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
//Lolololol
namespace Tanks
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>5
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// Changing Stuff This is fun
        /// </summary>
        GraphicsDeviceManager graphics;
        ContentManager content;

        BitmapFont Times;
        
        Collision CollisionManager = new Collision();

        //Create the First player object;
        Player Player1 = new Player(new Vector2(10, 0), Keys.D1, 2.5f);
        Player Player2 = new Player(new Vector2(550,0), Keys.D2, 2.5f);
        Player Player3 = new Player(new Vector2(10, 450), Keys.D3, 2.5f);
        //Create the ground
        Model BulletModel;
        Model EnemyModel;
        List<Bullet> BulletClass = new List<Bullet>();

        Swarm Swarm;
        List<Enemy> enemies = new List<Enemy>();
        
        //Position of the Camera in world space, for our view matrix
        float CameraY = 80.0f;
        Vector3 cameraPosition = new Vector3(0.0f, 80.0f, .1f);

        //Aspect ratio to use for the projection matrix
        float aspectRatio = 640.0f / 480.0f;

        //KeyboardStates
        KeyboardState KeyState;
        bool KeyReleased;
        WindowManager windowManager;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            content = new ContentManager(Services);

            Components.Add(new GamerServicesComponent(this));

            windowManager = new WindowManager(this);


            Components.Add(windowManager);

            windowManager.AddScreen(new TitleScreen(false));

            
           
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            KeyState = Keyboard.GetState();
            Times = new BitmapFont("Content/newfont.xml");
            Player1.Type = 1;
            Player2.Type = 2;
            Player3.Type = 3;
            base.Initialize();

        }

        Model PlayerModel;
        /// <summary>
        /// Load your graphics content.  If loadAllContent is true, you should
        /// load content from both ResourceManagementMode pools.  Otherwise, just
        /// load ResourceManagementMode.Manual content.
        /// </summary>
        /// <param name="loadAllContent">Which type of content to load.</param>
        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            if (loadAllContent)
            {
                PlayerModel = content.Load<Model>("Models\\Tank");
                Player1.Model = PlayerModel;
                Player2.Model = Player1.Model;
                Player3.Model = Player1.Model;
                BulletModel = content.Load<Model>("Models\\Sphere");
                EnemyModel = content.Load<Model>("Models\\cone");
                // TODO: Load any ResourceManagementMode.Automatic content
            }
            Times.Reset(graphics.GraphicsDevice);
            Times.KernEnable = false;
            Times.TextColor = Color.White;


            int count = 0;
            for (int i = 0; i < 10; i++)
            {
                enemies.Add(new Enemy(new Vector2(20f, (float)(count * 1.1f)), Vector2.Zero, .001f, EnemyModel));
                //enemies[i].Target = new Vector2(20f, 0f);
                count++;
            }

            Swarm = new Swarm(new Vector2(0f, 0f), Vector2.Zero, enemies);
            
            // TODO: Load any ResourceManagementMode.Manual content
        }


        /// <summary>
        /// Unload your graphics content.  If unloadAllContent is true, you should
        /// unload content from both ResourceManagementMode pools.  Otherwise, just
        /// unload ResourceManagementMode.Manual content.  Manual content will get
        /// Disposed by the GraphicsDevice during a Reset.
        /// </summary>
        /// <param name="unloadAllContent">Which type of content to unload.</param>
        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            if (unloadAllContent == true)
            {
                content.Unload();
            }
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the default game to exit on Xbox 360 and Windows
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            /* //CameraY = MathHelper.SmoothStep(CameraY, 80f, .05f);
             cameraPosition.Y = CameraY;
             KeyState = Keyboard.GetState();

             if (KeyState.IsKeyDown(Keys.Escape))
                 this.Exit();
            
             if (KeyState.IsKeyDown(Keys.D1) && Player1.getReadyState() ==6)
             {
                 Collision.AllGamePlayObjects.Remove(Player1);
                 Player1 = new Player(new Vector2(10, 0), Keys.D1, 2.5f);
                 Player1.Model = PlayerModel;
                 Player1.Type = 1;
             }
             if (KeyState.IsKeyDown(Keys.D2) && Player2.getReadyState() == 6)
             {
                 Collision.AllGamePlayObjects.Remove(Player2);
                 Player2 = new Player(new Vector2(550, 0), Keys.D2, 2.5f);
                 Player2.Model = PlayerModel;
                 Player2.Type = 2;
             }
            
             if (KeyState.IsKeyDown(Keys.Subtract))
                 CameraY -= 5;
             if (KeyState.IsKeyDown(Keys.D0))
                 CameraY += 5;

             Player1.Update(KeyState, KeyReleased, gameTime, CollisionManager, BulletClass);
             Player2.Update(KeyState, KeyReleased, gameTime, CollisionManager, BulletClass);
             Player3.Update(KeyState, KeyReleased, gameTime, CollisionManager, BulletClass);

             List<Player> PlayerList = new List<Player>();
             PlayerList.Add(Player1);
             PlayerList.Add(Player2);
             PlayerList.Add(Player3);

             foreach (Enemy e in enemies)
                 e.Update(gameTime, CollisionManager, PlayerList);

             CollisionManager.Update(gameTime.ElapsedGameTime.Milliseconds);

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
             Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 10000.0f);Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 10000.0f);
             Matrix View = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, new Vector3(0, 1, 0));
             Matrix World = Matrix.CreateTranslation(0, 0, 0);

             Vector3 v;
             Vector2 mouseLoc = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
             v.X = (((2.0f * mouseLoc.X) / 800) - 1) /Projection.M11;
             v.Y = -(((2.0f * mouseLoc.Y) / 600) - 1) / Projection.M22;
             v.Z = 1.0f;

             Matrix m = View * World; 
            



             Swarm.Position = new Vector2(m.M41,m.M42);
             Swarm.Update(gameTime, PlayerList);
          
 
             base.Update(gameTime);
             //Update the Keyboard
             if (KeyState.GetPressedKeys().Length == 0)
             {
                 KeyReleased = true;
             }
             else
             {
                 KeyReleased = false;
             }
             * */
            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            /*graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            //Draw the ground
            //DrawModel(Ground, cameraPosition, aspectRatio);
            //Copy any parent transforms
            foreach (Bullet x in BulletClass)
            {
                if (x!=null) x.Draw(BulletModel, cameraPosition, aspectRatio, Times);
            }


            Times.DrawString(0, 300, Swarm.Position.X.ToString());
            Times.DrawString(0, 310, Swarm.Position.Y.ToString());

            foreach (Enemy e in enemies)
                e.Draw(cameraPosition, aspectRatio);
            Player1.Draw(cameraPosition, aspectRatio, Times);
            Player2.Draw(cameraPosition, aspectRatio, Times);
            Player3.Draw(cameraPosition, aspectRatio, Times);*/
            base.Draw(gameTime);
        }
        protected void DrawModel(Model Model, Vector3 Camera, float aspectratio)
        {
            Matrix[] transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);
            //Draw the model, a model can have multiple meshes, so loop
            foreach (ModelMesh mesh in Model.Meshes)
            {


                //This is where the mesh orientation is set, as well as our camera and projection
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index]
                     * Matrix.CreateTranslation(Vector3.Zero)
                     * Matrix.CreateScale(new Vector3(13.8f, 0.0f, 10.5f));

                    effect.View = Matrix.CreateLookAt(Camera, Vector3.Zero, new Vector3(0, 1, 0));
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 10000.0f);
                }
                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }
        }

    }
}