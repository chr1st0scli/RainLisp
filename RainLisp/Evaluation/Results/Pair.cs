using System.Text;

namespace RainLisp.Evaluation.Results
{
    public class Pair : EvaluationResult
    {
        public Pair(EvaluationResult first, EvaluationResult second)
        {
            First = first;
            Second = second;
        }

        public EvaluationResult First { get; set; }

        public EvaluationResult Second { get; set; }

        public override string? ToString()
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
                    sb.Append(pair.First.ToString());

                bool secondIsNotNil = pair.Second is not Nil;
                if (secondIsNotNil)
                    sb.Append(' ');

                if (pair.Second is Pair secondPair)
                    HandlePair(secondPair, false);
                else if (secondIsNotNil)
                    sb.Append(". ").Append(pair.Second.ToString());

                if (openParen)
                    sb.Append(')');
            }

            HandlePair(this);

            return sb.ToString();
        }
    }
}
