//Gokul is not stupid
//Infact, ted is the stupid one
//No I would say Gokul is the stupid one

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using XNAExtras;
#endregion

namespace Tanks
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //This is some amazing code right here
        //Whoever wrote this comment should be deemed a god.
        GraphicsDeviceManager graphics;
        ContentManager content;

        BitmapFont Times;

        //Create the First player object;
        Player Player1 = new Player(new Vector2(10, 0), Keys.D1, 5);
        Player Player2 = new Player(new Vector2(550,0), Keys.D2, 5);
        Player Player3 = new Player(new Vector2(10, 450), Keys.D3, 5);
        //Create the ground
        Model BulletModel;
        Bullet[] BulletClass = new Bullet[100];
        
        //Position of the Camera in world space, for our view matrix
        Vector3 cameraPosition = new Vector3(0.0f, 80.0f, .1f);

        //Aspect ratio to use for the projection matrix
        float aspectRatio = 640.0f / 480.0f;

        //KeyboardStates
        KeyboardState KeyState;
        bool KeyReleased;

        //Collision
        Collision CollisionHandler = new Collision();
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            content = new ContentManager(Services);
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

                Player1.Model = content.Load<Model>("Models\\Tank");
                Player2.Model = Player1.Model;
                Player3.Model = Player1.Model;
                BulletModel = content.Load<Model>("Models\\Sphere");
                // TODO: Load any ResourceManagementMode.Automatic content
            }
            Times.Reset(graphics.GraphicsDevice);
            Times.KernEnable = false;
            Times.TextColor = Color.White;
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

            KeyState = Keyboard.GetState();

            // TODO: Add your update logic here
            foreach (Bullet x in BulletClass)
            {
                if (x != null) x.Update(gameTime, CollisionHandler);
            }

            Player2.Update(KeyState, KeyReleased, gameTime, CollisionHandler, BulletClass);
            Player1.Update(KeyState, KeyReleased, gameTime, CollisionHandler, BulletClass);
           
            Player3.Update(KeyState, KeyReleased, gameTime, CollisionHandler, BulletClass);

            
            Player1.CheckCollision(CollisionHandler);
            Player2.CheckCollision(CollisionHandler);
            Player3.CheckCollision(CollisionHandler);
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
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            //Draw the ground
            //DrawModel(Ground, cameraPosition, aspectRatio);
            //Copy any parent transforms
            foreach (Bullet x in BulletClass)
            {
                if (x!=null) x.Draw(BulletModel, cameraPosition, aspectRatio, Times);
            }

            
            Player1.Draw(cameraPosition, aspectRatio, Times);
            Player2.Draw(cameraPosition, aspectRatio, Times);
            Player3.Draw(cameraPosition, aspectRatio, Times);
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
//Merging sure is fun, but i bet it can be a pain also.
