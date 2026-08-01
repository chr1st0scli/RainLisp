using RainLisp.AbstractSyntaxTree;

namespace RainLisp.DerivedExpressions
{
    /// <summary>
    /// List of expressions combined in a boolean logical "and" fashion as described in the syntax grammar.
    /// </summary>
    /// <param name="expressions">A list of expressions to be combined in a boolean logical "and" fashion.</param>
    public class And(IList<Expression> expressions)
    {
        /// <summary>
        /// Gets or sets the list of expressions to be combined in a boolean logical "and" fashion.
        /// </summary>
        public IList<Expression> Expressions { get; init; } = expressions;
    }
}
