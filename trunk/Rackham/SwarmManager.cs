using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Tanks
{
    //This is a Manager that holds all the swarms together. Swarms should only be created through this manager!
    class SwarmManager
    {
        List<Swarm> SwarmList = new List<Swarm>();
        HiveQueen Queen;
        public static List<Enemy> EnemiesToDestroy = new List<Enemy>();
        //A Unique Id for each swarm
        int SwarmId;
        //Save the last Id so that you can generate a new unique ID Next Time
        String LastId;

        Random formationGenerator;
        Random r = new Random();

        int enemychosen = 0;
        public SwarmManager()
        {
            SwarmId = 0;
            LastId = "";
            formationGenerator = new Random();
        }

       /// <summary>
       /// adds a new swarm to manage
       /// </summary>
       /// <param name="newswarm">Create A new Swarm and pass it in here</param>
        public void addSwarm(Swarm newswarm, GameTime gameTime)
        {
            //This Creates a Unique ID for each swarm by taking the current time and adding in a Id Marker (Incase multiple swarms are created at the sametime)
            if (LastId.Equals(gameTime.TotalGameTime.ToString()))
            {
                SwarmId++;
            }
            else SwarmId = 0;
            LastId = gameTime.TotalGameTime.ToString();
            String NewId = gameTime.TotalGameTime.ToString() + SwarmId.ToString();
            newswarm.setId(NewId);
            //Adds the Swarm to the main list of swarms
            SwarmList.Add(newswarm);
        }
        /// <summary>
        /// This function should not be called unless you are calling it from LoadGameContent. Use the overload for better results
        /// </summary>
        /// <param name="newswarm">swarm to add (WARNING::DONT USE THIS UNLESS YOU HAVE TO, USE THE OVERLOAD)</param>
        public void addSwarm(Swarm newswarm)
        {
            newswarm.setId(SwarmId.ToString());
            SwarmId++;
            SwarmList.Add(newswarm);
        }
            

        public void Update(GameTime gameTime, List<Player> PlayerList, BulletManager BM)
        {
            cleanUpSwarm();
            if (PlayerList.Count > 0)
            {
                if (PlayerList[0].getReadyState() == 6)
                {
                    List<Swarm> SurroundList = new List<Swarm>();
                    for (int i = 0; i < SwarmList.Count; i++)
                    {
                        if (i > 0)
                            SurroundList.Add(SwarmList[i]);
                        else
                        {
                            SwarmList[i].moveSwarm(PlayerList[0].Position);
                            SwarmList[i].setRadius(80);
                        }
                    }
                    if (SurroundList.Count > 0)
                    {

                        float angle = 0;
                        float addangle = 360 / SurroundList.Count;
                        if (enemychosen >= SurroundList.Count)
                        {
                            enemychosen = 0;
                        }
                        for (int i = 0; i < SurroundList.Count; i++)
                        {

                            Swarm s = SurroundList[i];
                            if (i == enemychosen)
                            {

                                s.moveSwarm(PlayerList[0].Position);
                                s.outwardcircle = 30f;
                                s.comprimise = 3 / 6;
                                if (s.State == "IDLE")
                                    enemychosen = formationGenerator.Next(0, SurroundList.Count - 1);

                            }
                            else
                            {
                                s.moveSwarm(PlayerList[0].Position + (250 * new Vector2((float)Math.Cos(MathHelper.ToRadians(angle)), (float)Math.Sin(Math.Sin(MathHelper.ToRadians(angle))))));
                            }
                            angle += addangle;

                        }
                    }
                }
            }
            
            for (int i =0; i<SwarmList.Count; i++)
             {
                Swarm s = SwarmList[i];
                if (s.EnemiesInSwarm.Count == 0)
                {
                    SwarmList.Remove(s);
                }
                else
                 {
                    s.Update(gameTime, PlayerList);
                }
            }
        }

        public void MergeSwarm(Swarm swarm1, Swarm swarm2)
        {
            foreach (Enemy e in swarm2.EnemiesInSwarm)
            {
                swarm1.AddEnemy(e);
                EnemiesToDestroy.Add(e);
            }
        }

        public void cleanUpSwarm()
        {
            foreach (Enemy e in EnemiesToDestroy)
            {
                Collision.KillList.Add(e);
                int i = 0;
                while (i < SwarmList.Count)
                {
                    Swarm swarm = SwarmList[i];
                    i++;
                    if (swarm.getId().Equals(e.mySwarmId))
                    {                       
                        swarm.LoseEnemy(e);
                        if (swarm.EnemiesInSwarm.Count <= 0)
                            swarm.KillThisSwarm = true;
                        i = SwarmList.Count;
                    }
                    
                }

            }
            EnemiesToDestroy.Clear();

            for (int i = SwarmList.Count - 1; i >= 0; i--)
            {
                if (SwarmList[i].KillThisSwarm)
                    SwarmList.Remove(SwarmList[i]);
            }
        }

        public void DrawSwarms(Vector3 CameraPosition, float aspectRatio)
        {
            foreach (Swarm s in SwarmList)
            {
                foreach (Enemy e in s.EnemiesInSwarm)
                {
                    e.Draw(CameraPosition, aspectRatio);
                }
            }
        }

        public Swarm getSwarm(int Index)
        {
            if (Index < SwarmList.Count)
            {
                return SwarmList[Index];
            }
            return null;
        }

        public int CountSwarms()
        {
            return SwarmList.Count;
        }
    }
}
