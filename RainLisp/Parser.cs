using RainLisp.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainLisp
{
    internal class Parser
    {
        public void Parse(List<string> tokens)
        {
            // (if (> a 0) (+ 1 1 1) (foo1 (foo2 (foo3))))
            // (if (> a 0) (+ 1 1 1) (+ 2 2))
            // (if (lambda (a) (> a 0) 4) (+ 1 1 1) (+ 2 2))

            

            int[,] arr = new int[3, 4];
            arr[0, 2] = 3;

            string[][] arr2 = new string[4][];
            arr2[0] = new string[1];
            arr2[1] = new string[3];
            arr2[2] = new string[4];
            arr2[3] = new string[3];

            Expression? currExpression = null;

            for (int i = 0; i < tokens.Count; i++)
            {
                var currToken = tokens[i];

                if (currToken == "(")
                {
                    // start expression
                }
                else if (currToken == ")")
                {
                    // close expression
                }
            }
        }

        private IfExpression GetIfExpression() => new IfExpression();
    }
}
