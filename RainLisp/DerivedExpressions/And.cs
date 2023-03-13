using RainLisp.AbstractSyntaxTree;

namespace RainLisp.DerivedExpressions
{
    /// <summary>
    /// List of expressions combined in a boolean logical "and" fashion as described in the syntax grammar.
    /// </summary>
    public class And
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="And"/> class.
        /// </summary>
        /// <param name="expressions">A list of expressions to be combined in a boolean logical "and" fashion.</param>
        public And(IList<Expression> expressions)
            => Expressions = expressions;

        /// <summary>
        /// Gets or sets the list of expressions to be combined in a boolean logical "and" fashion.
        /// </summary>
        public IList<Expression> Expressions { get; init; }
    }
}
