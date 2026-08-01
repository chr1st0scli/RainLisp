using RainLisp.AbstractSyntaxTree;

namespace RainLisp.DerivedExpressions
{
    /// <summary>
    /// Cons stream expression as described in the syntax grammar.
    /// </summary>
    /// <param name="first">The first constituent expression.</param>
    /// <param name="second">The second constituent expression.</param>
    public class ConsStream(Expression first, Expression second)
    {
        /// <summary>
        /// Gets or sets the first constituent expression.
        /// </summary>
        public Expression First { get; init; } = first;

        /// <summary>
        /// Gets or sets the second constituent expression.
        /// </summary>
        public Expression Second { get; init; } = second;
    }
}
