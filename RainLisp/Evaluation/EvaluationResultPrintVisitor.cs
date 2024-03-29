﻿using RainLisp.Evaluation.Results;
using System.Globalization;
using System.Text;
using static RainLisp.Grammar.Delimiters;
using static RainLisp.Grammar.StringEscapableChars;

namespace RainLisp.Evaluation
{
    /// <summary>
    /// Represents a transformer of evaluation results to their string representation.
    /// A common usage scenario is writing the result of evaluation to the standard output.
    /// </summary>
    public class EvaluationResultPrintVisitor : IEvaluationResultVisitor<string>
    {
        /// <summary>
        /// Returns a string by processing a numeric evaluation result.
        /// </summary>
        /// <param name="numberDatum">The numeric evaluation result to transform.</param>
        /// <returns>A string representation of the evaluation result.</returns>
        public string VisitNumberDatum(NumberDatum numberDatum)
            => numberDatum.Value.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Returns a string by processing a boolean evaluation result.
        /// </summary>
        /// <param name="boolDatum">The boolean evaluation result to transform.</param>
        /// <returns>A string representation of the evaluation result.</returns>
        public string VisitBoolDatum(BoolDatum boolDatum)
            => boolDatum.Value.ToString().ToLower();

        /// <summary>
        /// Returns a string by processing a string evaluation result.
        /// </summary>
        /// <param name="stringDatum">The string evaluation result to transform.</param>
        /// <returns>A string representation of the evaluation result.</returns>
        public string VisitStringDatum(StringDatum stringDatum)
        {
            var sb = new StringBuilder();
            sb.Append(DOUBLE_QUOTE);

            // Printing a string requires escaping special characters back again.
            foreach (char c in stringDatum.Value)
            {
                if (c == ESCAPABLE_DOUBLE_QUOTE)
                    sb.Append(ESCAPE).Append(ESCAPABLE_DOUBLE_QUOTE);

                else if (c == ESCAPE)
                    sb.Append(ESCAPE).Append(ESCAPE);

                else if (c == NEW_LINE)
                    sb.Append(ESCAPE).Append(ESCAPABLE_NEW_LINE);

                else if (c == CARRIAGE_RETURN)
                    sb.Append(ESCAPE).Append(ESCAPABLE_CARRIAGE_RETURN);

                else if (c == TAB)
                    sb.Append(ESCAPE).Append(ESCAPABLE_TAB);

                else
                    sb.Append(c);
            }

            sb.Append(DOUBLE_QUOTE);

            return sb.ToString();
        }

        /// <summary>
        /// Returns a string by processing a datetime evaluation result.
        /// </summary>
        /// <param name="dateTimeDatum">The datetime evaluation result to transform.</param>
        /// <returns>A string representation of the evaluation result.</returns>
        public string VisitDateTimeDatum(DateTimeDatum dateTimeDatum)
            => dateTimeDatum.Value.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

        /// <summary>
        /// Returns a string by processing a quote symbol evaluation result.
        /// </summary>
        /// <param name="quoteSymbol">The quote symbol evaluation result to transform.</param>
        /// <returns>A string representation of the evaluation result.</returns>
        public string VisitQuoteSymbol(QuoteSymbol quoteSymbol)
            => quoteSymbol.SymbolText;

        /// <summary>
        /// Returns a string by processing a primitive procedure evaluation result.
        /// </summary>
        /// <param name="primitiveProcedure">The primitive procedure evaluation result to transform.</param>
        /// <returns>A string representation of the evaluation result.</returns>
        public string VisitPrimitiveProcedure(PrimitiveProcedure primitiveProcedure)
            => $"[{nameof(PrimitiveProcedure)}] {primitiveProcedure.Name}";

        /// <summary>
        /// Returns a string by processing a user procedure evaluation result.
        /// </summary>
        /// <param name="userProcedure">The user procedure evaluation result to transform.</param>
        /// <returns>A string representation of the evaluation result.</returns>
        public string VisitUserProcedure(UserProcedure userProcedure)
        {
            string parameters = "0";
            if (userProcedure.Parameters != null && userProcedure.Parameters.Count > 0)
                parameters = string.Join(", ", userProcedure.Parameters);

            return $"[{nameof(UserProcedure)}] {nameof(userProcedure.Parameters)}: {parameters}";
        }

        /// <summary>
        /// Returns a string by processing the unspecified evaluation result.
        /// </summary>
        /// <param name="unspecified">The unspecified evaluation result to transform.</param>
        /// <returns>A string representation of the evaluation result.</returns>
        public string VisitUnspecified(Unspecified unspecified)
            => string.Empty;

        /// <summary>
        /// Returns a string by processing the nil evaluation result.
        /// </summary>
        /// <param name="nil">The nil evaluation result to transform.</param>
        /// <returns>A string representation of the evaluation result.</returns>
        public string VisitNil(Nil nil)
            => "()";

        /// <summary>
        /// Returns a string by processing a pair evaluation result.
        /// </summary>
        /// <param name="pair">The pair evaluation result to transform.</param>
        /// <returns>A string representation of the evaluation result.</returns>
        public string VisitPair(Pair pair)
        {
            var sb = new StringBuilder();
            const int MAX_DEPTH = 100;
            int depth = 0;
            var nil = Nil.GetNil();

            // This is based on observing how other LISP implementations print pairs.
            void VisitPairRecursively(Pair pair, bool openParen = true)
            {
                // Do not allow this to go on forever.
                if (++depth > MAX_DEPTH)
                {
                    sb.Append("...");
                    return;
                }

                if (openParen)
                    sb.Append('(');

                if (pair.First is Pair firstPair)
                    VisitPairRecursively(firstPair);
                else
                    sb.Append(pair.First.AcceptVisitor(this));

                bool secondIsNotNil = pair.Second != nil;

                if (secondIsNotNil)
                    sb.Append(' ');

                if (pair.Second is Pair secondPair)
                    VisitPairRecursively(secondPair, false);
                else if (secondIsNotNil)
                    sb.Append(". ").Append(pair.Second.AcceptVisitor(this));

                if (openParen)
                    sb.Append(')');
            }

            VisitPairRecursively(pair);

            return sb.ToString();
        }
    }
}
