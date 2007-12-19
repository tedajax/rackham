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
    class Enemy
    {
        public Vector3 position;
        public Vector3 velocity;
        public float speed;

        public Vector3 target;

        public Model enemyModel;

        public Enemy(Vector3 pos, Vector3 vel, float spd, Model m)
        {
            position = pos;
            velocity = vel;
            target = position;
            enemyModel = m;
            speed = spd;
        }

        public Enemy(Vector3 pos, Vector3 vel, float spd, Model m, Vector3 tar)
        {
            position = pos;
            velocity = vel;
            target = tar;
            enemyModel = m;
            speed = spd;
        }

        public void Update(GameTime GameTime)
        {
            if (position.X > target.X)
                velocity.X -= speed * GameTime.ElapsedGameTime.Milliseconds;
            if (position.X < target.X)
                velocity.X += speed * GameTime.ElapsedGameTime.Milliseconds;

            if (position.Y > target.Y)
                velocity.Y -= speed * GameTime.ElapsedGameTime.Milliseconds;
            if (position.Y < target.Y)
                velocity.Y += speed * GameTime.ElapsedGameTime.Milliseconds;
        }

        public void drawPlayer(Vector3 cp, float ar)
        {

            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[enemyModel.Bones.Count];
            enemyModel.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in enemyModel.Meshes)
            {
                // This is where the mesh orientation is set, as well as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateRotationX(MathHelper.ToRadians(270.0f))
                        * Matrix.CreateRotationY(MathHelper.ToRadians(180.0f))
                        * Matrix.CreateTranslation(position.X, position.Y, 0.0f);
                    effect.View = Matrix.CreateLookAt(cp, Vector3.Zero, new Vector3(0, 1, 0));
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f),
                        ar, 1.0f, 150.0f);
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }
    }
}
