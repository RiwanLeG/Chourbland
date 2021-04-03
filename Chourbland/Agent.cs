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
        public Case[,] beliefs = new Case[,] { };

        // Position de l'agent
        public Tuple<int, int> pos_agent = new Tuple<int, int>(0, 0);

        public int performance_indicator = 0;

        public Agent(int length, int width, Case initialposition_case, Tuple<int, int> initialPos, int performance)
        {
            pos_agent = initialPos;
            beliefs = new Case[length, width];
            performance_indicator = performance;
            for (int column = 0; column < width; column++)
            {
                for (int row = 0; row < length; row++)
                {
                    Case new_case = new Case();
                    beliefs[column, row] = new_case;
                }
            }

            beliefs[initialPos.Item1, initialPos.Item2] = initialposition_case;
            beliefs[initialPos.Item1, initialPos.Item2].Set_visited(true);
        }

        public Agent() { }

        public void Set_performance_indicator(int value)
        {
            performance_indicator += value;
            Console.WriteLine("performance_indicator in agent.cs : " + performance_indicator);
        }

        public void Shoot_rock(Tuple<int, int> target_pos)
        {
            // Récompense négative
            Set_performance_indicator(-10);

            //On supprime toute probabilité de monstre sur la case visée puisqu'ils ont été tués
            beliefs[target_pos.Item1,target_pos.Item2].Set_Monster(0f);
        }

        public Tuple<int, int> Consider_shooting_rock()
        {
            float monsterProb = 0f;
            Tuple<int, int> target_pos = new Tuple<int, int>(-1,-1);
            foreach (var box in beliefs)
            {
                if (box.Get_border())
                {
                    //si il existe une case frontière sans niveau de danger, on peut déjà quitter cette fonction
                    if (box.Get_Monster() == 0f && box.Get_Cliff() == 0f)
                    {
                        return target_pos;
                    }

                    //On choisit la case avec la plus haute probabilité d'avoir un monstre
                    if (box.Get_Monster() > monsterProb)
                    {
                        monsterProb = box.Get_Monster();
                        target_pos = CoordinatesOf(beliefs, box);
                    }
                }
            }
            Tuple<int, int> no_target = new Tuple<int, int>(-1, -1);
            // Si on a une cible, on tire
            if (!target_pos.Equals(no_target))
            {
                Shoot_rock(target_pos);
            }
            // On renvoie les coordonnées de la cible, pour supprimer les monstres présent sur la vraie grille
            return target_pos;
        }

        // On position l'agent
        public void Set_agent_position(Case a_case, Tuple<int, int> agent_position)
        {
            // On lui transmets les informations de la case surlaquelle il se trouve
            beliefs[agent_position.Item1, agent_position.Item2] = a_case;

            // Mis à jour de la position de l'agent
            pos_agent = agent_position;

            // La case a été visitée et ne fait plus parti de la frontière
            beliefs[agent_position.Item1, agent_position.Item2].Set_visited(true);
            beliefs[agent_position.Item1, agent_position.Item2].Set_border(false);

        }

        // Met à jour toutes les cases à côté de l'agent en fonction de sa case
        public void Update_all_unknown_adjacent_cases(Tuple<int, int> currentCasePos, KeyValuePair<string, JToken> rule, float value)
        {
            string danger = rule.Value["danger"].ToString();
            string goal = rule.Value["goal"].ToString();

            int x = currentCasePos.Item1;
            int y = currentCasePos.Item2;

            for (int dx = -1; dx <= 1; ++dx)
            {
                for (int dy = -1; dy <= 1; ++dy)
                {
                    int xdx = x + dx;
                    int ydy = y + dy;
                    //On vérifie bien qu'on ne sort pas de la grille
                    if ((xdx < 0) || (xdx > beliefs.GetLength(0) - 1) || (ydy < 0) ||
                        (ydy > beliefs.GetLength(1) - 1))
                    {
                        continue;
                    }
                    Case candidate = beliefs[xdx, ydy];
                    if (((dx != 0 && dy == 0) || (dx == 0 && dy != 0)) && !candidate.Get_Visited())
                    {
                        candidate.Set_border(true);
                        if (danger == "monster")
                        {
                            candidate.Add_Monster(candidate);
                        }
                        if (danger == "cliff")
                        {
                            candidate.Add_Cliff(candidate);
                        }
                        if (danger == "portal")
                        {
                            candidate.Set_Portal(value);
                        }
                        if (danger == "none")
                        {
                            candidate.Set_Cliff(0f);
                            candidate.Set_Monster(0f);
                        }
                        if(goal == "none")
                        {
                            candidate.Set_Portal(0f);
                        }
                    }
                }
            }
        }

        //CoordinatesOf renvoie les coordonnées d'une case dans une grille
        public static Tuple<int, int> CoordinatesOf(Case[,] grid, Case box)
        {
            int size = grid.GetLength(0);

            for (int x = 0; x < size; ++x)
            {
                for (int y = 0; y < size; ++y)
                {
                    if (grid[x, y].Equals(box))
                    return Tuple.Create(x, y);
                }
            }
            return Tuple.Create(-1, -1);
        }

        public bool Check_possibilities()
        {
            bool is_possibilities = false;

            foreach (Case box in beliefs)
            {
                if(box.Get_border())
                {
                    if(box.Get_Cliff() == 0f)
                    {
                        is_possibilities = true;
                    }
                }
            }
            Console.WriteLine("is_possibilities" + is_possibilities);
            return is_possibilities;
        }

        public Tuple<int, int> Move_agent()
        {
            // Récompense négative
            Set_performance_indicator(-1);

            if (!Check_possibilities())
            {
                foreach (var candidate in beliefs)
                {
                    if (candidate.Get_border())
                    {
                        Console.WriteLine("Apply_probbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb");
                        Apply_proba(candidate);
                    }
                }
            }

            Tuple<int, int> next_pos_agent = new Tuple<int, int>(0, 0);
            float safest = 1.0f;
            foreach (Case box in beliefs)
            {
                //On ne considère que les cases frontières car l'agent n'a aucun intérêt à retourner dans une case déjà visitée
                if (box.Get_border())
                {
                    if ((box.Get_Monster() + box.Get_Cliff()) < safest)
                    {
                        safest = (box.Get_Monster() + box.Get_Cliff());
                        next_pos_agent = CoordinatesOf(beliefs, box);

                    }
                }
            }
            return next_pos_agent;
        }

        // Fonction pour lire le JSON
        public JObject Load_Json()
        {

            string project_location = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            string path = project_location + @"\..\..\Rules.json";

            // read JSON directly from a file
            using (StreamReader file = File.OpenText(path))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject rules = (JObject)JToken.ReadFrom(reader);

                return rules;
            }
        }

        private bool Is_the_rule_applicable(KeyValuePair<string,JToken> a_rule, Case a_case)
        {
            bool is_rule_applicable = false;
            if ((a_rule.Key == "smell") && a_case.Get_Smell())
            {
                is_rule_applicable = true;
            }
            if ((a_rule.Key == "wind") && a_case.Get_Wind())
            {
                is_rule_applicable = true;
            }
            if ((a_rule.Key == "shine") && a_case.Get_Light())
            {
                is_rule_applicable = true;
            }
            if ((a_rule.Key == "nothing") && (!a_case.Get_Smell() && !a_case.Get_Wind() && !a_case.Get_Light()))
            {
                is_rule_applicable = true;
            }
            return is_rule_applicable;
        }

        // Chainage avant
        public void Forward_chaining_new_version()
        {
            // Récupération de la position actuelle de l'agent ainsi que de sa case
            var x = pos_agent.Item1;
            var y = pos_agent.Item2;
            var currentCase = beliefs[x, y];

            // Récupération des règles du fichier Json
            JObject rules = Load_Json();

            // Création d'une queue à laquelle nous allons ajouter toutes les règles du Json
            Queue<KeyValuePair<string, JToken>> queue_rules = new Queue<KeyValuePair<string, JToken>>();
            foreach (var rule in rules)
            {
                queue_rules.Enqueue(rule);
            }

            // On parcourt toutes les règles de la queue et on supprime celle marquée
            while (queue_rules.Count != 0)
            {
                // On Choisit une règle
                KeyValuePair<string, JToken> a_rule = queue_rules.Dequeue();

                // Si la règle n'est pas appliccable on passe à l'itération suivante
                if(!Is_the_rule_applicable(a_rule, currentCase))
                {
                    continue;
                }

                // On applique la règle choisie
                Update_all_unknown_adjacent_cases(pos_agent, a_rule, 0.25f);

            }
        }

        public void Apply_proba(Case the_case)
        {
            // Liste des cases vent
            List<Tuple<int, int>> wind_cases = new List<Tuple<int, int>>();

            // Liste des case frontières
            List<Tuple<int, int>> border_cases = new List<Tuple<int, int>>();
            for (int x = 0; x < beliefs.GetLength(0); x++)
            {
                for (int y = 0; y < beliefs.GetLength(0); y++)
                {
                    if (beliefs[x, y].Get_Wind())
                    {
                        wind_cases.Add(Tuple.Create(x, y));
                    }
                    else if (beliefs[x, y].Get_border())
                    {
                        border_cases.Add(Tuple.Create(x, y));
                    }
                }
            }

            // Liste de scénarios
            /*List<List<Tuple<bool, Tuple<int, int>>>> scenarios = new List<List<Tuple<bool, Tuple<int, int>>>>();*/

            int[] an_array = new int[border_cases.Count()];

            // Fonction qui créée tous les scénarios possibles
            an_array = generateAllBinaryStrings(border_cases.Count(), an_array, 0);

            int i = 0;

            List<List<bool>> several_scenarios = new List<List<bool>>();
            List<bool> a_scenario = new List<bool>();

            foreach (int el in an_array)
            {
                if (i < border_cases.Count)
                {
                    i++;
                    a_scenario.Add(Convert_int_to_bool(el));
                    continue;
                }

                several_scenarios.Add(a_scenario);
                a_scenario = new List<bool>();
                i = 0;
            }

            foreach (var scenario in several_scenarios)
            {
                foreach (var el in scenario)
                {
                    Console.Write(el + " ");
                }
                Console.WriteLine(" ");
            }


            foreach (Tuple<int, int> a_case in wind_cases)
            {

                for (int a = 0; a < several_scenarios.Count; a++)
                {
                    bool is_there_cliff_near = false;
                    for (int b = 0; b < several_scenarios[a].Count; b++)
                    {
                        int x_case = a_case.Item1;
                        int y_case = a_case.Item2;
                        if (y_case > 0)
                        {
                            Tuple<int, int> case_up = Tuple.Create(x_case, y_case - 1);
                            if (case_up.Equals(border_cases[b]) && (several_scenarios[a][b] == true))
                            {
                                is_there_cliff_near = true;
                            }
                        }

                        if (y_case < beliefs.GetLength(1))
                        {
                            Tuple<int, int> case_down = Tuple.Create(x_case, y_case + 1);
                            if (case_down.Equals(border_cases[b]) && (several_scenarios[a][b] == true))
                            {
                                is_there_cliff_near = true;
                            }
                        }

                        if (x_case < beliefs.GetLength(0))
                        {
                            Tuple<int, int> case_right = Tuple.Create(x_case + 1, y_case);
                            if (case_right.Equals(border_cases[b]) && (several_scenarios[a][b] == true))
                            {
                                is_there_cliff_near = true;
                            }
                        }

                        if (x_case > 0)
                        {
                            Tuple<int, int> case_left = Tuple.Create(x_case - 1, y_case);
                            if (case_left.Equals(border_cases[b]) && (several_scenarios[a][b] == true))
                            {
                                is_there_cliff_near = true;
                            }
                        }


                    }

                    if (!is_there_cliff_near)
                    {
                        several_scenarios.RemoveAt(a);
                    }
                }
            }

            // Cacul

            Tuple<int, int> case_coordinate = CoordinatesOf(beliefs, the_case);

            int index_case = 0;

            for (int c = 0; c < border_cases.Count; c++)
            {
                if (case_coordinate.Equals(border_cases[c]))
                {
                    index_case = c;
                }
            }

            // Calcul
            float probability_cliff_not_normalized = 0f;
            float probability_not_cliff_not_normalized = 0f;



            foreach (var scenario in several_scenarios)
            {
                if (scenario[index_case] == true)
                {
                    Console.WriteLine("Test de passage");
                    int number_of_cliff_up = 0;
                    int number_of_cliff_down = 0;
                    for (int d = 0; d < scenario.Count; d++)
                    {
                        if (scenario[d] == true)
                        {
                            number_of_cliff_up++;
                        }
                        else
                        {
                            number_of_cliff_down++;
                        }
                    }
                    probability_cliff_not_normalized += (float)(Math.Pow(0.2, number_of_cliff_up) * Math.Pow(0.8, number_of_cliff_up));
                }
                else
                {
                    int number_of_cliff_up = 0;
                    int number_of_cliff_down = 0;
                    for (int d = 0; d < scenario.Count; d++)
                    {
                        if (scenario[d] == true)
                        {
                            number_of_cliff_up++;
                        }
                        else
                        {
                            number_of_cliff_down++;
                        }
                    }
                    probability_not_cliff_not_normalized += (float)(Math.Pow(0.2, number_of_cliff_up) * Math.Pow(0.8, number_of_cliff_up));
                }
            }
            float alpha = 1 / (probability_not_cliff_not_normalized + probability_cliff_not_normalized);
            float probability_cliff_normalized = alpha * probability_cliff_not_normalized;
            float probability_not_cliff_normalized = alpha * probability_not_cliff_not_normalized;

            Console.WriteLine("probability_cliff_normalized : " + probability_cliff_normalized );
            // Application de la probabilité
            the_case.Set_Cliff(probability_cliff_normalized);
        }

        private bool Convert_int_to_bool(int a_value)
        {
            return a_value.Equals(1);
        }


        // Fonction permettant de générer des scénarios binaire en fonction du nombre de frontière passé en paramètre
        public int[] generateAllBinaryStrings(int n,
                                    int[] arr, int i)
        {
            if (i == n)
            {
                return arr;
            }

            arr[i] = 0;
            generateAllBinaryStrings(n, arr, i + 1);

            arr[i] = 1;
            generateAllBinaryStrings(n, arr, i + 1);

            return arr;
        }
    }
}