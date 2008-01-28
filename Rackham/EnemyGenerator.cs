using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tanks
{
    class EnemyGenerator : GameplayObject
    {
        int NumberPerGeneration; //Number made everytime it's charged
        
        TimeSpan TimeBetweenGeneration; //Time before next generation
        TimeSpan CurrentTimeBetweenGeneration;
        static TimeSpan CoolDown = new TimeSpan(0, 0, 5);
        TimeSpan CurrentCoolDown = CoolDown;

        Random GenerationRandomizer;

        float YRotation = 0f;
        float RotationSpeed;

        Swarm ReturnSwarm;

        private bool cooldown = false;

        /// <summary>
        /// Creates an enemy generator
        /// </summary>
        /// <param name="Pos">Generator position</param>
        /// <param name="numpergen">Number of enemies made during each generation</param>
        /// <param name="time">Time it takes the generator to create enemies</param>
        public EnemyGenerator(Vector2 Pos, int numpergen, TimeSpan time)
        {
            NumberPerGeneration = numpergen;
            TimeBetweenGeneration = time;
            CurrentTimeBetweenGeneration = TimeBetweenGeneration;

            RotationSpeed = 1f;

            position = Pos;

            nocollide.Add(5);
            nocollide.Add(20);

            this.type = 6;
            
            this.radius = 9f;

            GenerationRandomizer = new Random();

            this.Initialize();
        }

        public Swarm Update(GameTime gameTime, Model EnemyModel)
        {
            if (CurrentTimeBetweenGeneration.TotalMilliseconds > 0)
            {
                CurrentTimeBetweenGeneration -= gameTime.ElapsedGameTime;
                
                if (CurrentTimeBetweenGeneration.TotalSeconds <= 5)
                    RotationSpeed += .1f;
            }
            else
            {
                if (!cooldown)
                {
                    List<Enemy> EnemyList = new List<Enemy>();

                    for (int i = 0; i < NumberPerGeneration; i++)
                    {
                        Vector2 EPos = new Vector2(this.Position.X + GenerationRandomizer.Next(-3, 3), this.Position.Y + GenerationRandomizer.Next(-3, 3));
                        EnemyList.Add(new Enemy(EPos, Vector2.Zero, .001f, EnemyModel));
                    }

                    ReturnSwarm = new Swarm(this.Position, Vector2.Zero, EnemyList);
                    ReturnSwarm.moveSwarm(Vector2.Zero);

                    cooldown = true;
                }
                else
                {
                    if (CurrentCoolDown.TotalMilliseconds > 0)
                    {
                        CurrentCoolDown -= gameTime.ElapsedGameTime;
                        RotationSpeed -= .3f;
                        if (RotationSpeed < 1f) RotationSpeed = 1f;
                    }
                    else
                    {
                        CurrentTimeBetweenGeneration = TimeBetweenGeneration;
                        CurrentCoolDown = CoolDown;

                        cooldown = false;
                    }

                    ReturnSwarm = null;
                }
            }

            velocity *= 0.85f;
            YRotation += RotationSpeed;

            return ReturnSwarm;
        }

        public void Draw(Model Model, Vector3 Camera, float aspectRatio)
        {
            Matrix[] transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);
            Vector3 NewPosition = new Vector3(Position.X, 0, Position.Y);
            //Draw the model, a model can have multiple meshes, so loop
            foreach (ModelMesh mesh in Model.Meshes)
            {


                //This is where the mesh orientation is set, as well as our camera and projection
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index]
                                   * Matrix.CreateRotationY(MathHelper.ToRadians(YRotation))
                                   * Matrix.CreateTranslation(NewPosition / 5)
                                   * Matrix.CreateScale(5.0f);


                    effect.View = Matrix.CreateLookAt(Camera, new Vector3(Camera.X, 0f, Camera.Z - .1f), new Vector3(0, 1, 0));
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 10000.0f);
                }
                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }
        }
    }
}
