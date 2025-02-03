using RainLisp.AbstractSyntaxTree;

namespace RainLisp.DerivedExpressions
{
    /// <summary>
    /// Cons stream expression as described in the syntax grammar.
    /// </summary>
    public class ConsStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsStream"/> class.
        /// </summary>
        /// <param name="first">The first constituent expression.</param>
        /// <param name="second">The second constituent expression.</param>
        public ConsStream(Expression first, Expression second)
        {
            First = first;
            Second = second;
        }

        /// <summary>
        /// Gets or sets the first constituent expression.
        /// </summary>
        public Expression First { get; init; }

        /// <summary>
        /// Gets or sets the second constituent expression.
        /// </summary>
        public Expression Second { get; init; }
    }
}
