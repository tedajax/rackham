
#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Tanks
{
    /// <summary>
    /// A base public class for all gameplay objects.
    /// </summary>
    abstract public class GameplayObject
    {
        #region Status Data

       

        protected List<int> nocollide = new List<int>();
        public List<int> NoCollide
        {
            get { return nocollide; }
        }

        protected int type = 0;
        public int Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// If true, the object is active in the world.
        /// </summary>
        protected bool active = false;
        public bool Active
        {
            get { return active; }
        }


        #endregion


        #region Graphics Data


        protected Vector2 position = Vector2.Zero;
        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
            }
        }

        protected Vector2 velocity = Vector2.Zero;
        public Vector2 Velocity
        {
            get { return velocity; }
            set
            {
                if ((value.X == Single.NaN) || (value.Y == Single.NaN))
                {
                    throw new ArgumentException("Velocity was NaN");
                }
                velocity = value;
            }
        }

        protected float rotation = 0f;
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }


        #endregion


        #region Collision Data


        protected float radius = 1f;
        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        protected float mass = 1f;
        public float Mass
        {
            get { return mass; }
            set { mass = value; }
        }

        protected bool collidedThisFrame = false;
        public bool CollidedThisFrame
        {
            get { return collidedThisFrame; }
            set { collidedThisFrame = value; }
        }


        #endregion


        #region Initialization Methods


        /// <summary>
        /// Constructs a new gameplay object.
        /// </summary>
        protected GameplayObject() { }


        /// <summary>
        /// Initialize the object to it's default gameplay states.
        /// </summary>
        public virtual void Initialize()
        {
            Collision.AddGamePlayObject(this);
            if (!active)
            {
                active = true;
            }
        }


        #endregion


        #region Updating Methods


        #endregion


        #region Interaction Methods

        public virtual bool Touch(GameplayObject target)
        {
            return true;
        }
        
        public virtual void BoundingSphereTouch(int type)
        {
        }


        public virtual void HitBoundry()
        {
            
        }


        public virtual bool Damage(GameplayObject source, float damageAmount)
        {
            return false;
        }

        public virtual void Die(GameplayObject source, bool cleanupOnly)
        {
            // deactivate the object
            if (active)
            {
                active = false;
                //CollisionManager.Collection.QueuePendingRemoval(this);
            }
        }

        #endregion 




    }
}
