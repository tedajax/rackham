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
        public Vector2 AvgPosition;
        public Vector2 Velocity;

        public float outwardcircle = 25;
        public float comprimise = 3 / 5;
        
        public float radius = 10;
        private float modifiedradius = 10;

        private bool canburst = true;
        public TimeSpan BurstTimer = new TimeSpan();
        public TimeSpan MaxBurstTime = new TimeSpan();
        public TimeSpan NextChange = new TimeSpan();
        public TimeSpan ChangeFormation = new TimeSpan();

        private TimeSpan MovementTimer;

        public List<Enemy> EnemiesInSwarm;
        public int EnemyCount;

        public BoundingSphere SwarmSightSphere;

        public string State;

        double angle = 0;
        private double angleOffset = 0;

        Random MovementRandomizer;

        public Vector2 direction;

        public bool KillThisSwarm = false;

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
            SwarmSightSphere.Radius = 2f;
            AvgPosition = Vector2.Zero;

            foreach (Enemy e in EnemiesInSwarm)
            {
                AvgPosition.X += e.Position.X;
                AvgPosition.Y += e.Position.Y;
            }

            AvgPosition.X /= EnemiesInSwarm.Count;
            AvgPosition.Y /= EnemiesInSwarm.Count;
            BurstTimer += GameTime.ElapsedGameTime;
            if (canburst == false && BurstTimer >= MaxBurstTime) canburst = true;
            if (State.ToUpper().Equals("BURST"))
            {
               
                if (BurstTimer >= MaxBurstTime)
                {
                    State = "MOVE";
                    BurstTimer = new TimeSpan();
                }
                foreach (Enemy e in EnemiesInSwarm)
                {
                    foreach (Enemy eout in EnemiesInSwarm)
                    {
                        Vector2 outmove = new Vector2();
                        outmove += eout.Velocity;
                        outmove.X = eout.Position.X - this.AvgPosition.X;
                        outmove.Y = eout.Position.Y - this.AvgPosition.Y;
                        if (Math.Abs(outmove.X) < 30 && Math.Abs(outmove.Y) < 30)
                        {
                            outmove = Vector2.Normalize(outmove) / 10;
                            eout.Velocity = outmove;
                        }
                    }
                }
            }

            else if (State.ToUpper().Equals("MOVE") || State.ToUpper().Equals("IDLE"))
            {
                
                
                // = GameTime.TotalGameTime.Milliseconds + tempspan.TotalMilliseconds;
                

                float i = 0;
               // 
                
                double angleadd = 360 / EnemyCount;
                MovementTimer += GameTime.ElapsedGameTime;
                if (MovementTimer.TotalMilliseconds > NextChange.TotalMilliseconds)
                {
                    angle += MovementRandomizer.Next(0,360);
                    NextChange = new TimeSpan(0, 0, 0, 0, MovementRandomizer.Next(400, 1000));
                    MovementTimer = new TimeSpan(0, 0, 0, 0,0);
                }
              

                int count = 0;
                double newangle = angle;
                foreach (Enemy e in EnemiesInSwarm)
                {

                   i = (float) newangle;
                    newangle += angleadd;
                    if (e.Radius != this.radius) e.Radius = this.radius;
                    if (MovementTimer == new TimeSpan(0,0,0)) e.ModifiedRadius = e.Radius * (MovementRandomizer.Next(80, 120) / 100); 
                 
                   // float oldox = e.Target - Position.X;
                   float ox = (float)Math.Cos(i) * 25;//(SwarmSightSphere.Radius / 5);
                   float oy = (float)Math.Sin(i) * 25;//(SwarmSightSphere.Radius / 5);
                   ox = (float)Math.Cos(i) *e.ModifiedRadius;
                   oy = (float)Math.Sin(i) *e.ModifiedRadius;
                    e.Target.X = ox + Position.X;
                    e.Target.Y = oy + Position.Y;
                    
                    if (Math.Sqrt((Math.Pow(e.Position.Y - Position.Y, 2)) + Math.Pow(e.Position.X - Position.X, 2)) < outwardcircle)
                    {
                        count++;
                    }

                    e.Update(GameTime);

                    i++;
                    /*angleOffset += 0.05;
                    if (angleOffset > 359)
                        angleOffset = 0;*/
                    
                }

                if (count > EnemiesInSwarm.Count * comprimise)
                    State = "IDLE";

            }

            else if (State.ToUpper().Equals("IDLE"))
            {
                /*
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
                */
            }
            else if (State.ToUpper().Equals("DEFEND"))
            {


                // = GameTime.TotalGameTime.Milliseconds + tempspan.TotalMilliseconds;


                float i = 0;
                // 

                double angleadd = 360 / EnemyCount;


                int count = 0;
                double newangle = angle;
                foreach (Enemy e in EnemiesInSwarm)
                {

                    i = (float)newangle;
                    newangle += angleadd;
                    if (e.Radius != this.radius) e.Radius = this.radius;
                    if (MovementTimer == new TimeSpan(0, 0, 0)) e.ModifiedRadius = e.Radius * (MovementRandomizer.Next(80, 120) / 100);

                    // float oldox = e.Target - Position.X;
                    float ox = (float)Math.Cos(i) * 25;//(SwarmSightSphere.Radius / 5);
                    float oy = (float)Math.Sin(i) * 25;//(SwarmSightSphere.Radius / 5);
                    ox = (float)Math.Cos(newangle) * e.ModifiedRadius;
                    oy = (float)Math.Sin(newangle) * e.ModifiedRadius;
                    e.Target.X = ox + Position.X;
                    e.Target.Y = oy + Position.Y;


                    e.Update(GameTime);

                    i++;

                }

                if (count > EnemiesInSwarm.Count * comprimise)
                    State = "IDLE";

            }

            else if (State.ToUpper().Equals("MARCH"))
            {

                Vector2 positionabove = Position + direction;
                direction.Normalize();
                int enemiesplaced = 0;
                int totalenemiesplaced = 0;
                float rowmodifier = -10;
                if (EnemiesInSwarm.Count - totalenemiesplaced < 5)
                {
                    rowmodifier = -5 * (((EnemiesInSwarm.Count - totalenemiesplaced) - 1) / 2);
                }

                float rowrandomer = -1;
                MovementTimer += GameTime.ElapsedGameTime;
                rowrandomer = -1;
                if (MovementTimer.TotalMilliseconds > 600)
                {
                    rowrandomer = 1;

                }
                if (MovementTimer.TotalMilliseconds > 1200)
                {
                    MovementTimer = new TimeSpan(0, 0, 0);
                }
                foreach (Enemy e in EnemiesInSwarm)
                {
                    e.Target = positionabove + new Vector2(rowmodifier + rowrandomer, 0);
                    rowmodifier += 5f;
                    enemiesplaced++;
                    totalenemiesplaced++;
                    if (enemiesplaced > 5)
                    {
                        enemiesplaced = 0;
                        positionabove += direction * 5;
                        rowmodifier = -10;
                        if (EnemiesInSwarm.Count - totalenemiesplaced < 5)
                        {
                            rowmodifier = -5 * (((EnemiesInSwarm.Count - totalenemiesplaced) - 1) / 2);
                        }
                        rowrandomer *= -1;
                    }
                    e.Update(GameTime);
                }
            }

            ChangeFormation += GameTime.ElapsedGameTime;

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
            if (Position != newPosition && State !="BURST")
            {
                
                Position = newPosition;
                State = "MOVE";
            }
         
        }

        public void burstSwarm(TimeSpan timer)
        {
            if (canburst)
            {
                State = "BURST";
                BurstTimer = new TimeSpan();
                MaxBurstTime = timer;
            }
        }

        public void defendSwarm(Vector2 pos, float rad)
        {
            State = "DEFEND";
            Position = pos;
            radius = rad;
        }

        public void marchSwarm(Vector2 position, Vector2 Direction)
        {
            this.Position = position;
            this.direction = Direction;
            State = "MARCH";
        }

        public void setRadius(float R)
        {
            radius = R;
            modifiedradius = R;
        }

    }
}
