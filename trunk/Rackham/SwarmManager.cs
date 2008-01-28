using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections;
namespace Tanks
{
    //This is a Manager that holds all the swarms together. Swarms should only be created through this manager!
    class SwarmManager
    {
        static int AmountOfEnemiesBeforeAttack = 25;
        bool initialscaredburst = false;
        List<Swarm> SwarmList = new List<Swarm>();
        HiveQueen Queen;

        public int EnemiesIGot;
        public int GeneratorsToDefend;

        public static List<Enemy> EnemiesToDestroy = new List<Enemy>();
        //A Unique Id for each swarm
        int SwarmId;
        //Save the last Id so that you can generate a new unique ID Next Time
        String LastId;

        Random formationGenerator;
        Random r = new Random();

        int enemychosen = 0;
        public SwarmManager(HiveQueen queenie)
        {
            SwarmId = 0;
            LastId = "";
            formationGenerator = new Random();
            Queen = queenie;
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
            EnemiesIGot += newswarm.EnemiesInSwarm.Count;
        }
        /// <summary>
        /// This function should not be called unless you are calling it from LoadGameContent. Use the overload for better results
        /// </summary>
        /// <param name="newswarm">swarm to add (WARNING::DONT USE THIS UNLESS YOU HAVE TO, USE THE OVERLOAD)</param>
        public void addSwarm(Swarm newswarm)
        {
            newswarm.setId(SwarmId.ToString());
            SwarmId++;
            EnemiesIGot += newswarm.EnemiesInSwarm.Count;
            SwarmList.Add(newswarm);
        }
            

        public void Update(GameTime gameTime, List<Player> PlayerList, BulletManager BM)
        {
            cleanUpSwarm();
            if (PlayerList.Count > 0)
            {
                if (PlayerList[0].getReadyState() == 6)
                {
                    if (SwarmList.Count > 0)
                    {
                        //PHASE 1..COUNT YOUR TROOPS!
                        //PHASE 2..Distribute Your Troops
                        int EnemiesLeftToDist = EnemiesIGot;
                        int SwarmsIGot = SwarmList.Count;
                        Queue<Swarm> SwarmstoDist = new Queue<Swarm>();
                        foreach (Swarm s in SwarmList)
                        {
                            SwarmstoDist.Enqueue(s);
                        }
                        BoundingSphere b = new BoundingSphere(WindowManager.V3FromV2(Queen.Position), 150f);
                        BoundingSphere c = new BoundingSphere(WindowManager.V3FromV2(PlayerList[0].Position), 2.5f);
                        if (b.Intersects(c))
                        {

                            foreach (Swarm s in SwarmList)
                            {
                                if (initialscaredburst == false)
                                {
                                    s.burstSwarm(new TimeSpan(0, 0, 1));

                                }
                                else
                                {

                                    s.moveSwarm(PlayerList[0].Position);
                                    s.setRadius(5);
                                }

                            }
                            initialscaredburst = true;
                        }
                        else
                        {
                            initialscaredburst = false;
                            if (SwarmsIGot <= Queen.Generators.Count)
                            {
                                Swarm newswarm = SwarmstoDist.Dequeue();
                                newswarm.setRadius(15);
                                newswarm.moveSwarm(Queen.Position);
                                SwarmsIGot--;
                                int j = 0;
                                for (int i = 0; i < SwarmsIGot; i++)
                                {
                                    newswarm = SwarmstoDist.Dequeue();
                                    newswarm.setRadius(12);
                                    newswarm.moveSwarm(Queen.Generators[j].Position);
                                    j++;
                                    if (j == Queen.Generators.Count) j = 0;


                                }
                            }
                            else
                            {
                                int swarmsinhalf = Queen.Generators.Count;
                                Swarm newswarm = SwarmstoDist.Dequeue();
                                newswarm.setRadius(10);
                                newswarm.moveSwarm(Queen.Position);
                                SwarmsIGot--;
                                int j = 0;
                                for (int i = 0; i < swarmsinhalf; i++)
                                {
                                    newswarm = SwarmstoDist.Dequeue();
                                    newswarm.setRadius(9);
                                    newswarm.moveSwarm(Queen.Generators[j].Position);
                                    SwarmsIGot--;
                                    j++;
                                    if (j == Queen.Generators.Count) j = 0;

                                }
                                swarmsinhalf = (int)Math.Floor(((double)SwarmsIGot * 3 / 5));
                                for (int i = 0; i < swarmsinhalf; i++)
                                {

                                    newswarm = SwarmstoDist.Dequeue();
                                    newswarm.moveSwarm(PlayerList[0].Position);
                                    newswarm.setRadius(5);
                                    
                                    
                                    SwarmsIGot--;
                                }
                                for (int i = 0; i < SwarmsIGot; i++)
                                {
                                    newswarm = SwarmstoDist.Dequeue();
                                    newswarm.setRadius(9);
                                    newswarm.moveSwarm(Queen.Generators[j].Position);
                                    SwarmsIGot--;
                                    j++;
                                    if (j == Queen.Generators.Count) j = 0;

                                }
                                


                            }
                            foreach (Swarm newswarm in SwarmList)
                            {
                                foreach (DictionaryEntry d in BM.GetBulletHashTable())
                                {
                                    Bullet bullet = (Bullet)d.Value;
                                    BoundingSphere a = new BoundingSphere(WindowManager.V3FromV2(bullet.Position), bullet.Radius);
                                    BoundingSphere a2 = new BoundingSphere(WindowManager.V3FromV2(newswarm.Position), 25);
                                    if (a2.Intersects(a))
                                    {
                                        newswarm.burstSwarm(new TimeSpan(0, 0, 0, 0, 500));
                                        newswarm.Position = PlayerList[0].Position;
                                    }
                                }
                            }

                        }
                    }



                }
                else
                    Queen.Health = 250;
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
                SwarmList.Remove(swarm2);
            }
        }

        public Swarm SplitSwarm(Swarm swarm1, int AmountOfEnemiestoRemove, Vector2 position, GameTime gameTime)
        {
            List<Enemy> EnemyList = new List<Enemy>();
            for (int i = 0; i < AmountOfEnemiestoRemove; i++)
            {
                if (swarm1.EnemiesInSwarm.Count < i)
                {
                    EnemyList.Add(swarm1.EnemiesInSwarm[i]);
                    swarm1.EnemiesInSwarm.Remove(swarm1.EnemiesInSwarm[i]);
                    EnemiesIGot--;
                }
            }
            Swarm newswarm = new Swarm(position, Vector2.Zero, EnemyList);
            addSwarm(newswarm, gameTime);
            return newswarm;
        }

        public void cleanUpSwarm()
        {
            foreach (Enemy e in EnemiesToDestroy)
            {
                Collision.KillList.Add(e);
                EnemiesIGot--;
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
