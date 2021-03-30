﻿using System;
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

        private int performance_indicator = 0;

        public Agent(int length, int width, Case initialposition_case, Tuple<int, int> initialPos)
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
            beliefs[initialPos.Item1, initialPos.Item2].Set_visited(true);
        }

        public Agent() { }

        public void Set_performance_indicator(int value)
        {
            performance_indicator += value;
            Console.WriteLine("performance_indicator : " + performance_indicator);
        }

        public void Shoot_rock(Tuple<int, int> target_pos)
        {
            // Récompense négative
            Set_performance_indicator(-10);

            /*            if element on the cell == "monster"
                            kill_monster(target_pos);
            */
        }

        public void Consider_shooting_rock()
        {
            float monsterProb = 0f;
            Tuple<int, int> target_pos = new Tuple<int, int>(-1,-1);
            foreach (var box in beliefs)
            {
                if (box.Get_border())
                {
                    //si il existe une case frontière sans niveau de danger, on peut déjà quitter cette fonction
                    if (box.Get_Monster() == 0f)
                    {
                        return;
                    }
                    else
                    {
                        //On choisit la case avec la plus haute probabilité d'avoir un monstre
                        if (box.Get_Monster() > monsterProb)
                        {
                            monsterProb = box.Get_Monster();
                            target_pos = CoordinatesOf(beliefs, box);
                        }
                    }
                }
                //Les cases non-comprises dans la frontière ne nous intéressent pas
                else
                {
                    continue;
                }
            }
            Console.WriteLine("L'agent va tirer en :("+target_pos.Item1+","+target_pos.Item2+")");
            Shoot_rock(target_pos);
            return;
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
        /*KeyValuePair<string, JToken>*/
        /*public void Update_all_unknown_adjacent_cases(Tuple<int, int> currentCasePos, string field, float value)*/
        public void Update_all_unknown_adjacent_cases(Tuple<int, int> currentCasePos, KeyValuePair<string, JToken> rule, float value)
        {
            string danger = rule.Value["danger"].ToString();
            string goal = rule.Value["goal"].ToString();

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
                    if ((xdx < 0) || (xdx > beliefs.GetLength(0) - 1) || (ydy < 0) ||
                        (ydy > beliefs.GetLength(1) - 1))
                    {
                        continue;
                    }
                    Case candidate = beliefs[xdx, ydy];
                    if (((dx != 0 && dy == 0) || (dx == 0 && dy != 0)) && !candidate.Get_Visited())
                    {
                        candidate.Set_border(true);
                        number_candidate++;
                        //Console.WriteLine("Case(" + xdx + "," + ydy + "): monster:" + candidate.Get_Monster() + "; cliff:" + candidate.Get_Cliff() + "; portal:" + candidate.Get_Portal());
                        if (danger == "monster")
                        {
                            Console.WriteLine("Attention monstre !");
                            candidate.Add_Monster(value);
                        }
                        if (danger == "cliff")
                        {
                            Console.WriteLine("Attention cliff !");
                            candidate.Add_Cliff(value);
                        }
                        if (danger == "portal")
                        {
                            Console.WriteLine("Attention portal !");
                            candidate.Set_Portal(value);
                        }
                        if (danger == "none")
                        {
                            Console.WriteLine("Pas de danger !");
                            candidate.Set_Cliff(0f);
                            candidate.Set_Monster(0f);
                        }
                        if(goal == "none")
                        {
                            Console.WriteLine("Pas de portail !");
                            candidate.Set_Portal(0f);
                        }
                        if(goal == "portal")
                        {
                            Console.WriteLine("Portail en vu !");
                            /*candidate.Set_Portal(0f);*/
                            candidate.Substract_cliff(-0.25f);
                        }
                    }
                }
            }
            Console.WriteLine("number_candidate : " + number_candidate);
        }
        public static Tuple<int, int> CoordinatesOf(Case[,] grid, Case box)
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

        public Tuple<int, int> Move_agent()
        {
            Console.WriteLine("Move");
            // Récompense négative
            Set_performance_indicator(-1);

            Tuple<int, int> next_pos_agent = new Tuple<int, int>(0, 0);
            float safest = 1.0f;
            int number_iteration = 0;
            foreach (Case box in beliefs)
            {
                
                if (box.Get_border()/* || box.Get_Visited()*/)
                {
                    Console.WriteLine("POS" + CoordinatesOf(beliefs, box) + " danger="+ (box.Get_Monster() + box.Get_Cliff()) + "\nMonster ?:" + box.Get_Smell() + box.Get_Monster() + "Cliff ?:" + box.Get_Wind() + box.Get_Cliff());
                }
                //if ((box.Get_border() == true)&&(box.Get_Visited() == false))
                /*Console.WriteLine("box.Get_border() : " + box.Get_border());*/
                /*Console.WriteLine("box.Get_Visited() : " + box.Get_Visited());*/
                if (box.Get_border())
                {
                    number_iteration++;
                    //next_pos_agent = CoordinatesOf(beliefs, box);
                    Console.WriteLine("box.Get_Visited() : " + next_pos_agent);
                    if ((box.Get_Monster() + box.Get_Cliff()) < safest)
                    {
                        safest = (box.Get_Monster() + box.Get_Cliff());
                        next_pos_agent = CoordinatesOf(beliefs, box);
                        Console.WriteLine("Position monstre ! " + safest);

                    }
                    /*if (box.Get_Cliff() < safest)
                    {
                        safest = box.Get_Cliff();
                        next_pos_agent = CoordinatesOf(beliefs, box);
                        Console.WriteLine("Position falaise ! ");
                    }*/
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
/*                    if ((dx != 0 && dy == 0) || (dx == 0 && dy != 0) && !candidate.Get_Visited())
                    {
                        candidate.Set_border(true);
                    }*/
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
            if ((a_rule.Key == "smell") && (a_case.Get_Smell() == true))
            {
                is_rule_applicable = true;
            }
            if ((a_rule.Key == "wind") && (a_case.Get_Wind() == true))
            {
                is_rule_applicable = true;
            }
            if ((a_rule.Key == "shine") && (a_case.Get_Light() == true))
            {
                is_rule_applicable = true;
            }
            if ((a_rule.Key == "nothing") && (!a_case.Get_Smell() && !a_case.Get_Wind() && !a_case.Get_Light()))
            {
                is_rule_applicable = true;
            }
            Console.WriteLine("La règle est applicable : " + is_rule_applicable);
            return is_rule_applicable;
        }

        // Choisi la première règle applicable du dictionnaire
        public KeyValuePair<string, JToken> Get_a_chosen_rule(Dictionary<KeyValuePair<string, JToken>, bool> rules)
        {
            KeyValuePair<string, JToken> rule_chosen = new KeyValuePair<string, JToken>();
            foreach (var rule in rules)
            {
                if (rule.Value == true)
                {
                    rule_chosen = rule.Key;
                    break;
                }
            }
            return rule_chosen;
        }


        // Chainage avant
        public void Forward_chaining_new_version()
        {
            // Récupération de la position actuelle de l'agent ainsi que de sa case
            var x = pos_agent.Item1;
            var y = pos_agent.Item2;
            var currentCase = beliefs[x, y];
            Console.WriteLine("Case actuelle : " + x + " " + y);


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
                Console.WriteLine("Nombre de règle : " + queue_rules.Count());

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

    }
}