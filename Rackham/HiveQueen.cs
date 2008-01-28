using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Tanks
{
    class HiveQueen : GameplayObject
    {
        static int MaxGenerators = 10; 
        public int EnemiesDefendingMe;  //Only to be used by swarm manager

        public Model QueenModel;

        public Vector3 ModelRotation;

        private TimeSpan TimeSinceLastGenerator = new TimeSpan();
        private TimeSpan TimeTillNextGenerator = new TimeSpan(0, 1, 0);
        private int GeneratorsICanCreate = 3;

        Vector2 Target;
        Vector2 StartPosition;
        float MaxVelocity = .05f;
        float speed = .001f;

        float yheight = 0f;

        Random QueenRandom;

        bool SetGenerator = false;

        public List<EnemyGenerator> Generators = new List<EnemyGenerator>();

        public float Health;

        public static bool QueenDead = false;

        float bounds = 1500;

        private List<Swarm> ReturnSwarms;

        public HiveQueen(Vector2 pos, Model QueenModel)
        {
            Position = pos;
            StartPosition = Position;
            Target = Position;
            this.QueenModel = QueenModel;
            Health = 100;
            
            this.mass = 5;
            this.radius = 10f;

            this.nocollide.Add(20);

            QueenRandom = new Random();
            
            this.type = 5;
            this.Initialize();
        }

        public List<Swarm> Update(GameTime gameTime, Model EnemyModel)
        {
            collidedThisFrame = false;


            float ExtraSpeed = 8f;

            if (Position.X > Target.X)
            {
                if (Velocity.X > 0)
                    velocity.X -= speed * ExtraSpeed;
                else
                    velocity.X -= speed;
            }
            if (Position.X < Target.X)
            {
                if (Velocity.X < 0)
                    velocity.X += speed * ExtraSpeed;
                else
                    velocity.X += speed;
            }

            if (Position.Y > Target.Y)
            {
                if (Velocity.Y > 0)
                    velocity.Y -= speed * ExtraSpeed;
                else
                    velocity.Y -= speed;
            }
            if (Position.Y < Target.Y)
            {
                if (Velocity.Y < 0)
                    velocity.Y += speed * ExtraSpeed;
                else
                    velocity.Y += speed;
            }
            if (Math.Abs(velocity.X) > MaxVelocity)
            {
                velocity.X /= Math.Abs(velocity.X);
                velocity.X *= MaxVelocity;
            }
            if (Math.Abs(Velocity.Y) > MaxVelocity)
            {
                velocity.Y /= Math.Abs(velocity.Y);
                velocity.Y *= MaxVelocity;
            }

            TimeSinceLastGenerator += gameTime.ElapsedGameTime;

            if (!QueenDead)
            {
                if (GeneratorsICanCreate>0 && !SetGenerator && Generators.Count < MaxGenerators)
                {
                    Target.X = StartPosition.X + QueenRandom.Next(-100, 100);
                    Target.Y = StartPosition.Y + QueenRandom.Next(-100, 100);

                    if (Target.X < -bounds) Target.X = -bounds;
                    if (Target.X > bounds) Target.X = bounds;
                    if (Target.Y < -bounds) Target.Y = -bounds;
                    if (Target.Y > bounds) Target.Y = bounds;
                    GeneratorsICanCreate--;
                    SetGenerator = true;
                }
                /*
                if (TimeSinceLastGenerator.TotalSeconds > 15 && !SetGenerator && Generators.Count > 3)
                {
                    if (QueenRandom.Next(100) < 50 - Generators.Count && Generators.Count <= 8)
                    {
                        Target.X = Position.X + QueenRandom.Next(-250, 250);
                        Target.Y = Position.Y + QueenRandom.Next(-250, 250);

                        if (Target.X < -bounds) Target.X = -bounds;
                        if (Target.X > bounds) Target.X = bounds;
                        if (Target.Y < -bounds) Target.Y = -bounds;
                        if (Target.Y > bounds) Target.Y = bounds;

                        SetGenerator = true;
                    }
                    else
                    {
                        TimeSinceLastGenerator = new TimeSpan(0, 0, 0);
                    }
                }*/
                if (TimeSinceLastGenerator > TimeTillNextGenerator)
                {
                    TimeSinceLastGenerator = new TimeSpan();
                    GeneratorsICanCreate++;

                    SetGenerator = true;
                }

                if (SetGenerator)
                {
                    if (WithIn(Target, 2f))
                    {
                        Generators.Add(new EnemyGenerator(Position, 5, new TimeSpan(0, 0, 8)));
                        SetGenerator = false;
                        TimeSinceLastGenerator = new TimeSpan(0, 0, 0);

                        Target = StartPosition;
                    }
                }

                ReturnSwarms = new List<Swarm>();
                for (int i=0; i<Generators.Count;i++)
                {
                    EnemyGenerator g = Generators[i];
                    if (g.Active)
                    {
                        Swarm AddSwarm = g.Update(gameTime, EnemyModel);

                        if (AddSwarm != null)
                        {
                            ReturnSwarms.Add(AddSwarm);
                        }
                    }
                    else
                    {
                        Generators.Remove(g);
                    }
                }

                ModelRotation.Y += 1.5f;
            }
            else
            {
                Target = Position;
                if (TimeSinceLastGenerator.TotalMilliseconds > 50)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Vector3 explodepos = new Vector3(Position.X + QueenRandom.Next(-10, 10), yheight, Position.Y + QueenRandom.Next(-10, 10));
                        Vector3 explodevel = new Vector3(QueenRandom.Next(-100, 100), QueenRandom.Next(-25, 25), QueenRandom.Next(-100, 100));
                        for (int j = 0; j < 5; j++)
                        {
                            WindowManager.explosionParticle.AddParticle(explodepos, explodevel);
                        }
                    }

                    TimeSinceLastGenerator = new TimeSpan(0, 0, 0);
                }

                ModelRotation.Y += 0.1f;
                ModelRotation.X += (float)QueenRandom.NextDouble();
                ModelRotation.Z += (float)QueenRandom.NextDouble();
                yheight -= 1f;
            }


            return ReturnSwarms;
        }
        
        /// <summary>
        /// Draw
        /// </summary>
        /// <param name="Camera">cam</param>
        /// <param name="aspectRatio">ar</param>
        /// <param name="Model">Generator Model (Don't Ask)</param>
        public void Draw(Vector3 Camera, float aspectRatio, Model Model)
        {
            Matrix[] transforms = new Matrix[QueenModel.Bones.Count];
            QueenModel.CopyAbsoluteBoneTransformsTo(transforms);
            Vector3 NewPosition = new Vector3(Position.X, yheight, Position.Y);
            //Draw the model, a model can have multiple meshes, so loop
            foreach (ModelMesh mesh in QueenModel.Meshes)
            {


                //This is where the mesh orientation is set, as well as our camera and projection
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.World = transforms[mesh.ParentBone.Index]
                                   * Matrix.CreateRotationY(MathHelper.ToRadians(this.ModelRotation.Y))
                                   * Matrix.CreateRotationX(MathHelper.ToRadians(this.ModelRotation.X))
                                   * Matrix.CreateRotationZ(MathHelper.ToRadians(this.ModelRotation.Z))
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

        private bool WithIn(Vector2 Pos, float amount)
        {
            if (Distance(Position, Pos) <= amount)
                return true;
            else
                return false;
        }

        private float Distance(Vector2 v1, Vector2 v2)
        {
            return (float)(Math.Sqrt(Math.Pow((double)v2.Y - (double)v1.Y, 2) + Math.Pow((double)v2.X - (double)v1.X, 2)));
        }

        public override bool Touch(GameplayObject target)
        {
            if (target.Type == 11)
            {
                if (!QueenDead)
                {
                    Health -= 4;
                    if (Health <= 0)
                    {
                        QueenDead = true;
                        for (int i = 0; i < 1000; i++)
                        {
                            Vector3 explodepos = new Vector3(Position.X + QueenRandom.Next(-10, 10), yheight, Position.Y + QueenRandom.Next(-10, 10));
                            Vector3 explodevel = new Vector3(QueenRandom.Next(-100, 100), QueenRandom.Next(-25, 25), QueenRandom.Next(-100, 100));
                            for (int j = 0; j < 15; j++)
                            {
                                WindowManager.explosionParticle.AddParticle(explodepos, explodevel);
                            }
                        }
                    }
                }
            }

            return true;
        }

        public override void HitBoundry()
        {
            if (Math.Abs(Position.X) > bounds)
            {
                if (Position.X > 0)
                {
                    position.X = bounds;
                    if (Velocity.X > 0)
                        velocity.X = 0;
                }
                else
                {
                    position.X = -bounds;
                    if (Velocity.X < 0)
                        velocity.X = 0;
                }
            }

            if (Math.Abs(Position.Y) > bounds)
            {
                if (Position.Y > 0)
                {
                    position.Y = bounds;
                    if (Velocity.Y > 0)
                        velocity.Y = 0;
                }
                else
                {
                    position.Y = -bounds;
                    if (Velocity.Y < 0)
                        velocity.Y = 0;
                }
            }
        }
    }
}
