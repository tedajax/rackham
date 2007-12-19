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
    class Enemy : GameplayObject
    {
        
        public float speed;

        public Vector2 Target;

        public Model enemyModel;

        public Enemy(Vector2 pos, Vector2 vel, float spd, Model m)
        {
            Position = pos;
            Velocity = vel;
            Target = Position;
            enemyModel = m;
            speed = spd;
            this.radius = .2f;
            this.Mass = 5f;
            Initialize();
        }

        public Enemy(Vector2 pos, Vector2 vel, float spd, Model m, Vector2 tar)
        {
            Position = pos;
            Velocity = vel;
            Target = tar;
            enemyModel = m;
            speed = spd;
            this.radius = .2f;
            this.Mass = 5f;
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void Update(GameTime GameTime, Collision CollisionHandle)
        {
            CollidedThisFrame = false;

            if (Position.X > Target.X)
                velocity.X -= speed;
            if (Position.X < Target.X)
                velocity.X += speed;

            if (Position.Y > Target.Y)
                velocity.Y -= speed;
            if (Position.Y < Target.Y)
                velocity.Y += speed;
        }

        public void Draw(Vector3 cp, float ar)
        {
            DisplayModel(cp, ar);
        }

        public void DisplayModel(Vector3 Camera, float aspectRatio)
        {
            Matrix[] transforms = new Matrix[enemyModel.Bones.Count];
            enemyModel.CopyAbsoluteBoneTransformsTo(transforms);
            Vector3 NewPosition = new Vector3(Position.X, 0, Position.Y);
            //Draw the model, a model can have multiple meshes, so loop
            foreach (ModelMesh mesh in enemyModel.Meshes)
            {


                //This is where the mesh orientation is set, as well as our camera and projection
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(MathHelper.ToRadians(Rotation))
                     * Matrix.CreateTranslation(NewPosition)
                     * Matrix.CreateScale(1.0f);

                    effect.View = Matrix.CreateLookAt(Camera, Vector3.Zero, new Vector3(0, 1, 0));
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 10000.0f);
                }
                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }
        }
    }
}
