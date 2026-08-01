using RainLisp.AbstractSyntaxTree;

namespace RainLisp.DerivedExpressions
{
    /// <summary>
    /// List of expressions combined in a boolean logical "or" fashion as described in the syntax grammar.
    /// </summary>
    /// <param name="expressions">A list of expressions to be combined in a boolean logical "or" fashion.</param>
    public class Or(IList<Expression> expressions)
    {
        /// <summary>
        /// Gets or sets the list of expressions to be combined in a boolean logical "or" fashion.
        /// </summary>
        public IList<Expression> Expressions { get; init; } = expressions;
    }
}
