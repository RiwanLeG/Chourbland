using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace Chourbland
{
    public class Agent
    {
        // Percepts - liste des capteurs de l'agent
        public List<String> percepts = new List<String>(new string[] { "smell", "wind", "shine" });

        // Intentions - Liste des actions disponibles pour l'agent
        public List<String> intentions = new List<String>(new string[] { "up", "down", "left", "right", "shoot" });

        // Desire - Trouver le portail
        public Object portal_searched = new Object();

        //Beliefs
        public Case[,] beliefs = new Case[,]{};

        // Podition de l'agent
        public Tuple<int, int> pos_agent = new Tuple<int, int>(0, 0);

        public int performance_indicator = 0;

        public Agent(int length, int width, Case initialposition_case,Tuple<int, int> initialPos)
        {
            pos_agent = initialPos;
            beliefs = new Case[length, width];
            for (int column = 0; column < width; column++)
            {
                for (int row = 0; row < length; row++)
                {
                    Case new_case = new Case();
                    beliefs[column, row] = new_case;
                }
            }

            beliefs[initialPos.Item1, initialPos.Item2] = initialposition_case;

            /*Console.WriteLine("Après affectation : ");
            beliefs[initialPos.Item1, initialPos.Item2].Display_case();*/
            /*beliefs[this.pos_agent.Item1, this.pos_agent.Item1]*/
            Console.WriteLine("I'm aliiiiiive");
        }
        public Agent() { }

        public void Shoot_rock(Tuple<int, int> target_pos)
        {
            /*            if element on the cell == "monster"
                            kill_monster(target_pos);
            */
        }

        // On position l'agent
        public void Set_agent_position(Case a_case, Tuple<int,int> agent_position)
        {
            // On lui transmets les informations de la case surlaquelle il se trouve
            beliefs[agent_position.Item1, agent_position.Item2] = a_case;

            // Mis à jour de la position de l'agent
            pos_agent = agent_position;

            // La case a été visitée
            beliefs[agent_position.Item1, agent_position.Item2].Set_visited(true);

            Console.WriteLine("Set the agent position : " + pos_agent);
        }

        // Met à jour toutes les cases à côté de l'agent en fonction de sa case
        /*public void Update_all_unknown_adjacent_cases(Tuple<int, int> currentCasePos, Case[,] currentGrid, string field, float value)*/
        public void Update_all_unknown_adjacent_cases(Tuple<int, int> currentCasePos,  string field, float value)
        {
            int x = currentCasePos.Item1;
            int y = currentCasePos.Item2;

            int number_candidate = 0;

            for (int dx = -1; dx <= 1; ++dx)
            {
                for (int dy = -1; dy <= 1; ++dy)
                {
                    int xdx = x + dx;
                    int ydy = y + dy;
                    //On vérifie bien qu'on ne sort pas de la grille
                    if ((xdx < 0) || (xdx > beliefs.GetLength(0)-1) || (ydy < 0) ||
                        (ydy > beliefs.GetLength(1)-1))
                    {
                        continue;
                    }
                    Case candidate = beliefs[xdx,ydy];
                    if (((dx != 0 && dy == 0) || (dx == 0 && dy != 0)) && !candidate.Get_Visited())
                    {
                        candidate.Set_border(true);
                        /*candidate.Display_case();*/
                        //Console.WriteLine("Case(" + xdx + "," + ydy + "): monster:" + candidate.Get_Monster() + "; cliff:" + candidate.Get_Cliff() + "; portal:" + candidate.Get_Portal());
                        if (field == "monster")
                        {
                            candidate.Set_Monster(value);
                        }
                        if (field == "cliff")
                        {
                            candidate.Set_Cliff(value);
                        }
                        if (field == "portal")
                        {
                            candidate.Set_Portal(value);
                        }
                        /*candidate.Display_case();*/
                        //Console.WriteLine("then Case("+xdx+","+ydy+"): monster:"+candidate.Get_Monster()+"; cliff:"+candidate.Get_Cliff()+"; portal:"+candidate.Get_Portal());
                    }
                }
            }
            /*Console.WriteLine("number_candidate : " + number_candidate);*/
        }
        public static Tuple<int, int> CoordinatesOf( Case[,] grid, Case box)
        {
            int w = grid.GetLength(0); // width
            int h = grid.GetLength(1); // height

            for (int x = 0; x < w; ++x)
            {
                for (int y = 0; y < h; ++y)
                {
                    if (grid[x, y].Equals(box))
                        return Tuple.Create(x, y);
                }
            }

            return Tuple.Create(-1, -1);
        }
        // Chainage avant
        public void Forward_chaining()
        {
            Console.WriteLine("Forward chaining");
            var x = pos_agent.Item1;
            var y = pos_agent.Item2;
            var currentCase = beliefs[x, y];

            Console.WriteLine("Case actuelle : "+x+" "+y);
            Console.WriteLine("Case actuelle : " + beliefs[x,y].GetType());
            Console.WriteLine("Case actuelle : smell: " + beliefs[pos_agent.Item1, pos_agent.Item2].Get_Smell()+"; wind:"+ beliefs[pos_agent.Item1, pos_agent.Item2].Get_Wind() + "; light:" + beliefs[x, y].Get_Light());
            if (beliefs[x, y].Get_Smell())
            {
                Console.WriteLine("There is a monster nearby");
                Update_all_unknown_adjacent_cases(pos_agent, "monster", 1f);
            }
            if (beliefs[x, y].Get_Wind())
            {
                Console.WriteLine("There is a cliff nearby");
                Update_all_unknown_adjacent_cases(pos_agent, "cliff", 1f);
            }
            if (beliefs[x, y].Get_Light())
            {
                Console.WriteLine("There is a portal nearby");
                Update_all_unknown_adjacent_cases(pos_agent, "portal", 1f);
                Update_all_unknown_adjacent_cases(pos_agent, "monster", 0f);
                Update_all_unknown_adjacent_cases(pos_agent, "cliff", 0f);
            }

            // Si il n'y a rien
            if((!currentCase.Get_Wind()) && (!currentCase.Get_Light()) && (!currentCase.Get_Smell()))
            {
                Update_all_unknown_adjacent_cases(pos_agent, "portal", 0f);
                Update_all_unknown_adjacent_cases(pos_agent, "monster", 0f);
                Update_all_unknown_adjacent_cases(pos_agent, "cliff", 0f);
            }
        }

        public Tuple<int,int> Move_agent()
        {
            Console.WriteLine("Move");
            Tuple<int,int> next_pos_agent = new Tuple<int, int> (0,0);
            float safest = 1.0f;
            int number_iteration = 0;
            foreach (Case box in beliefs)
            {

                //if ((box.Get_border() == true)&&(box.Get_Visited() == false))
                /*Console.WriteLine("box.Get_border() : " + box.Get_border());*/
                /*Console.WriteLine("box.Get_Visited() : " + box.Get_Visited());*/
                if ((box.Get_border() == true) && (box.Get_Visited() == false))
                {
                    number_iteration++;
                    next_pos_agent = CoordinatesOf(beliefs, box);
                    Console.WriteLine("box.Get_Visited() : " + next_pos_agent);
                    if (box.Get_Monster() < safest)
                    {
                        safest = box.Get_Monster();
                        next_pos_agent = CoordinatesOf(beliefs, box);
                        Console.WriteLine("Position monstre ! ");

                    }
                    else if (box.Get_Cliff() < safest)
                    {
                        safest = box.Get_Cliff();
                        next_pos_agent = CoordinatesOf(beliefs, box);
                        Console.WriteLine("Position falaise ! ");
                    }

                } //else random between borders                
            }

            List<Case> Unknown_adjacent_cases = new List<Case>();
            int x = next_pos_agent.Item1;
            int y = next_pos_agent.Item2;
            for (int dx = -1; dx <= 1; ++dx)
            {
                for (int dy = -1; dy <= 1; ++dy)
                {
                    int xdx = x + dx;
                    int ydy = y + dy;
                    if ((xdx < 0) || (xdx > beliefs.GetLength(0) - 1) || (ydy < 0) || (ydy > beliefs.GetLength(1) - 1))
                    {
                        continue;
                    }
                    Case candidate = beliefs[xdx, ydy];
                    if ((dx != 0 && dy == 0) || (dx == 0 && dy != 0) && !candidate.Get_Visited())
                    {
                        candidate.Set_border(true);
                    }
                }
            }
/*            Console.WriteLine("next_pos_agent : " + next_pos_agent);
            Console.WriteLine("number_iteration : " + number_iteration);*/
            return next_pos_agent;
        }


        // Fonction pour lire le JSON

        public JObject Load_Json()
        {

            string project_location = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            string path = project_location + @"\..\..\Rules.json";

            Console.WriteLine("path : " + path);
            // read JSON directly from a file
            using (StreamReader file = File.OpenText(path))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject o2 = (JObject)JToken.ReadFrom(reader);
                foreach (var element in o2)
                {
                    Console.WriteLine("if : " + element.Key);
                    Console.WriteLine("else : " + element.Value["danger"].ToString());
                }
                Console.WriteLine("Number of element in the JObject : " + o2.Count);
                return o2;
            }
        }
    }
}