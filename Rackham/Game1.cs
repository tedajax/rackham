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

        public static ParticleSystem explosionParticle;
        public static ParticleSystem fireParticle;
        public static ParticleSystem smokeParticle;

        BloomComponent bloom;

        WindowManager windowManager;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.MinimumPixelShaderProfile = ShaderProfile.PS_2_0;
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            content = new ContentManager(Services);

            Components.Add(new GamerServicesComponent(this));

            explosionParticle = new ExplosionParticleSystem(this, content);
            fireParticle = new FireParticleSystem(this, content);
            smokeParticle = new SmokePlumeParticleSystem(this, content);

            Components.Add(explosionParticle);
            Components.Add(fireParticle);
            Components.Add(smokeParticle);

            windowManager = new WindowManager(this);
            windowManager.SetParticles((ExplosionParticleSystem)explosionParticle, (FireParticleSystem)fireParticle, (SmokePlumeParticleSystem)smokeParticle);

            Components.Add(windowManager);

            bloom = new BloomComponent(this);
            Components.Add(bloom);

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
           
            base.Initialize();

        }

       
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
               
                // TODO: Load any ResourceManagementMode.Automatic content
            }
          
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
            //*///CameraY = MathHelper.SmoothStep(CameraY, 80f, .05f);
            /*
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
        
    }
}
