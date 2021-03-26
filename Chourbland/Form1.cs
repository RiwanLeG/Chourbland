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
        String[,] cases = new string[5, 5];
        /*List<float>[,] cases = new List<float>[5, 5];*/
        // Nombre de ligne de la grille
        int line_number = 0;

        // Elément de la grille
        List<Element> elements = new List<Element>();

        // Random
        Random random = new Random();

        public Form1()
        {
            InitializeComponent();
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
            float size = (float)(100/line_number);

            // lignes verticales
            /*for (int i = 0; i < line_number; i++)*/
            for (int i = 0; i < line_number; i++)
            {
                graphic.DrawLine(pen, x, 0, x, line_number * size - size);
                x += size;
            }

            // lignes horizontales
            for (int i = 0; i < line_number; i++)
            {
                graphic.DrawLine(pen, 0, y, line_number * size - size, y);
                y += size;
            }



            // Pour chaque case : on génère ou non un élément
            for (int k = 0; k < cases.GetLength(0)-1; k++)
            {
                for (int n = 0; n < cases.GetLength(1) - 1; n++)
                {
                    graphic.DrawString(cases[k, n][0].ToString(), font, Brushes.Black, k * size, n * size);
                }
            }

        }

        public void Create_Grid()
        {
            line_number = cases.GetLength(0);
            // Génération aléatoirement du portail
            Tuple<int,int> portal_position = Generate_Portal();

            Console.WriteLine("portal_position : " + portal_position);

            // Pour chaque case : on génère ou non un élément
            for (int k = 0; k < line_number - 1; k++)
            {
                for (int n = 0; n < line_number - 1; n++)
                {
                    if((portal_position.Item1 != k) && (portal_position.Item2 != n)) {
                        Console.WriteLine("Un Monstre " + k + " " + n);
                        Generate_Monster_Or_Cleaf(k,n);
                    }
                }
            }

            Draw_Grid();
        }



        // Génération d'éléments pour chacune des cases
        private void Generate_Monster_Or_Cleaf(int new_x, int new_y)
        {

            int index_random_object_type = random.Next(100);

            String type_object = " ";


            // 20% de chance -> colline
            if (index_random_object_type <= 20)
            {
                type_object = "cleaf";
                type_object = "monster";
                if (new_x > 0)
                    cases[new_x - 1, new_y] = "wind";
                if (new_y > 0)
                    cases[new_x, new_y - 1] = "wind";
                if (new_y < cases.GetLength(1))
                    cases[new_x + 1, new_y] = "wind";
                if (new_y < cases.GetLength(0))
                    cases[new_x, new_y + 1] = "wind";
            }
            // 20 % de chance->monster
            else if ((index_random_object_type >= 20) && (index_random_object_type <= 40))
            {
                type_object = "monster";
                if(new_x > 0)
                    cases[new_x - 1, new_y] = "smell";
                if(new_y > 0)
                    cases[new_x, new_y-1] = "smell";
                if (new_y < cases.GetLength(1))
                    cases[new_x + 1, new_y] = "smell";
                if (new_y < cases.GetLength(0))
                    cases[new_x, new_y+1] = "smell";
            }
            // Pas d'objet à affecter à la case
            cases[new_x, new_y] = type_object;
/*            Element new_element = new Element(Tuple.Create(new_x, new_y), type_object);
            elements.Add(new_element);*/
        }

        public Tuple<int,int> Generate_Portal()
        {
            int portal_x = random.Next(line_number - 1);
            int portal_y = random.Next(line_number - 1);
            cases[portal_x, portal_y] = "portal";
            return Tuple.Create(portal_x,portal_y);
        }

        public void Display_Grid()
        {

            // Pour chaque case : on génère ou non un élément
            for (int k = 0; k < cases.GetLength(0); k++)
            {
                for (int n = 0; n < cases.GetLength(1); n++)
                {
                    Console.Write(cases[k, n] + " ");
                }
                Console.WriteLine("");
            }

        }


        private void button1_Click(object sender, EventArgs e)
        {
            // On passe en paramètre le tableau et on le dessine
            Console.WriteLine("before");
            Display_Grid();

            Create_Grid();

            Console.WriteLine("after");
            Display_Grid();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Nombre d'élément : " + elements.Count);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
