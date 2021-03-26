using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace chourbland
{

    public class Agent
    {

        // Percepts - liste des capteurs de l'agent
        public List<String> percepts = new List<String>(new string[] { "smell", "wind", "shine" });

        // Intentions - Liste des actions disponibles pour l'agent
        public List<String> intentions = new List<String>(new string[] { "up", "down", "left", "right", "shoot" });

        // Desire - Trouver le portail
        public Object portal_searched = new Object();

        // Podition de l'agent
        public Tuple<int, int> pos_agent = new Tuple<int, int>(0, 0);

        public int performance_indicator = 0;

        public void Shoot_rock(Tuple<int, int> target_pos)
        {
            /*            if element on the cell == "monster"
                            kill_monster(target_pos);
            */
        }

        public Tuple<int, int> Move_agent()
        {
            foreach case in cases:
            {
                float safest = 1.0f;
                if (Case.border == true)
                {
                    if (Case.monster < safest)
                    {
                        safest = Case.monster;
                        //next_pos_agent = pos_case;
                        if (Case.cliff < safest)
                        {
                            safest = Case.cliff;
                            //next_pos_agent = pos_case;
                        }
                    }
                } //else random between borders                
            }
            return next_pos_agent;
        }
    }
}