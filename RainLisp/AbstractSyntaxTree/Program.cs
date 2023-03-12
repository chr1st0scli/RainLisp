﻿namespace RainLisp.AbstractSyntaxTree
{
    /// <summary>
    /// Program in the abstract syntax tree.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Gets or sets the alternating definitions and expressions that constitute the program.
        /// </summary>
        public IList<Node>? DefinitionsAndExpressions { get; set; }
    }
}
