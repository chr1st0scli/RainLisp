using RainLisp.Evaluation.Results;

namespace RainLisp.Evaluation
{
    public interface IEvaluationResultVisitor<out T>
    {
        T VisitPrimitiveDatum(IPrimitiveDatum primitiveDatum);

        T VisitPrimitiveProcedure(PrimitiveProcedure primitiveProcedure);

        T VisitUserProcedure(UserProcedure userProcedure);

        T VisitUnspecified(Unspecified unspecified);

        T VisitNil(Nil ni);

        T VisitPair(Pair pair);
    }
}
