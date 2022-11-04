using RainLisp.Evaluation.Results;

namespace RainLisp.Evaluation
{
    public interface IEvaluationResultVisitor<out T>
    {
        T VisitNumberDatum(NumberDatum numberDatum);

        T VisitBoolDatum(BoolDatum boolDatum);

        T VisitStringDatum(StringDatum stringDatum);

        T VisitPrimitiveProcedure(PrimitiveProcedure primitiveProcedure);

        T VisitUserProcedure(UserProcedure userProcedure);

        T VisitUnspecified(Unspecified unspecified);

        T VisitNil(Nil ni);

        T VisitPair(Pair pair);
    }
}
