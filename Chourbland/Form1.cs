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
        // Nombre de ligne de la grille
        int line_number = 4;

        // Elément de la grille
        List<Element> elements = new List<Element>();

        // Random
        Random random = new Random();

        public Form1()
        {
            InitializeComponent();
        }

        public void Create_Full_Grid()
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

            // Génération aléatoirement du portail
            Tuple<int,int> portal_position = Generate_Portal();

            // Pour chaque case : on génère ou non un élément
            for (int k = 0; k < line_number - 1; k++)
            {
                for (int n = 0; n < line_number - 1; n++)
                {
                    if((portal_position.Item1 != k) || (portal_position.Item2 != n)) { 
                        Console.WriteLine("Generate_Monster_Or_Cleaf k et n : " + k + " " + n);
                        Generate_Monster_Or_Cleaf(k,n);
                    }
                }
            }

            // On dessine les éléments dans la grille
            foreach (Element element in elements)
            {
                graphic.DrawString(element.Get_type()[0].ToString(), font, Brushes.Black, element.position.Item1 * size, element.position.Item2 * size);

            }

            line_number++;
        }


        // Génération d'objets
        private void Generate_Monster_Or_Cleaf(int new_x, int new_y)
        {

            int index_random_object_type = random.Next(100);

            String type_object = "";


            // 20% de chance -> colline
            if (index_random_object_type <= 20)
            {
                type_object = "cleaf";
            }
            // 20 % de chance->monster
            else if ((index_random_object_type >= 20) && (index_random_object_type <= 40))
            {
                type_object = "monster";
            }
            // Pas d'objet à affecter à la case
            else
            {
                return;
            }

            Element new_element = new Element(Tuple.Create(new_x, new_y), type_object);
            elements.Add(new_element);
        }

        public Tuple<int,int> Generate_Portal()
        {
            Element new_element = new Element(Tuple.Create(random.Next(line_number-1), random.Next(line_number-1)), "portal");
            elements.Add(new_element);
            return new_element.position;
        }




        private void button1_Click(object sender, EventArgs e)
        {

            Create_Full_Grid();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Nombre d'élément : " + elements.Count);
        }
    }
}
