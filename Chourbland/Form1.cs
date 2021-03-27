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
        // Tableau de toutes les entrées
        /*String[,] cases = new string[5, 5];*/

        int grid_size = 5;

        Case[,] cases = new Case[5, 5];
        // Nombre de ligne de la grille
        int line_number = 0;
        
        // Elément de la grille
        List<Element> elements = new List<Element>();

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

            // Clear la liste d'éléments
            elements = new List<Element>();

            // Création de la grille visuelle
            Graphics graphic = grid.CreateGraphics();
            Pen effective_pen = new Pen(Brushes.Black, 1);
            Pen pen = new Pen(Brushes.Black, 1);
            Pen grass_pen = new Pen(Brushes.Black, 3);
            Font font = new Font("Arial", 10);


            
            float x = 0f;
            float y = 0f;

            // Taille des case
            float size = (float)(100/cases.GetLength(0));

            // lignes verticales
            /*for (int i = 0; i < line_number; i++)*/
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

        public void Create_Grid(int a_grid_size)
        {
            /*line_number = cases.GetLength(0);*/
            line_number = a_grid_size;


            cases = new Case[line_number, line_number];
            Initialize_Tab_Case();
            // Génération aléatoirement du portail



            // Initialisation du tableau
            Initialize_Tab_Case();

            Tuple<int,int> portal_position = Generate_Portal();

            /*Console.WriteLine("portal_position : " + portal_position);*/

            // Pour chaque case : on génère ou non un élément
            for (int k = 0; k < cases.GetLength(0); k++)
            {
                for (int n = 0; n < cases.GetLength(1); n++)
                {
                    if((portal_position.Item1 != k) && (portal_position.Item2 != n)) {
                        /*Console.WriteLine("Un Monstre " + k + " " + n);*/
                        Generate_Monster_Or_Cliff(k,n);
                    }
                }
            }
            Draw_Grid();
        }


        // Génération d'éléments pour chacune des cases
        private void Generate_Monster_Or_Cliff(int new_x, int new_y)
        {

            int index_random_object_type = random.Next(100);

            String type_object = " ";

            /*Console.WriteLine("Position : " + new_x + " " + new_y);*/

            // 20% de chance -> colline
            if (index_random_object_type <= 20)
            {
                type_object = "cliff";
                cases[new_x, new_y].Set_Cliff(1.0f);
                if (new_x > 0)
                    cases[new_x - 1, new_y].Set_Wind(true);
                if (new_y > 0)
                    cases[new_x, new_y - 1].Set_Wind(true);
                if (new_x < cases.GetLength(0)-1)
                    cases[new_x + 1, new_y].Set_Wind(true);
                if (new_y < cases.GetLength(1)-1)
                    cases[new_x, new_y + 1].Set_Wind(true);
            }
            // 20 % de chance->monster
            else if ((index_random_object_type >= 20) && (index_random_object_type <= 40))
            {
                type_object = "monster";
                cases[new_x, new_y].Set_Monster(1.0f);
                if (new_x > 0)
                    cases[new_x - 1, new_y].Set_Smell(true);
                if(new_y > 0)
                    cases[new_x, new_y-1].Set_Smell(true);
                if (new_x < cases.GetLength(1)-1)
                    cases[new_x + 1, new_y].Set_Smell(true);
                if (new_y < cases.GetLength(0)-1)
                    cases[new_x, new_y+1].Set_Smell(true);
            }
        }

        public Tuple<int,int> Generate_Portal()
        {
            int portal_x = random.Next(line_number - 1);
            int portal_y = random.Next(line_number - 1);
            cases[portal_x, portal_y].Set_Portal(1.0f);

            if (portal_x > 0)
                cases[portal_x - 1, portal_y].Set_Light(true);
            if (portal_y > 0)
                cases[portal_x, portal_y - 1].Set_Light(true);
            if (portal_x < cases.GetLength(1))
                cases[portal_x + 1, portal_y].Set_Light(true);
            if (portal_y < cases.GetLength(0))
                cases[portal_x, portal_y + 1].Set_Light(true);

            return Tuple.Create(portal_x,portal_y);
        }

        private void Display_Grid(Case[,] a_cases)
        {

            // Pour chaque case : on génère ou non un élément
            for (int k = 0; k < a_cases.GetLength(0); k++)
            {
                for (int n = 0; n < a_cases.GetLength(1); n++)
                {
                    Console.Write(a_cases[n, k].Get_Light() + " ");
                }
                Console.WriteLine("");
            }
        }

        private void Update_Agent_position(Tuple<int, int> old_agent_position, Tuple<int,int> new_agent_position)
        {
            // Suppression de l'ancienne position de l'agent
            cases[old_agent_position.Item1, old_agent_position.Item2].Set_Agent(false);

            // Mis à jour du déplacement de l'agent
            cases[new_agent_position.Item1, new_agent_position.Item2].Set_Agent(true);

            // Test si agent encore en vie
            if((cases[new_agent_position.Item1, new_agent_position.Item2].Get_Cliff() == 1.0f) || (cases[new_agent_position.Item1, new_agent_position.Item2].Get_Monster() == 1.0f))
            {
                // Fonction supprimer l'agent + regénération de la grille
                Console.WriteLine("DEFEAT !");
                Create_Grid(grid_size);
            }
            if ((cases[new_agent_position.Item1, new_agent_position.Item2]).Get_Portal() == 1.0f)
            {
                // Pop up victoire + regénération de la grile
                Console.WriteLine("VICTORY !");
                Create_Grid(grid_size);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            // On passe en paramètre le tableau et on le dessine
            /*Console.WriteLine("before");
            Display_Grid(cases);*/

            Create_Grid(grid_size);
            grid_size++;
            Console.WriteLine("GridSize : " + grid_size);
            Console.WriteLine("after");
            Display_Grid(cases);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Tuple<int, int> initPos = new Tuple<int, int>(3, 2); 

            //Console.WriteLine("cases " + cases[3,2].Image );
            Agent personnage = new Agent(cases.GetLength(0), cases.GetLength(1), cases[initPos.Item1, initPos.Item2], initPos);
            personnage.Forward_chaining();
        }

        private void grid_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}
