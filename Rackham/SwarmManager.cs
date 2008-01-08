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
        public static List<Enemy> EnemiesToDestroy = new List<Enemy>();
        //A Unique Id for each swarm
        int SwarmId;
        //Save the last Id so that you can generate a new unique ID Next Time
        String LastId;


        public SwarmManager()
        {
            SwarmId = 0;
            LastId = "";

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
            

        public void Update(GameTime gameTime, List<Player> PlayerList)
        {
            cleanUpSwarm();
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

        public void cleanUpSwarm()
        {
            foreach (Enemy e in EnemiesToDestroy)
            {
                int i = 0;
                while (i < SwarmList.Count)
                {
                    Swarm swarm = SwarmList[i];
                    i++;
                    if (swarm.getId().Equals(e.mySwarmId))
                    {
                        swarm.LoseEnemy(e);
                        i = SwarmList.Count;
                    }
                    
                }

            }
            EnemiesToDestroy.Clear();
        }
    }
}
