using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chourbland
{
    class Case
    {
        private float monster = 0f;
        private bool smell = false;

        private float cliff = 0f;
        private bool wind = false;

        private float portal = 0f;
        private bool light = false;

        private bool visited = false;

        public string image = " ";

        public Case()
        {

        }

        public void Set_Monster(float a_monster) {
            monster = a_monster;
            Update_Image();
        }

        public void Set_Cliff(float a_cleaf) {
            cliff = a_cleaf;
            Update_Image();
        }
        public void Set_Portal(float a_portal) {
            portal = a_portal;
            Update_Image();
        }
        public void Set_Smell(bool is_smell) {
            smell = is_smell;
            Update_Image();
        }
        public void Set_Wind(bool is_wind) {
            wind = is_wind;
            Update_Image();
        }
        public void Set_Light(bool is_light) {
            light = is_light;
            Update_Image();
        }


        private void Update_Image()
        {
            if (monster == 1.0f)
            {
                image = "m";
            }
            else if (cliff == 1.0f)
            {
                image = "c";
            }
            else if (portal == 1.0f)
            {
                image = "p";
            }
            else if (smell == true)
            {
                image = "s";
            }
            else if (wind == true)
            {
                image = "w";
            }
            else if (light == true)
            {
                image = "l";
            }
        }

    }
}
