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
    class Swarm
    {
        private String MyId = "NoId";

        public Vector2 Position;
        public Vector2 Velocity;

        public List<Enemy> EnemiesInSwarm;
        public int EnemyCount;

        public BoundingSphere SwarmSightSphere;

        private TimeSpan MovementTimer;

        public string State;

        private double angleOffset = 0;

        Random MovementRandomizer;

        public Swarm(Vector2 pos, Vector2 vel, List<Enemy> elist)
        {
            Position = pos;
            Velocity = vel;

            EnemiesInSwarm = elist;
            EnemyCount = EnemiesInSwarm.Count;

            Initialize();
        }

        private void Initialize()
        {
            EnemyCount = EnemiesInSwarm.Count;

            SwarmSightSphere = new BoundingSphere(new Vector3(Position.X, 0f, Position.Y), (float)(EnemyCount * 10));
            State = "Idle";

            foreach (Enemy e in EnemiesInSwarm)
            {
                e.InSwarm = true;
                e.mySwarmId = this.MyId;
            }

            MovementRandomizer = new Random();
        }

        public void AddEnemy(Enemy NewEnemy)
        {
            EnemiesInSwarm.Add(NewEnemy);
            NewEnemy.InSwarm = true;
            //Give Every Enemy an Id that references it back to this swarm
            NewEnemy.mySwarmId = MyId;
            EnemyCount++;
        }

        public void LoseEnemy(int index)
        {
            EnemiesInSwarm[index].InSwarm = false;
            EnemiesInSwarm[index].mySwarmId = "NoId";
            EnemiesInSwarm.Remove(EnemiesInSwarm[index]);
        }

        public void LoseEnemy(Enemy enemy)
        {
            if (EnemiesInSwarm.Remove(enemy))
            {
                enemy.InSwarm = false;
                enemy.mySwarmId = "NoId";
            }
        }
        /// <summary>
        /// This functions updates all the enemies inside its swarm and tells them where to go and what to do.
        /// </summary>
        /// <param name="GameTime">gameTime needs to be passed in here to ensure timer based movement</param>
        /// <param name="PlayerList">A list of players created inside GamePlay.cs is needed for some odd reason</param>
        public void Update(GameTime GameTime, List<Player> PlayerList)
        {
            SwarmSightSphere.Radius = (float)EnemyCount * 10f;
            if (State.ToUpper().Equals("MOVE"))
            {
                int numberofenemieswithindist = 0;
                float distancetobewithin = 5f;
                foreach (Enemy e in EnemiesInSwarm)
                {
                    e.Target.X = Position.X + MovementRandomizer.Next(-15,15);
                    e.Target.Y = Position.Y + MovementRandomizer.Next(-15,15);

                    float distance = (float) Math.Sqrt(Math.Pow(e.Position.Y - e.Target.Y, 2) + Math.Pow(e.Position.X - e.Target.X, 2));
                    if (distance <= distancetobewithin)
                    {
                        numberofenemieswithindist++;
                    }

                    e.Update(GameTime);
                }
                if (numberofenemieswithindist > (EnemiesInSwarm.Count * 4) / 5)
                {
                    State = "IDLE";
                }
            }
            else if (State.ToUpper().Equals("PROTECT"))
            {
                if (GameTime.TotalGameTime.Milliseconds > MovementTimer.Milliseconds)
                {
                    TimeSpan tempspan = new TimeSpan(0, 0, 0, 0, 200);
                    MovementTimer = GameTime.TotalGameTime.Add(tempspan);
                    int i = 0;
                    double angle = 360 / EnemyCount;
                    foreach (Enemy e in EnemiesInSwarm)
                    {
                        
                        float ox = (float)Math.Sin(MathHelper.ToRadians((float)((angle * i) + angleOffset))) * 10;//(SwarmSightSphere.Radius / 5);
                        float oy = (float)Math.Cos(MathHelper.ToRadians((float)((angle * i) + angleOffset))) * 10;//(SwarmSightSphere.Radius / 5);

                        e.Target.X = ox + Position.X;
                        e.Target.Y = oy + Position.Y;

                        e.Update(GameTime);

                        i++;
                        angleOffset += 0.05;
                        if (angleOffset > 359)
                            angleOffset = 0;
                    }
                }
            }

            else if (State.ToUpper().Equals("IDLE"))
            {
                
                    TimeSpan tempspan = new TimeSpan(0, 0, 0, 0, 200);
                    MovementTimer.Add(tempspan);// = GameTime.TotalGameTime.Milliseconds + tempspan.TotalMilliseconds;
                    int i = 0;
                    double angle = 360 / EnemyCount;
                    foreach (Enemy e in EnemiesInSwarm)
                    {
                        i = MovementRandomizer.Next(0, 360);
                        float ox = (float)Math.Sin(MathHelper.ToRadians((float)(i))) *5;//(SwarmSightSphere.Radius / 5);
                        float oy = (float)Math.Cos(MathHelper.ToRadians((float)(i))) *5;//(SwarmSightSphere.Radius / 5);
                        if (((Vector2)(e.Position - e.Target)).Length() < 1)
                        {
                            e.Target.X = ox + Position.X;
                            e.Target.Y = oy + Position.Y;
                        }

                        e.Update(GameTime);

                        i++;
                        angleOffset += 0.05;
                        if (angleOffset > 359)
                            angleOffset = 0;
                    }
                
            }

            SwarmSightSphere.Center = new Vector3(Position.X, 0f, Position.Y);
        }

        public String getId() { return MyId; }
        public void setId(String Id) 
        { 
            MyId = Id;
            foreach (Enemy e in EnemiesInSwarm)
            {
                e.mySwarmId = Id;
            }
        
        }
        /// <summary>
        /// Tells the swarm to move all its units to a new position
        /// </summary>
        /// <param name="newPosition">position to move to</param>
        public void moveSwarm(Vector2 newPosition)
        {
            Position = newPosition;
            State = "IDLE";
        }
    }
}
