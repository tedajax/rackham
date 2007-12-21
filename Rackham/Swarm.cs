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
                e.InSwarm = true;

            MovementRandomizer = new Random();
        }

        public void AddEnemy(Enemy NewEnemy)
        {
            EnemiesInSwarm.Add(NewEnemy);
            NewEnemy.InSwarm = true;
            EnemyCount++;
        }

        public void LoseEnemy(int index)
        {
            EnemiesInSwarm[index].InSwarm = false;
            EnemiesInSwarm.Remove(EnemiesInSwarm[index]);
        }

        public void Update(GameTime GameTime, List<Player> PlayerList)
        {
            SwarmSightSphere.Radius = (float)EnemyCount * 10f;

            if (State.ToUpper().Equals("IDLE"))
            {
                if (GameTime.TotalGameTime.Milliseconds > MovementTimer.Milliseconds)
                {
                    TimeSpan tempspan = new TimeSpan(0, 0, 0, 0, 200);
                    MovementTimer.Add(tempspan);// = GameTime.TotalGameTime.Milliseconds + tempspan.TotalMilliseconds;
                    int i = 0;
                    double angle = 360 / EnemyCount;
                    foreach (Enemy e in EnemiesInSwarm)
                    {
                        float ox = (float)Math.Sin(MathHelper.ToRadians((float)((angle * i) + angleOffset))) * 10;//(SwarmSightSphere.Radius / 5);
                        float oy = (float)Math.Cos(MathHelper.ToRadians((float)((angle * i) + angleOffset))) * 10;//(SwarmSightSphere.Radius / 5);

                        e.Target.X = ox + Position.X;
                        e.Target.Y = oy + Position.Y;

                        i++;
                        angleOffset += 0.05;
                        if (angleOffset > 359)
                            angleOffset = 0;
                    }
                }
            }

            SwarmSightSphere.Center = new Vector3(Position.X, 0f, Position.Y);
        }
    }
}
