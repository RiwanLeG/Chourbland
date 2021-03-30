using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chourbland
{
    public class Case
    {
        private bool _agent = false;

        private float _monster = 0f;
        private bool _smell = false;

        private float _cliff = 0f;
        private bool _wind = false;

        private float _portal = 0f;
        private bool _light = false;

        private bool _visited = false;

        private bool _border = false;

        public string Image = " ";

        public Case()
        {

        }

        public void Set_Agent(bool is_agent)
        {
            _agent = is_agent;
            Update_Image();
        }
        public bool Get_Agent()
        {
            return _agent;
        }
        public void Set_Monster(float a_monster) {
            _monster = a_monster;
            Update_Image();
        }

        public void Add_Monster(float a_monster)
        {
            _monster = a_monster + 0.25f;
            Update_Image();
        }
        public float Get_Monster()
        {
            return _monster;
        }

        public void Set_Cliff(float a_cliff) {
            _cliff = a_cliff;
            Update_Image();
        }

        public void Add_Cliff(float a_cliff)
        {
            _cliff = a_cliff + 0.25f ;
            /*Update_Image();*/
        }


        public void Substract_cliff(float value)
        {
            _cliff += value;
            /*Update_Image();*/
        }


        public float Get_Cliff()
        {
            return _cliff;
        }
        public void Set_Portal(float a_portal) {
            _portal = a_portal;
            Update_Image();
        }
        public float Get_Portal()
        {
            return _portal;
        }
        public void Set_Smell(bool is_smell) {
            _smell = is_smell;
            Update_Image();
        }
        public bool Get_Smell()
        {
            return _smell;
        }
        public void Set_Wind(bool is_wind) {
            _wind = is_wind;
            Update_Image();
        }
        public bool Get_Wind()
        {
            return _wind;
        }
        public void Set_Light(bool is_light) {
            _light = is_light;
            Update_Image();
        }
        public bool Get_Light()
        {
            return _light;
        }
        public void Set_visited(bool is_visited)
        {
            _visited = is_visited;
            Update_Image();
        }
        public bool Get_Visited()
        {
            return _visited;
        }

        public void Set_border(bool is_border)
        {
            _border = is_border;
            Update_Image();
            Console.WriteLine("Border updated");
        }
        public bool Get_border()
        {
            return _border;
        }


        public void Update_Image()
        {
            if (_agent == true)
            {
                Image = "a";
            }
            else if (_monster == 1.0f)
            {
                Image = "m";
            }
            else if (_cliff == 1.0f)
            {
                Image = "c";
            }
            else if (_portal == 1.0f)
            {
                Image = "p";
            }
            else if (_smell == true)
            {
                Image = "s";
            }
            else if (_wind == true)
            {
                Image = "w";
            }
            else if (_light == true)
            {
                Image = "l";
            }
            else
            {
                Image = " ";
            }
            
        }

/*        private bool _agent = false;

        private float _monster = 0f;
        private bool _smell = false;

        private float _cliff = 0f;
        private bool _wind = false;

        private float _portal = 0f;
        private bool _light = false;

        private bool _visited = false;

        private bool _border = false;

        public string Image = " ";*/

        public void Display_case()
        {
            Console.WriteLine();
            Console.WriteLine("Agent is here : " + _agent );
            Console.WriteLine("Monster is here : " + _monster );
            Console.WriteLine("Cliff is here : " + _cliff );
            Console.WriteLine("Wind is here : " + _wind );
            Console.WriteLine("Light is here : " + _light );
            Console.WriteLine("Smell is here : " + _smell );
            Console.WriteLine("Border is here : " + _border);
            Console.WriteLine("The case has been visited : " + _visited);
            Console.WriteLine("Image : " + Image);
            Console.WriteLine();
        }
    }
}
