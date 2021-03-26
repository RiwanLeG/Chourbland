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

        public Agent(int length, int width, Case initialposition,Tuple<int, int> initialPos)
        {
            this.pos_agent = initialPos;
            beliefs = new Case[length, width];
            for (int column = 0; column < width; column++)
            {
                for (int row = 0; row < length; row++)
                {
                    Case new_case = new Case();
                    beliefs[column, row] = new_case;
                }
            }

            beliefs[this.pos_agent.Item1, this.pos_agent.Item1] = initialposition;
            Console.WriteLine("I'm aliiiiiive");
        }

        public void Shoot_rock(Tuple<int, int> target_pos)
        {
            /*            if element on the cell == "monster"
                            kill_monster(target_pos);
            */
        }

        public void Update_all_unknown_adjacent_cases(Tuple<int, int> currentCasePos, Case[,] currentGrid, string field, float value)
        {
            int x = currentCasePos.Item1;
            int y = currentCasePos.Item2;
            for (int dx = -1; dx <= 1; ++dx)
            {
                for (int dy = -1; dy <= 1; ++dy)
                {
                    int xdx = x + dx;
                    int ydy = y + dy;
                    //On vérifie bien qu'on ne sort pas de la grille
                    if ((xdx < 0) || (xdx > currentGrid.GetLength(0)) || (ydy < 0) ||
                        (ydy > currentGrid.GetLength(1)))
                    {
                        continue;
                    }
                    Case candidate = currentGrid[xdx,ydy];
                    if ((dx != 0 && dy == 0) || (dx == 0 && dy != 0) && !candidate.Get_Visited())
                    {
                        Console.WriteLine("Case(" + xdx + "," + ydy + "): monster:" + candidate.Get_Monster() + "; cliff:" + candidate.Get_Cliff() + "; portal:" + candidate.Get_Portal());
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
                        Console.WriteLine("then Case("+xdx+","+ydy+"): monster:"+candidate.Get_Monster()+"; cliff:"+candidate.Get_Cliff()+"; portal:"+candidate.Get_Portal());
                    }
                }
            }
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
        public void Forward_chaining()
        {
            Console.WriteLine("Forward chaining");
            var x = this.pos_agent.Item1;
            var y = this.pos_agent.Item2;
            var currentCase = this.beliefs[x, y];
            Console.WriteLine("Case actuelle : "+x+" "+y);
            if (currentCase.Get_Smell())
            {
                Console.WriteLine("There is a monster nearby");
                Update_all_unknown_adjacent_cases(this.pos_agent, this.beliefs, "monster", 1f);
            }
            if (currentCase.Get_Wind())
            {
                Console.WriteLine("There is a cliff nearby");
                Update_all_unknown_adjacent_cases(this.pos_agent, this.beliefs, "cliff", 1f);
            }
            if (currentCase.Get_Light())
            {
                Console.WriteLine("There is a portal nearby");
                Update_all_unknown_adjacent_cases(this.pos_agent, this.beliefs, "portal", 1f);
                Update_all_unknown_adjacent_cases(this.pos_agent, this.beliefs, "monster", 0f);
                Update_all_unknown_adjacent_cases(this.pos_agent, this.beliefs, "cliff", 0f);
            }
        }

        public Tuple<int,int> Move_agent()
        {
            Tuple<int,int> next_pos_agent = new Tuple<int, int> (0,0);
            float safest = 1.0f;
            foreach (Case box in beliefs)
            {
                
                if (box.Get_border() == true)
                {
                    if (box.Get_Monster() < safest)
                    {
                        safest = box.Get_Monster();
                        next_pos_agent = CoordinatesOf(beliefs, box);
                        
                    }
                    else if (box.Get_Cliff() < safest)
                    {
                        safest = box.Get_Cliff();
                        next_pos_agent = CoordinatesOf(beliefs, box);
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
                    Case candidate = beliefs[x + dx, y + dy];
                    if ((dx != 0 && dy == 0) || (dx == 0 && dy != 0) && !candidate.Get_Visited())
                    {
                        candidate.Set_border(true);
                    }
                }
            }
            return next_pos_agent;
        }


        // Fonction pour lire le JSON

        public void Load_Json()
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
            }
        }
    }
}