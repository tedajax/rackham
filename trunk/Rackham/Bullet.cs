using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using XNAExtras;

namespace Tanks
{
    class Bullet
    {
        Vector3 Position;
        Vector3 Velocity;
        float Radius;
        Vector2 CollisionNum;

        public Bullet(Vector3 Pos, Vector3 Velo, float Rad, Collision CollisionHandle)
        {
            Position = Pos;
            Velocity = Velo;
            Radius = Rad;
            CollisionHandle.AddBoundingSphere(Position, Radius, 2);
        }
        public void Update(GameTime Gametime, Collision CollisionHandle)
        {
            //Position = (Position + Velocity) * Gametime.ElapsedGameTime.Milliseconds;
            Position.X += Velocity.X * Gametime.ElapsedGameTime.Milliseconds;
            Position.Z += Velocity.Z * Gametime.ElapsedGameTime.Milliseconds;

        }
        public void Draw(Model Model, Vector3 Camera, float aspectRatio, BitmapFont Font)
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
                     * Matrix.CreateTranslation(Position)
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
