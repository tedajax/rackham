using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

using XNAExtras;

namespace Tanks
{
    class Player : GameplayObject
    {
        public string Name;
        public int Health;
        public Model Model;

        public GamePlayer LinkedProfile;

        public BoundingSphere PresenceSphere;

        private float VelocityCap = .1f;


        public float speed;

        public int SelectedGamerNum = 0;
        public SignedInGamer SelectedGamer;
        public String GamerName;

        private bool NoKeyPressed;

        private TimeSpan ShotTime;
        private Vector2 DrawBase;
        private int Ready;
        private Keys Upkey;
        private Keys Downkey;
        private Keys Leftkey;
        private Keys Rightkey;
        private Keys StopKey;
        private Keys ShootKey;
        private Keys Action;

        public TimeSpan OnFire = new TimeSpan();

        public Player(Vector2 DrawBase, Keys Start, float radius)
        {
            
            Name = null;
            Health = 100;
            Model = null;
            Position = new Vector2();
            Rotation = 0.0f;
            this.DrawBase = DrawBase;
            Ready = 0;
            Action = Start;
            this.radius = radius;
            speed = .001f;
            Velocity = Vector2.Zero;
            type = 1;

            PresenceSphere = new BoundingSphere(new Vector3(Position.X, 0f, Position.Y), 20f);
            LinkedProfile = null;
            //base.Initialize();

        }

        public override void Initialize()
        {
            base.Initialize();
        }


        public void Draw(Vector3 Camera, float aspectRatio, SpriteFont font, SpriteBatch batch)
        {
            batch.Begin();
            if (Ready == 6)
            {
                DisplayModel(Camera, aspectRatio);
                
                batch.DrawString(font,Health.ToString(), DrawBase, Color.White);
                

            }
            else
            {
                InitialDraw(batch, font);
            }

            batch.End();
        }
    


        public void Update(KeyboardState Pressed, bool KeyReleased, GameTime Gametime)
        {
            collidedThisFrame = false;
            if (Ready == 6)
            {
                ControlModel(Pressed, Gametime);
            }
            else
            {
                Initial(Pressed, KeyReleased);
                if (Ready == 6)
                {
                     this.Position = new Vector2(0, -12);
                   // this.velocity = new Vector2(-3, 0);
                    base.Initialize();
                }
                
            }
        }

        public override bool Touch(GameplayObject target)
        {
            Health -= (int)(target.Mass*50f);
            if (Health <= 0)
            {
                Random exploderandom = new Random();

                Ready = 0;
                Health = 100;
                int explosions = 10;
                int exploderadius = 20;
                Vector2 explodepos = new Vector2();
                for (int j = 0; j < explosions; j++)
                {
                    explodepos = new Vector2(exploderandom.Next(exploderadius) - (exploderadius / 2), exploderandom.Next(exploderadius) - (exploderadius / 2));
                    for (int i = 0; i < 10; i++)
                    {
                        WindowManager.explosionParticle.AddParticle(new Vector3(position.X + explodepos.X, 0f, position.Y + explodepos.Y), new Vector3
                            (velocity.X + explodepos.X, 0f, velocity.Y + explodepos.Y));
                    }
                }
            }
            return true;
        }

        public void ControlModel(KeyboardState Pressed, GameTime Gametime)
        {
            float OldRotation = Rotation;
            Rotation = 0.0f;
            int NumberRot = 0;

            NoKeyPressed = true;

            Vector2 OldVelocity = new Vector2(Velocity.X, Velocity.Y);
            if (Pressed.IsKeyDown(Upkey))
            {
                velocity.Y -= speed ;
                Rotation += 90.0f;
                NumberRot++;
                NoKeyPressed = false;
            }
            
            if (Pressed.IsKeyDown(Downkey))
            {
               
                velocity.Y += speed ;
                Rotation += -90.0f;
                NumberRot++;
                NoKeyPressed = false;
            }
            if (Pressed.IsKeyDown(Leftkey))
            {

                velocity.X -= speed ;
                Rotation += 180.0f;
                NumberRot++;
                NoKeyPressed = false;
            }
             if (Pressed.IsKeyDown(Rightkey))
            {
                velocity.X += speed ;
                Rotation += 0.0f;
                NumberRot++;
                NoKeyPressed = false;
            }
            if (Velocity.X > VelocityCap) velocity.X = VelocityCap;
            if (Velocity.Y > VelocityCap) velocity.Y = VelocityCap;
            if (Velocity.X < -VelocityCap) velocity.X = -VelocityCap;
            if (Velocity.Y < -VelocityCap) velocity.Y = -VelocityCap;
            if (NoKeyPressed)
                Velocity *= .95f;

            if (Velocity.X + Velocity.Y > 2) Velocity = OldVelocity;
            //If you pressed keys (increaes numberRot) then you should divide rotation by number of keys pressed (to get angle in between)
            if (NumberRot > 0)
            {
                Rotation /= NumberRot;
            }
            else
            {
                //If no keys were pressed then revert to the old rotation (so you can drift)
                Rotation = OldRotation;
            }
            //This is a workaround, when you press down and left at the same time, the model doesn't rotate correctly, this fixes it
            if (Pressed.IsKeyDown(Leftkey) && Pressed.IsKeyDown(Downkey)) Rotation += 180;
            
            
                        
            if (Pressed.IsKeyDown(ShootKey) && Gametime.TotalGameTime.CompareTo(ShotTime) > 0)
            {
                float eangle = 10f;

                List<Bullet> newbullets = new List<Bullet>();

                int maxbullets = 1;

                for (int i = 0; i < maxbullets; i++)
                {
                 
                    newbullets.Add(new Bullet(Position, 2.5f * new Vector2((float)(Math.Cos((double)MathHelper.ToRadians((Rotation + (eangle * (i - (int)(maxbullets / 2)))))) / 10), (float)(Math.Sin((double)MathHelper.ToRadians((Rotation + (eangle * (i - (int)(maxbullets / 2))))))) / -10), 1.65f, this.Rotation + 90));
                }

                foreach (Bullet b in newbullets)
                {
                    b.NoCollide.Add(type);
                    BulletManager.AddBullet(b, Gametime);
                }

                

                ShotTime = Gametime.TotalGameTime + new TimeSpan(0, 0, 0, 0, 200);
            }

            PresenceSphere.Center = new Vector3(Position.X, 0f, Position.Y);
        }

      
            
        public float ToAngle(float oldvalue, float newvalue, float speed, float tolerance)
        {
            if (oldvalue == newvalue)
            {
                return oldvalue;
            }
            if (oldvalue > newvalue)
            {
                oldvalue -= speed;
            }
            if (oldvalue < newvalue)
            {
                oldvalue += speed;
            }
            if (Math.Abs(oldvalue - newvalue) <= tolerance)
            {
                oldvalue = newvalue;
            }
            return oldvalue;
        }

        public void Initial(KeyboardState Pressed, bool KeyReleased)
        {
            if (Ready == 0)
            {
                if (Pressed.IsKeyDown(Action) && KeyReleased == true)
                {

                    if (LinkedProfile.Loaded == true)
                    {
                        Leftkey = LinkedProfile.LeftKey;
                        Rightkey = LinkedProfile.RightKey;
                        Upkey = LinkedProfile.UpKey;
                        Downkey = LinkedProfile.DownKey;
                        StopKey = LinkedProfile.EnterKey;
                        ShootKey = LinkedProfile.EnterKey;
                        Ready = 6;
                    }  
                }
            }
        }
        public void InitialDraw(SpriteBatch batch, SpriteFont font)
        {
            if (Ready == 0)
            {
                batch.DrawString(font, "Press \"" + Action.ToString() + "\" To Enter", DrawBase, Color.White);


            }
            
        }
        public void DisplayModel(Vector3 Camera, float aspectRatio)
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

        public int getReadyState() { return Ready; }



    }

}
