using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chourbland
{
    public partial class Form1 : Form
    {
        // Création de l'agent
        Agent the_agent = new Agent();

        // Taille du tableau
        int grid_size = 3;

        Case[,] cases = new Case[3, 3];

        // Nombre de ligne de la grille
        int line_number = 0;

        // Position actuelle de l'agent
        Tuple<int, int> current_agent_position = Tuple.Create(0, 0);

        // Random
        Random random = new Random();

        // Ancienne position de l'agent
        Tuple<int, int> old_agent_position = Tuple.Create(0,0);

        public Form1()
        {
            InitializeComponent();
        }

        public void Initialize_Tab_Case()
        {
            // Initialisation des cellules du tableau
            for (int column = 0; column < cases.GetLength(0); column++)
            {
                for (int row = 0; row < cases.GetLength(1); row++)
                {
                    Case new_case = new Case();
                    cases[column, row] = new_case;
                }
            }
        }

        public void Draw_Grid()
        {
            // Clear the grid
            grid.Refresh();

            // Création de la grille visuelle
            Graphics graphic = grid.CreateGraphics();
            Pen pen = new Pen(Brushes.Black, 1);
            Font font = new Font("Arial", 10);

            float x = 0f;
            float y = 0f;

            // Taille des case
            float size = (250/cases.GetLength(0));

            // lignes verticales
            for (int i = 0; i < cases.GetLength(0) +1; i++)
            {
                graphic.DrawLine(pen, x, 0, x, cases.GetLength(0) * size);
                x += size;
            }

            // lignes horizontales
            for (int i = 0; i < cases.GetLength(1) + 1; i++)
            {
                graphic.DrawLine(pen, 0, y, cases.GetLength(1) * size, y);
                y += size;
            }



            // Pour chaque case : on génère ou non un élément
            for (int k = 0; k < cases.GetLength(0); k++)
            {
                for (int n = 0; n < cases.GetLength(1); n++)
                {
                    graphic.DrawString(cases[k, n].Image.ToString(), font, Brushes.Black, k * size, n * size);
                }
            }
        }

        // Créer une nouvelle grille : agent en position initial (0,0)
        public void Create_Grid(int a_grid_size)
        {
            line_number = a_grid_size;

            cases = new Case[line_number, line_number];
            Initialize_Tab_Case();

            // Initialisation du tableau

            // Génération aléatoirement du portail
            Tuple<int,int> portal_position = Generate_Portal();

            // Positionnement de l'agent sur la grille
            Agent_position_on_the_grid();
            
            // Pour chaque case : on génère ou non un élément
            for (int k = 0; k < cases.GetLength(0); k++)
            {
                for (int n = 0; n < cases.GetLength(1); n++)
                {
                    // pas de génération de monstre sur la position du portail et de l'agent
                    if (((portal_position.Item1 != k) && (portal_position.Item2 != n)) && ((current_agent_position.Item1 != k) && (current_agent_position.Item2 != n)))
                    {
                        Generate_Monster_Or_Cliff(k,n);
                    }
                }
            }
            Update_Smell_Wind_and_Light();
        }

        // Génération d'éléments pour chacune des cases
        private void Generate_Monster_Or_Cliff(int new_x, int new_y)
        {
            int index_random_object_type = random.Next(100);

            // 20% de chance -> colline
            if (index_random_object_type <= 20)
            {
                cases[new_x, new_y].Set_Cliff(1.0f);
            }
            // 20 % de chance->monster
            else if ((index_random_object_type >= 20) && (index_random_object_type <= 40))
            {
                cases[new_x, new_y].Set_Monster(1.0f);
            }
        }

        public void Update_Smell_Wind_and_Light()
        {
            foreach (Case box in cases)
            {
                // On réinitialise les odeurs, vents et lumière
                box.Set_Smell(false);
                box.Set_Wind(false);
                box.Set_Light(false);
            }

            foreach (Case box in cases)
            {
                Tuple<int, int> box_pos = CoordinatesOf(cases, box);
                if (box.Get_Monster() > 0f)
                {
                    for (int dx = -1; dx <= 1; ++dx)
                    {
                        for (int dy = -1; dy <= 1; ++dy)
                        {
                            int xdx = box_pos.Item1 + dx;
                            int ydy = box_pos.Item2 + dy;
                            //On vérifie bien qu'on ne sort pas de la grille
                            if (xdx < 0 || xdx > cases.GetLength(0) - 1 || ydy < 0 || ydy > cases.GetLength(1) - 1)
                            {
                                continue;
                            }

                            if (dx != 0 && dy == 0 || dx == 0 && dy != 0)
                            {
                                cases[xdx, ydy].Set_Smell(true);
                            }
                        }
                    }
                }

                if (box.Get_Cliff() > 0f)
                {
                    for (int dx = -1; dx <= 1; ++dx)
                    {
                        for (int dy = -1; dy <= 1; ++dy)
                        {
                            int xdx = box_pos.Item1 + dx;
                            int ydy = box_pos.Item2 + dy;
                            //On vérifie bien qu'on ne sort pas de la grille
                            if (xdx < 0 || xdx > cases.GetLength(0) - 1 || ydy < 0 || ydy > cases.GetLength(1) - 1)
                            {
                                continue;
                            }

                            if (((dx != 0 && dy == 0) || (dx == 0 && dy != 0)))
                            {
                                cases[xdx, ydy].Set_Wind(true);

                            }
                        }
                    }
                }
                if (box.Get_Portal()>0)
                {
                    for (int dx = -1; dx <= 1; ++dx)
                    {
                        for (int dy = -1; dy <= 1; ++dy)
                        {
                            int xdx = box_pos.Item1 + dx;
                            int ydy = box_pos.Item2 + dy;
                            //On vérifie bien qu'on ne sort pas de la grille
                            if (xdx < 0 || xdx > cases.GetLength(0) - 1 || ydy < 0 || ydy > cases.GetLength(1) - 1)
                            {
                                continue;
                            }

                            if (((dx != 0 && dy == 0) || (dx == 0 && dy != 0)))
                            {
                                cases[xdx, ydy].Set_Light(true);

                            }
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

        public Tuple<int,int> Generate_Portal()
        {
            // On génère aléatoirement les coordonnées du portail
            int portal_x = random.Next(line_number);
            int portal_y = random.Next(line_number);
            cases[portal_x, portal_y].Set_Portal(1.0f);
            
            return Tuple.Create(portal_x,portal_y);
        }

        // Restart la position de l'agent
        public void Agent_position_on_the_grid()
        {
            // Initialisation de l'agent à la case (0,0)
            the_agent = new Agent(cases.GetLength(0), cases.GetLength(1), cases[current_agent_position.Item1, current_agent_position.Item2], current_agent_position);

            current_agent_position = Tuple.Create(0, 0);
            cases[current_agent_position.Item1, current_agent_position.Item2].Set_Agent(true);
        }

        private void Update_Agent_position(Tuple<int,int> new_agent_position)
        {
            // Test de victoire ou de défaite de l'agent
            bool restard_grid = Victory_And_Defeat_Test(new_agent_position);

            // Suppression de l'ancienne position de l'agent
            cases[current_agent_position.Item1, current_agent_position.Item2].Set_Agent(false);
            if (restard_grid)
            {
                current_agent_position = Tuple.Create(0, 0);
                Create_Grid(grid_size);
            }
            else
            {
                current_agent_position = new_agent_position;
                // Mis à jour de l'ancienne position qui devient la position actuelle
                cases[new_agent_position.Item1, new_agent_position.Item2].Set_Agent(true);
            }

            // On translets à l'agent la case sur laquelle il se trouve ainsi que sa nouvelle position
            the_agent.Set_agent_position(cases[current_agent_position.Item1, current_agent_position.Item2], current_agent_position);

            Draw_Grid();
        }

        // Test de victoire et de défaite en fonction de la position passsée en paramètre
        private bool Victory_And_Defeat_Test(Tuple<int,int> agent_position)
        {
            bool restart_the_grid = false;
            // Test si agent encore en vie
            if (cases[agent_position.Item1, agent_position.Item2].Get_Cliff() == 1.0f || cases[agent_position.Item1, agent_position.Item2].Get_Monster() == 1.0f)
            {
                // Fonction supprimer l'agent + regénération de la grille
                restart_the_grid = true;

                // Récompense négative
                the_agent.Set_performance_indicator(-10 * grid_size * grid_size);

            }
            if (cases[agent_position.Item1, agent_position.Item2].Get_Portal() == 1.0f)
            {
                // Récompense positive
                the_agent.Set_performance_indicator(10* grid_size* grid_size);

                // On augmente la taille de la grille
                grid_size++;
                restart_the_grid = true;
            }
            return restart_the_grid;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            // Création de la 1ère grille
            Create_Grid(grid_size);

            // Test si l'agent spawn sur le portail
            Update_Agent_position(current_agent_position);

            // On dessine la grille
            Draw_Grid();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            the_agent.Forward_chaining_new_version();
            Tuple<int, int> smashedcase = the_agent.Consider_shooting_rock();
            Tuple<int, int> error_value = new Tuple<int, int>(-1, -1);
            if (!smashedcase.Equals(error_value))
            {
                cases[smashedcase.Item1, smashedcase.Item2].Set_Monster(0f);
                Update_Smell_Wind_and_Light();
            }

            // Nouvelle position de l'agent
            Tuple<int, int> new_agent_position = the_agent.Move_agent();

            // Mise à jour graphique et dans le tableau cases de la position de l'agent
            Update_Agent_position(new_agent_position);
        }
        
        // Test de la fonction de récupération du fichier Json
        private void button7_Click(object sender, EventArgs e)
        {
            Agent the_agent = new Agent(cases.GetLength(0), cases.GetLength(1), cases[current_agent_position.Item1, current_agent_position.Item2], current_agent_position);
            the_agent.Load_Json();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
