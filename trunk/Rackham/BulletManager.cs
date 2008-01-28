using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework;
using System.Collections;
namespace Tanks
{
    class BulletManager
    {
        static System.Collections.Hashtable bullethashtable = new System.Collections.Hashtable();
        public static List<String> BulletsToRemove = new List<String>();

        static int ExtraCounter;
        static TimeSpan lasttime;
        

        public void Update(GameTime gameTime, Player p)
        {
            RemoveDeadBullets();
            foreach (DictionaryEntry de in bullethashtable)
            {
                Bullet b = (Bullet)de.Value;
                if (Math.Abs(b.Position.X) > 200 + Math.Abs(p.Position.X) || Math.Abs(b.Position.Y) > 200 + Math.Abs(p.Position.Y))
                {
                    b.Kamikazie();
                }
                else
                    b.Update(gameTime);
            }
        }

        public System.Collections.Hashtable GetBulletHashTable() { return bullethashtable; }

        public void Draw(Microsoft.Xna.Framework.Graphics.Model m, Vector3 CameraPosition, float AspectRatio)
        {
            foreach (DictionaryEntry de in bullethashtable)
            {
                Bullet x = (Bullet)de.Value;
                
                if (x != null)
                {
                    x.Draw(m, CameraPosition,AspectRatio);
                }
                
            }
        }
        public void RemoveDeadBullets()
        {
            foreach (String b in BulletsToRemove)
            {
                bullethashtable.Remove(b);
            }
            BulletsToRemove.Clear();
        }
        public static void AddBullet(Bullet b, GameTime gameTime)
        {
            if (gameTime.ElapsedGameTime.Equals(lasttime))
            {
                ExtraCounter++;
            }
            else
            {
                ExtraCounter = 0;
            }
            string key = gameTime.ElapsedGameTime.ToString() + ExtraCounter.ToString();
            b.mykey = key;
            bullethashtable.Add(key, b);
            lasttime = gameTime.ElapsedGameTime;
        }

        public Hashtable getBullets() { return bullethashtable; }
        

    }
}
