using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chourbland
{
    class Regle

        {
            public Smell smell { get; set; }
            public Wind wind { get; set; }
            public Shine shine { get; set; }
            public Nothing nothing { get; set; }
        }

        public class Smell
        {
            public string danger { get; set; }
        }

        public class Wind
        {
            public string danger { get; set; }
        }

        public class Shine
        {
            public string danger { get; set; }
            public string goal { get; set; }
        }

        public class Nothing
        {
            public string danger { get; set; }
        }
}
