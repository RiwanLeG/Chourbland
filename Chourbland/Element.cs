using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chourbland
{
    class Element
    {
        public Tuple<int,int> position;
        private String type;

        public Element(Tuple<int, int> new_position, String new_type)
        {
            position = new_position;
            type = new_type;
        }

        public String Get_type()
        {
            return type;
        }
    }
}
