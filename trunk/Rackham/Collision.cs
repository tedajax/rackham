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
    class Collision
    {
        BoundingSphere[] CollisionObj = new BoundingSphere[500];
        int[] type = new int[500];
        int Max;

        public Collision()
        {
            Max = 0;
        }

        public int AddBoundingSphere(Vector3 Position, float Radius, int type)
        {
            int x = 0;
            while (CollisionObj[x].Radius != 0) x++;
            CollisionObj[x] = new BoundingSphere(Position, Radius);
            if (x > Max) Max = x;
            this.type[x] = type;
            return x;
        }
        public void MoveObject(Vector3 position, float Radius, int callnum)
        {
      
            CollisionObj[callnum] = new BoundingSphere(position, Radius);
            
       

        }
        public bool StartCheckCollision(int callnum, int[] checktypeS)
       
        {
            return CheckCollision(callnum, checktypeS);

        }

        public bool isCheckType(int[] checktypes, int checkagainst)
        {
            foreach (int i in checktypes)
            {
                if (i == type[checkagainst])
                    return true;
            }
            return false;
        }

        public bool CheckCollision(int callnum, int[] checktypes)
        {
          
            for (int i = 0; i <= Max; i++)
            {
                if (i != callnum)
                {
                    if (isCheckType(checktypes, i))
                    {
                        if (CollisionObj[i].Intersects(CollisionObj[callnum]))
                            return true;

                    }
                }
               
            }

            return false;
        }
    }
}
