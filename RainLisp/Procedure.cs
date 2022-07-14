using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainLisp
{
    internal class Procedure : Expression
    {
        public string[] Parameters { get; set; }

        public Expression Body { get; set; }

        public Environment Environment { get; set; }
    }
}
