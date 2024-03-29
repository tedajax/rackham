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
        public float Radius;
        public float ModifiedRadius;

        public Vector2 Target;

        public Model enemyModel;
        public BoundingSphere EnemySightSphere;

        public static float MaxVelocity = .1f;

        Vector2 OriginalPosition;

        public bool InSwarm = false;

        
        public String mySwarmId = "NoId";

        public Enemy(Vector2 pos, Vector2 vel, float spd, Model m)
        {
            Position = pos;
            Velocity = vel;
            Target = Position;
            enemyModel = m;
            speed = spd;
            this.radius = .2f;
            
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
            
            Initialize();
        }

        public override void Initialize()
        {
            this.Mass = 0.5f;
            this.type = 20;
            this.NoCollide.Add(this.type);
            base.Initialize();

            OriginalPosition = Position;
            EnemySightSphere = new BoundingSphere(new Vector3(Position.X, 0f, Position.Y), 10f);
        }

        public void Update(GameTime GameTime)
        {
            CollidedThisFrame = false;

            if (InSwarm == true)
            {



                EnemySightSphere.Center = new Vector3(Position.X, 0f, Position.Y);

                //position.X = MathHelper.SmoothStep(Position.X, Target.X, .1f);
                //position.Y = MathHelper.SmoothStep(Position.Y, Target.Y, .1f);

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
            }
            else
            {
                //This should never happen, an enemy should never be created without being placed inside a swarm
            }
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
                    effect.PreferPerPixelLighting = true;
                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(MathHelper.ToRadians(Rotation))
                     * Matrix.CreateTranslation(NewPosition)
                     * Matrix.CreateScale(1.0f);

                    effect.View = Matrix.CreateLookAt(Camera, new Vector3(Camera.X, 0f, Camera.Z - .1f), new Vector3(0, 1, 0));
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 10000.0f);
                }
                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }
        }

        public override void BoundingSphereTouch(int type)
        {
           // Collision.addboundlist.Add(new RegisteredBoundingSphere(new BoundingSphere(new Vector3(position.X, 0, position.Y), 5), 15, new TimeSpan(0, 0, 0, 0, 100)));
            Kamikazi();
        }

        public override bool Touch(GameplayObject target)
        {
            if (target.Type == 11)
            {
                //Collision.addboundlist.Add(new RegisteredBoundingSphere(new BoundingSphere(new Vector3(position.X, 0, position.Y), 5), 15, new TimeSpan(0, 0, 0, 0, 100)));
            }

            Kamikazi();

            return base.Touch(target);
        }

        public void Kamikazi()
        {
            Vector3 pos = new Vector3(Position.X, 0f, Position.Y);
            //Vector3 vel = new Vector3(Velocity.X, 0f, Velocity.Y);
            Vector3 vel = Vector3.Zero;
            this.active = false;
            for (int x = 0; x < 5; x++)
                WindowManager.explosionParticle.AddParticle(pos, vel);
            SwarmManager.EnemiesToDestroy.Add(this);
//            Collision.addboundlist.Add(new RegisteredBoundingSphere(new BoundingSphere(new Vector3(position.X, 0, position.Y), 10), 1, new TimeSpan(0, 0, 0, 0, 100)));
              
        }

        public override void HitBoundry()
        {
            if (Math.Abs(Position.X) > 1500)
            {
                if (Position.X > 0)
                {
                    position.X = 1500;
                    if (Velocity.X > 0)
                        velocity.X = 0;
                }
                else
                {
                    position.X = -1500;
                    if (Velocity.X < 0)
                        velocity.X = 0;
                }
            }

            if (Math.Abs(Position.Y) > 1500)
            {
                if (Position.Y > 0)
                {
                    position.Y = 1500;
                    if (Velocity.Y > 0)
                        velocity.Y = 0;
                }
                else
                {
                    position.Y = -1500;
                    if (Velocity.Y < 0)
                        velocity.Y = 0;
                }
            }
        }
    }
}
