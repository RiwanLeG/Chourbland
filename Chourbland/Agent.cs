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

        public void Shoot_rock(Tuple<int, int> target_pos)
        {
            /*            if element on the cell == "monster"
                            kill_monster(target_pos);
            */
        }

        public List<Case> Update_all_unknown_adjacent_cases(Tuple<int, int> current_case_pos, Case[,] current_grid, string field, float value)
        {
            List<Case> Unknown_adjacent_cases = new List<Case>();
            int x = current_case_pos.Item1;
            int y = current_case_pos.Item2;
            for (int dx = -1; dx <= 1; ++dx)
            {
                for (int dy = -1; dy <= 1; ++dy)
                {
                    Case candidate = current_grid[x + dx,y + dy];
                    if ((dx != 0 && dy == 0) || (dx == 0 && dy != 0) && !candidate.Get_Visited())
                    {
                        candidate.GetType().GetProperty(field).SetValue(candidate, value, null);
                    }
                }
            }
            return Unknown_adjacent_cases;
        }
        public static Tuple<int, int> CoordinatesOf( Case[,] grid, Case candidiate)
        {
            int w = grid.GetLength(0); // width
            int h = grid.GetLength(1); // height

            for (int x = 0; x < w; ++x)
            {
                for (int y = 0; y < h; ++y)
                {
                    if (grid[x, y].Equals(candidiate))
                        return Tuple.Create(x, y);
                }
            }

            return Tuple.Create(-1, -1);
        }
        public void Forward_chaining(Case[,] currentBeliefs)
        {
            foreach (Case candidate in currentBeliefs)
            {
                //var neighbors = Get_all_unknown_adjacent_cases(candidate, currentBeliefs)
                Tuple<int, int> candidate_pos = CoordinatesOf(currentBeliefs, candidate);
                //Si une case sent mauvaise,alors il y a peut-être un monstre dans les cases adjacentes non-visitées
                if (candidate.Get_Smell())
                {
                    Update_all_unknown_adjacent_cases(candidate_pos, currentBeliefs, "monster", 1f);
                }
                //Si une case sent mauvaise,alors il y a peut-être une falaise dans les cases adjacentes non-visitées
                if (candidate.Get_Wind())
                {
                    Update_all_unknown_adjacent_cases(candidate_pos, currentBeliefs, "cliff", 1f);
                }
                //Si une case sent mauvaise,alors il y a le portail dans l'une des cases adjacentes non-visitées
                if (candidate.Get_Light())
                {
                    Update_all_unknown_adjacent_cases(candidate_pos, currentBeliefs, "portal", 1f);
                    Update_all_unknown_adjacent_cases(candidate_pos, currentBeliefs, "monster", 0f);
                    Update_all_unknown_adjacent_cases(candidate_pos, currentBeliefs, "cliff", 0f);
                }
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