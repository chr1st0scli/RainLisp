using RainLisp.Evaluation.Results;
using System.Globalization;
using System.Text;
using static RainLisp.Grammar.Delimiters;
using static RainLisp.Grammar.StringEscapableChars;

namespace RainLisp.Evaluation
{
    public class EvaluationResultPrintVisitor : IEvaluationResultVisitor<string>
    {
        public string VisitNumberDatum(NumberDatum numberDatum)
            => numberDatum.Value.ToString(CultureInfo.InvariantCulture);

        public string VisitBoolDatum(BoolDatum boolDatum)
            => boolDatum.Value.ToString().ToLower();

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

        public string VisitPrimitiveProcedure(PrimitiveProcedure primitiveProcedure)
            => $"[{nameof(PrimitiveProcedure)}] {primitiveProcedure.ProcedureType}";

        public string VisitUserProcedure(UserProcedure userProcedure)
        {
            string parameters = "0";
            if (userProcedure.Parameters != null && userProcedure.Parameters.Count > 0)
                parameters = string.Join(", ", userProcedure.Parameters);

            return $"[{nameof(UserProcedure)}] {nameof(userProcedure.Parameters)}: {parameters}";
        }

        public string VisitUnspecified(Unspecified unspecified)
            => string.Empty;

        public string VisitNil(Nil ni)
            => "()";

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

        public string VisitProgramResult(ProgramResult programResult)
        {
            if (programResult.Results == null || programResult.Results.Count == 0)
                return string.Empty;

            var sb = new StringBuilder();
            foreach (var result in programResult.Results)
                sb.AppendLine(result.AcceptVisitor(this));

            return sb.ToString();
        }
    }
}
