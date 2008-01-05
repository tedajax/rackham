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

        public BoundingSphere PresenceSphere;
        
        public int CollisionNum;


        public float speed;

        public int SelectedGamerNum = 0;
        public SignedInGamer SelectedGamer;
        public String GamerName;



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

            //base.Initialize();

        }

        public override void Initialize()
        {
            base.Initialize();
        }


        public void Draw(Vector3 Camera, float aspectRatio, BitmapFont Font)
        {
            if (Ready == 6)
            {
                DisplayModel(Camera, aspectRatio);
                Font.DrawString((int)DrawBase.X, (int)DrawBase.Y, GamerName+":"+Health.ToString());

            }
            else
            {
                InitialDraw(Font);
            }
        }
    


        public void Update(KeyboardState Pressed, bool KeyReleased, GameTime Gametime, Collision CollisionHandle, List<Bullet> BulletClass)
        {
            collidedThisFrame = false;
            if (Ready == 6)
            {
                ControlModel(Pressed, Gametime, CollisionHandle,BulletClass);
            }
            else
            {
                Initial(Pressed, KeyReleased);
                if (Ready == 6) base.Initialize();
                
            }
        }

        public override bool Touch(GameplayObject target)
        {
            Health -= (int)(target.Mass*5f);
            return true;
        }

        public void ControlModel(KeyboardState Pressed, GameTime Gametime, Collision CollisionHandle, List<Bullet> BulletClass)
        {
            float OldRotation = Rotation;
            Rotation = 0.0f;
            int NumberRot = 0; 
            if (Pressed.IsKeyDown(Upkey))
            {

                velocity.Y -= speed ;
                Rotation += 90.0f;
                NumberRot++;

            }
             if (Pressed.IsKeyDown(Downkey))
            {
               
                velocity.Y += speed ;
                Rotation += -90.0f;
                NumberRot++;

            }
            if (Pressed.IsKeyDown(Leftkey))
            {

                velocity.X -= speed ;
                Rotation += 180.0f;
                NumberRot++;

            }
             if (Pressed.IsKeyDown(Rightkey))
            {
                velocity.X += speed ;
                Rotation += 0.0f;
                NumberRot++;

            }
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
            Velocity *= .99f;
            if (Pressed.IsKeyDown(ShootKey) && Gametime.TotalGameTime.CompareTo(ShotTime)>0)
            {
                float eangle = 10f;

                List<Bullet> newbullets = new List<Bullet>();

                int maxbullets = 7;

                for (int i = 0; i < maxbullets; i++)
                {
                    newbullets.Add(new Bullet(Position, 5 * new Vector2((float)(Math.Cos((double)MathHelper.ToRadians((Rotation + (eangle * (i - (int)(maxbullets / 2)))))) / 100), (float)(Math.Sin((double)MathHelper.ToRadians((Rotation + (eangle * (i - (int)(maxbullets / 2))))))) / -100), 0.5f, CollisionHandle));
                }
                
                foreach (Bullet b in newbullets)
                    b.NoCollide.Add(type);

                BulletClass = AddBullets(BulletClass, newbullets);

                ShotTime = Gametime.TotalGameTime + new TimeSpan(0, 0, 0, 0, 200);
            }
        
           
            
            /*if (Pressed.IsKeyDown(Keys.LeftShift) && Gametime.TotalGameTime.Seconds> ShotTime)
            {
                int z = 0;
                while (BulletClass[z] != null) z++;
                BulletClass[z] = new Bullet(Position, new Vector3((float)(Math.Cos((double)MathHelper.ToRadians(Rotation))/100), 0.0f, (float)(Math.Sin((double)MathHelper.ToRadians(Rotation)))/-100), 0.5f, CollisionHandle);
                ShotTime = Gametime.TotalGameTime.Seconds + 3;
            }*/


            PresenceSphere.Center = new Vector3(Position.X, 0f, Position.Y);
        }

        private List<Bullet> AddBullets(List<Bullet> BulletClass, List<Bullet> BulletList)
        {
            foreach (Bullet b in BulletList)
                BulletClass.Add(b);

            return BulletClass;
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
                   
                    //Guide.ShowSignIn(1, false);
                    Ready = 10;
                    
                }
            }
            else if (Ready == 10)
            {
                if (Guide.IsVisible == false)
                {
                    Ready = 1;
                    if (Gamer.SignedInGamers.Count > 0)
                    {

                        if (Guide.IsVisible == false)
                        {
                            
                            ////lololololool
                            GamerName = Gamer.SignedInGamers[0].Gamertag;
                            SelectedGamer = Gamer.SignedInGamers[0];
                            Ready = 1;
                            
                        }
                    }
                }
            }
           
            else if (Ready == 1)
            {
                if (KeyReleased == true && Pressed.GetPressedKeys().Length == 1)
                {
                    Leftkey = Pressed.GetPressedKeys()[0];
                    Ready++;
                }

            }
            else if (Ready == 2)
            {
                if (KeyReleased == true && Pressed.GetPressedKeys().Length == 1)
                {
                    Rightkey = Pressed.GetPressedKeys()[0];
                    Ready++;
                }
            }
            else if (Ready == 3)
            {
                if (KeyReleased == true && Pressed.GetPressedKeys().Length == 1)
                {
                    Upkey = Pressed.GetPressedKeys()[0];
                    Ready++;
                }
            }
            else if (Ready == 4)
            {
                if (KeyReleased == true && Pressed.GetPressedKeys().Length == 1)
                {
                    Downkey = Pressed.GetPressedKeys()[0];
                    Ready++;
                }
            }
            else if (Ready == 5)
            {
                if (KeyReleased == true && Pressed.GetPressedKeys().Length == 1)
                {
                    ShootKey = Pressed.GetPressedKeys()[0];
                    Ready++;
                }
            }

        }
        public void InitialDraw(BitmapFont Font)
        {
            if (Ready == 0)
            {

                Font.DrawString((int)DrawBase.X, (int)DrawBase.Y, "Press \"" + Action.ToString() + "\" To Enter");
            }
            if (Ready == 11)
            {
                Font.DrawString((int)DrawBase.X, (int)DrawBase.Y, "Select A Profile : "+GamerName);
            }

            if (Ready == 1)
            {
                Font.DrawString((int)DrawBase.X, (int)DrawBase.Y, "Press Leftkey now");
            }
            if (Ready == 2)
            {
                Font.DrawString((int)DrawBase.X, (int)DrawBase.Y, "Press Rightkey now");
            }
            if (Ready == 3)
            {
                Font.DrawString((int)DrawBase.X, (int)DrawBase.Y, "Press Upkey now");
            }
            if (Ready == 4)
            {
                Font.DrawString((int)DrawBase.X, (int)DrawBase.Y, "Press Downkey now");
            }
            if (Ready == 5)
            {
                Font.DrawString((int)DrawBase.X, (int)DrawBase.Y, "Press Shoot Key now");
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

                    effect.View = Matrix.CreateLookAt(Camera, Vector3.Zero, new Vector3(0, 1, 0));
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 10000.0f);
                }
                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }
        }

        public int getReadyState() { return Ready; }



    }

}
