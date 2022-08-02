using RainLisp.AbstractSyntaxTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainLisp
{
    internal class Procedure : Node
    {
        public string[] Parameters { get; set; }

        public Node Body { get; set; }

        public Environment Environment { get; set; }
    }
}
