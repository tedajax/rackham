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
    class Player
    {
        public string Name;
        public int Health;
        public Model Model;
        public Vector3 Position;
        public Vector3 Velocity;
        public float Rotation;
        public int CollisionNum;
        public float radius;

        public float speed;

        private int ShotTime;
        private Vector2 DrawBase;
        private int Ready;
        private Keys Upkey;
        private Keys Downkey;
        private Keys Leftkey;
        private Keys Rightkey;
        private Keys StopKey;
        private Keys Action;
        public Player(Vector2 DrawBase, Keys Start, float radius)
        {
            Name = null;
            Health = 100;
            Model = null;
            Position = new Vector3(0.0f, 0.0f, 0.0f);
            Rotation = 0.0f;
            this.DrawBase = DrawBase;
            Ready = 0;
            Action = Start;
            this.radius = radius;
            speed = .0004f;
            Velocity = Vector3.Zero;
        }
        public void setupCollision(Collision CollisionHandle)
        {
            CollisionNum = CollisionHandle.AddBoundingSphere(Position, radius, 1);

        }

        public void Draw(Vector3 Camera, float aspectRatio, BitmapFont Font)
        {
            if (Ready == 6)
            {
                DisplayModel(Camera, aspectRatio);
                Font.DrawString((int)DrawBase.X, (int)DrawBase.Y, CollisionNum.ToString());

            }
            else
            {
                InitialDraw(Font);
            }




        }
        public void Update(KeyboardState Pressed, bool KeyReleased, GameTime Gametime, Collision CollisionHandle, Bullet[] BulletClass)
        {
            if (Ready == 6)
            {
                ControlModel(Pressed, Gametime, CollisionHandle,BulletClass);
            }
            else
            {
                Initial(Pressed, KeyReleased);
                if (Ready == 6) setupCollision(CollisionHandle);
            }
        }

        public void ControlModel(KeyboardState Pressed, GameTime Gametime, Collision CollisionHandle, Bullet[] BulletClass)
        {
           
            if (Pressed.IsKeyDown(Upkey))
            {

                Velocity.Z -= speed * Gametime.ElapsedGameTime.Milliseconds;
                Rotation = 90.0f;

            }
             if (Pressed.IsKeyDown(Downkey))
            {

                Velocity.Z += speed * Gametime.ElapsedGameTime.Milliseconds;
                Rotation = -90.0f;

            }
            if (Pressed.IsKeyDown(Leftkey))
            {

                Velocity.X -= speed * Gametime.ElapsedGameTime.Milliseconds;
                Rotation = 180.0f;

            }
             if (Pressed.IsKeyDown(Rightkey))
            {
                Velocity.X += speed * Gametime.ElapsedGameTime.Milliseconds;
                Rotation = 0.0f;

            }
             if (Pressed.IsKeyDown(StopKey))
            {
                Velocity *= .95f;
            }
            Position += Velocity;
           
            CollisionHandle.MoveObject(Position, radius, CollisionNum);
            if (Pressed.IsKeyDown(Keys.LeftShift) && Gametime.TotalGameTime.Seconds> ShotTime)
            {
                int z = 0;
                while (BulletClass[z] != null) z++;
                BulletClass[z] = new Bullet(Position, new Vector3((float)(Math.Cos((double)MathHelper.ToRadians(Rotation))/100), 0.0f, (float)(Math.Sin((double)MathHelper.ToRadians(Rotation)))/-100), 0.5f, CollisionHandle);
                ShotTime = Gametime.TotalGameTime.Seconds + 3;
            }

        }
        public void CheckCollision(Collision CollisionHandle)
        {
            if (Ready == 6)
            {
                 int[] groupofints = {1};
                if (CollisionHandle.StartCheckCollision(CollisionNum,groupofints)  == true)
                {
                    Velocity = Velocity / 2 * -1f;
                    
                }
            }
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
                if (Pressed.IsKeyDown(Action) && KeyReleased == true) Ready++;
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
                    StopKey = Pressed.GetPressedKeys()[0];
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
                Font.DrawString((int)DrawBase.X, (int)DrawBase.Y, "Press Stop Key now");
            }
        }
        public void DisplayModel(Vector3 Camera, float aspectRatio)
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
                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(MathHelper.ToRadians(Rotation))
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
