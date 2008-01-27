using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Tanks
{
    class HiveQueen : GameplayObject
    {
        public Model QueenModel;

        public Vector3 ModelRotation;

        public List<EnemyGenerator> Generators = new List<EnemyGenerator>();

        float Health;

        private List<Swarm> ReturnSwarms;

        public HiveQueen(Vector2 pos, Model QueenModel)
        {
            Position = pos;
            this.QueenModel = QueenModel;
            Health = 100;
            
            this.mass = 5;
            this.radius = 20f;

            this.nocollide.Add(20);

            this.type = 5;

            Generators.Add(new EnemyGenerator(new Vector2(50, 0), 10, new TimeSpan(0, 0, 10)));

            this.Initialize();
        }

        public List<Swarm> Update(GameTime gameTime, Model EnemyModel)
        {
            collidedThisFrame = false;

            Velocity *= 0.95f;

            ReturnSwarms = new List<Swarm>();
            foreach (EnemyGenerator g in Generators)
            {
                Swarm AddSwarm = g.Update(gameTime, EnemyModel);

                if (AddSwarm != null)
                {
                    ReturnSwarms.Add(AddSwarm);
                }
            }

            ModelRotation.Y += 1.5f;

            return ReturnSwarms;
        }

        /// <summary>
        /// Draw
        /// </summary>
        /// <param name="Camera">cam</param>
        /// <param name="aspectRatio">ar</param>
        /// <param name="Model">Generator Model</param>
        public void Draw(Vector3 Camera, float aspectRatio, Model Model)
        {
            Matrix[] transforms = new Matrix[QueenModel.Bones.Count];
            QueenModel.CopyAbsoluteBoneTransformsTo(transforms);
            Vector3 NewPosition = new Vector3(Position.X, 0, Position.Y);
            //Draw the model, a model can have multiple meshes, so loop
            foreach (ModelMesh mesh in QueenModel.Meshes)
            {


                //This is where the mesh orientation is set, as well as our camera and projection
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index]
                                   * Matrix.CreateRotationY(MathHelper.ToRadians(this.ModelRotation.Y))
                                   * Matrix.CreateTranslation(NewPosition)
                                   * Matrix.CreateScale(1.0f);


                    effect.View = Matrix.CreateLookAt(Camera, new Vector3(Camera.X, 0f, Camera.Z - .1f), new Vector3(0, 1, 0));
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 10000.0f);
                }
                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }

            foreach (EnemyGenerator g in Generators)
            {
                g.Draw(Model, Camera, aspectRatio);
            }
        }
    }
}
