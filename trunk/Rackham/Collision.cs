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
    //This is going to be a godly Collision System
    class Collision
    {
        #region Helper Types


        /// <summary>
        /// The result of a collision query.
        /// </summary>
        struct CollisionResult
        {
            /// <summary>
            /// How far away did the collision occur down the ray
            /// </summary>
            public float Distance;

            /// <summary>
            /// The collision "direction"
            /// </summary>
            public Vector2 Normal;

            /// <summary>
            /// What caused the collison (what the source ran into)
            /// </summary>
            public GameplayObject GameplayObject;


            public static int Compare(CollisionResult a, CollisionResult b)
            {
                return a.Distance.CompareTo(b.Distance);
            }
        }


        #endregion

        public static List<GameplayObject> AllGameplayObjects = new List<GameplayObject>();


        public Collision()
        {

        }

        public static bool AddGamePlayObject(GameplayObject add)
        {
            AllGameplayObjects.Add(add);
        }

    }
}
