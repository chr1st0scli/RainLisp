using RainLisp.Evaluation.Results;
using System.Globalization;
using System.Text;

namespace RainLisp.Evaluation
{
    public class EvaluationResultPrintVisitor : IEvaluationResultVisitor<string>
    {
        public string VisitPrimitiveDatum(IPrimitiveDatum primitiveDatum)
        {
            if (primitiveDatum is PrimitiveDatum<double> doublePrimitive)
                return doublePrimitive.Value.ToString(CultureInfo.InvariantCulture);

            else if (primitiveDatum is PrimitiveDatum<bool> boolPrimitive)
                return boolPrimitive.Value.ToString().ToLower();

            else if (primitiveDatum is PrimitiveDatum<string> stringPrimitive)
                return "\"" + stringPrimitive.Value + "\"";

            else
                throw new NotImplementedException();
        }

        public string VisitPrimitiveProcedure(PrimitiveProcedure primitiveProcedure)
        {
            return $"[{nameof(PrimitiveProcedure)}] {primitiveProcedure.ProcedureType}";
        }

        public string VisitUserProcedure(UserProcedure userProcedure)
        {
            string parameters = "0";
            if (userProcedure.Parameters != null && userProcedure.Parameters.Count > 0)
                parameters = string.Join(", ", userProcedure.Parameters);

            return $"[{nameof(UserProcedure)}] {nameof(userProcedure.Parameters)}: {parameters}";
        }

        public string VisitUnspecified(Unspecified unspecified)
        {
            return string.Empty;
        }

        public string VisitNil(Nil ni)
        {
            return "()";
        }

        public string VisitPair(Pair pair)
        {
            var sb = new StringBuilder();

            const int MAX_DEPTH = 100;
            int depth = 0;

            void HandlePair(Pair pair, bool openParen = true)
            {
                if (++depth > MAX_DEPTH)
                {
                    sb.Append("...");
                    return;
                }

                if (openParen)
                    sb.Append('(');

                if (pair.First is Pair firstPair)
                    HandlePair(firstPair);
                else
                    sb.Append(pair.First.AcceptVisitor(this));

                bool secondIsNotNil = pair.Second is not Nil;
                if (secondIsNotNil)
                    sb.Append(' ');

                if (pair.Second is Pair secondPair)
                    HandlePair(secondPair, false);
                else if (secondIsNotNil)
                    sb.Append(". ").Append(pair.Second.AcceptVisitor(this));

                if (openParen)
                    sb.Append(')');
            }

            HandlePair(pair);

            return sb.ToString();
        }
    }
}
