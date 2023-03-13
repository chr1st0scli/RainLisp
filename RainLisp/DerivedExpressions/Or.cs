using RainLisp.AbstractSyntaxTree;

namespace RainLisp.DerivedExpressions
{
    /// <summary>
    /// List of expressions combined in a boolean logical "or" fashion as described in the syntax grammar.
    /// </summary>
    public class Or
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Or"/> class.
        /// </summary>
        /// <param name="expressions">A list of expressions to be combined in a boolean logical "or" fashion.</param>
        public Or(IList<Expression> expressions)
            => Expressions = expressions;

        /// <summary>
        /// Gets or sets the list of expressions to be combined in a boolean logical "or" fashion.
        /// </summary>
        public IList<Expression> Expressions { get; init; }
    }
}
