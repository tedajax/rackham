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
    public class Collision
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

        private static List<GameplayObject> killList = new List<GameplayObject>();
        public static List<GameplayObject> KillList
        {
            get { return killList; }
            
        }
        

        private static List<GameplayObject> allGameplayObjects = new List<GameplayObject>();
        public static List<GameplayObject> AllGamePlayObjects
        {
            get { return allGameplayObjects; }
        }
        List<CollisionResult> collisionResults = new List<CollisionResult>();

            

        public Collision() {        }


        public static bool AddGamePlayObject(GameplayObject add)
        {
            AllGamePlayObjects.Add(add);
            return true;
        }

        public void Update(float elapsedTime)
        {
            for (int i = 0; i < allGameplayObjects.Count; ++i)
            {
                if (allGameplayObjects[i].Active)
                {
                   
                    // determine how far they are going to move
                    Vector2 movement = allGameplayObjects[i].Velocity * elapsedTime;
                    // only allow collisionManager that have not collided yet 
                    // collisionManager frame to collide
                    // -- otherwise, objects can "double-hit" and trade their momentum
                    if (allGameplayObjects[i].CollidedThisFrame == false)
                    {
                        movement = MoveAndCollide(allGameplayObjects[i], movement);
                    }
                    // determine the new position
                    allGameplayObjects[i].Position += movement;
                }
            }
            RemoveDeadObjects();
        }
        /// <summary>
        /// Move the given gameplayObject by the given movement, colliding and adjusting
        /// as necessary.
        /// </summary>
        /// <param name="gameplayObject">The gameplayObject who is moving.</param>
        /// <param name="movement">The desired movement vector for this update.</param>
        /// <returns>The movement vector after considering all collisions.</returns>
        private Vector2 MoveAndCollide(GameplayObject gameplayObject,
            Vector2 movement)
        {
           
            // make sure we care about wehere this gameplayObject goes
            if (!gameplayObject.Active)
            {
                return movement;
            }
            // make sure the movement is significant
            if (movement.LengthSquared() <= 0f)
            {
                return movement;
            }

            // generate the list of collisions
            Collide(gameplayObject, movement);

            // determine if we had any collisions
            if (this.collisionResults.Count > 0)
            {
                this.collisionResults.Sort(CollisionResult.Compare);
                foreach (CollisionResult collision in this.collisionResults)
                {
                    // let the two objects touch each other, and see what happens
                    if (gameplayObject.Touch(collision.GameplayObject) &&
                        collision.GameplayObject.Touch(gameplayObject))
                    {
                        gameplayObject.CollidedThisFrame =
                            collision.GameplayObject.CollidedThisFrame = true;
                        // they should react to the other, even if they just died
                        AdjustVelocities(gameplayObject, collision.GameplayObject);
                        return Vector2.Zero;
                    }
                }
            }


            return movement;
        }

        /// <summary>
        /// Determine all collisions that will happen as the given gameplayObject moves.
        /// </summary>
        /// <param name="gameplayObject">The gameplayObject that is moving.</param>
        /// <param name="movement">The gameplayObject's movement vector.</param>
        /// <remarks>The results are stored in the cached list.</remarks>
        public void Collide(GameplayObject gameplayObject, Vector2 movement)
        {
           

            this.collisionResults.Clear();

           
            if (!gameplayObject.Active)
            {
                return;
            }

            // determine the movement direction and scalar
            float movementLength = movement.Length();
            if (movementLength <= 0f)
            {
                return;
            }

            // check each gameplayObject
            foreach (GameplayObject checkActor in allGameplayObjects)
            {
                if ((gameplayObject == checkActor) || !checkActor.Active)
                {
                    continue;
                }
                bool nocollide = false;
                foreach (int i in gameplayObject.NoCollide)
                {
                    if (checkActor.Type == i)
                    {
                        nocollide = true;
                    }
                }
                foreach (int i in checkActor.NoCollide)
                {
                    if (gameplayObject.Type == i)
                    {
                        nocollide = true;
                    }
                }


                if (nocollide == true)
                {
                    continue;
                }

                // calculate the target vector
                float combinedRadius = checkActor.Radius + gameplayObject.Radius;
                Vector2 checkVector = checkActor.Position - gameplayObject.Position;
                float checkVectorLength = checkVector.Length();
                if (checkVectorLength <= 0f)
                {
                    continue;
                }

                float distanceBetween = MathHelper.Max(checkVectorLength -
                    (checkActor.Radius + gameplayObject.Radius), 0);

                // check if they could possibly touch no matter the direction
                if (movementLength < distanceBetween)
                {
                    continue;
                }

                // determine how much of the movement is bringing the two together
                float movementTowards = Vector2.Dot(movement, checkVector);

                // check to see if the movement is away from each other
                if (movementTowards < 0f)
                {
                    continue;
                }

                if (movementTowards < distanceBetween)
                {
                    continue;
                }

                CollisionResult result = new CollisionResult();
                result.Distance = distanceBetween;
                result.Normal = Vector2.Normalize(checkVector);
                result.GameplayObject = checkActor;

                this.collisionResults.Add(result);
            }
        }

        /// <summary>
        /// Adjust the velocities of the two collisionManager as if they have collided,
        /// distributing their velocities according to their masses.
        /// </summary>
        /// <param name="actor1">The first gameplayObject.</param>
        /// <param name="actor2">The second gameplayObject.</param>
        private void AdjustVelocities(GameplayObject actor1,
            GameplayObject actor2)
        {
            // don't adjust velocities if at least one has negative mass
            if ((actor1.Mass <= 0f) || (actor2.Mass <= 0f))
            {
                return;
            }

            // determine the vectors normal and tangent to the collision
            Vector2 collisionNormal = actor2.Position - actor1.Position;
            if (collisionNormal.LengthSquared() > 0f)
            {
                collisionNormal.Normalize();
            }
            else
            {
                return;
            }

            Vector2 collisionTangent = new Vector2(
                -collisionNormal.Y, collisionNormal.X);

            // determine the velocity components along the normal and tangent vectors
            float velocityNormal1 = Vector2.Dot(actor1.Velocity, collisionNormal);
            float velocityTangent1 = Vector2.Dot(actor1.Velocity, collisionTangent);
            float velocityNormal2 = Vector2.Dot(actor2.Velocity, collisionNormal);
            float velocityTangent2 = Vector2.Dot(actor2.Velocity, collisionTangent);

            // determine the new velocities along the normal
            float velocityNormal1New = ((velocityNormal1 * (actor1.Mass - actor2.Mass))
                + (2f * actor2.Mass * velocityNormal2)) / (actor1.Mass + actor2.Mass);
            float velocityNormal2New = ((velocityNormal2 * (actor2.Mass - actor1.Mass))
                + (2f * actor1.Mass * velocityNormal1)) / (actor1.Mass + actor2.Mass);

            // determine the new total velocities
            actor1.Velocity = (velocityNormal1New * collisionNormal) +
                (velocityTangent1 * collisionTangent);
            actor2.Velocity = (velocityNormal2New * collisionNormal) +
                (velocityTangent2 * collisionTangent);
        }

        private void RemoveDeadObjects()
        {
            for (int i =0; i<killList.Count; i++)
            {
                GameplayObject o = KillList[i];
                killList.Remove(o);
                allGameplayObjects.Remove(o);
                o = null;
                
            }
        }

    }
}
