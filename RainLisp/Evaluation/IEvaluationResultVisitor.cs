using RainLisp.Evaluation.Results;

namespace RainLisp.Evaluation
{
    /// <summary>
    /// Represents a transformer of evaluation results.
    /// </summary>
    /// <typeparam name="T">The type to transform evaluation results to.</typeparam>
    public interface IEvaluationResultVisitor<out T>
    {
        /// <summary>
        /// Returns a <typeparamref name="T"/> by processing a numeric evaluation result.
        /// </summary>
        /// <param name="numberDatum">The numeric evaluation result to transform.</param>
        /// <returns>A <typeparamref name="T"/> from the evaluation result.</returns>
        T VisitNumberDatum(NumberDatum numberDatum);

        /// <summary>
        /// Returns a <typeparamref name="T"/> by processing a boolean evaluation result.
        /// </summary>
        /// <param name="boolDatum">The boolean evaluation result to transform.</param>
        /// <returns>A <typeparamref name="T"/> from the evaluation result.</returns>
        T VisitBoolDatum(BoolDatum boolDatum);

        /// <summary>
        /// Returns a <typeparamref name="T"/> by processing a string evaluation result.
        /// </summary>
        /// <param name="stringDatum">The string evaluation result to transform.</param>
        /// <returns>A <typeparamref name="T"/> from the evaluation result.</returns>
        T VisitStringDatum(StringDatum stringDatum);

        /// <summary>
        /// Returns a <typeparamref name="T"/> by processing a datetime evaluation result.
        /// </summary>
        /// <param name="dateTimeDatum">The datetime evaluation result to transform.</param>
        /// <returns>A <typeparamref name="T"/> from the evaluation result.</returns>
        T VisitDateTimeDatum(DateTimeDatum dateTimeDatum);

        /// <summary>
        /// Returns a <typeparamref name="T"/> by processing a quote symbol evaluation result.
        /// </summary>
        /// <param name="quoteSymbol">The quote symbol evaluation result to transform.</param>
        /// <returns>A <typeparamref name="T"/> from the evaluation result.</returns>
        T VisitQuoteSymbol(QuoteSymbol quoteSymbol);

        /// <summary>
        /// Returns a <typeparamref name="T"/> by processing a primitive procedure evaluation result.
        /// </summary>
        /// <param name="primitiveProcedure">The primitive procedure evaluation result to transform.</param>
        /// <returns>A <typeparamref name="T"/> from the evaluation result.</returns>
        T VisitPrimitiveProcedure(PrimitiveProcedure primitiveProcedure);

        /// <summary>
        /// Returns a <typeparamref name="T"/> by processing a user procedure evaluation result.
        /// </summary>
        /// <param name="userProcedure">The user procedure evaluation result to transform.</param>
        /// <returns>A <typeparamref name="T"/> from the evaluation result.</returns>
        T VisitUserProcedure(UserProcedure userProcedure);

        /// <summary>
        /// Returns a <typeparamref name="T"/> by processing the unspecified evaluation result.
        /// </summary>
        /// <param name="unspecified">The unspecified evaluation result to transform.</param>
        /// <returns>A <typeparamref name="T"/> from the evaluation result.</returns>
        T VisitUnspecified(Unspecified unspecified);

        /// <summary>
        /// Returns a <typeparamref name="T"/> by processing the nil evaluation result.
        /// </summary>
        /// <param name="nil">The nil evaluation result to transform.</param>
        /// <returns>A <typeparamref name="T"/> from the evaluation result.</returns>
        T VisitNil(Nil nil);

        /// <summary>
        /// Returns a <typeparamref name="T"/> by processing a pair evaluation result.
        /// </summary>
        /// <param name="pair">The pair evaluation result to transform.</param>
        /// <returns>A <typeparamref name="T"/> from the evaluation result.</returns>
        T VisitPair(Pair pair);
    }
}
