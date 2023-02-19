using RainLisp.Evaluation.Results;
using static RainLisp.Evaluation.PrimitiveOperation;

namespace RainLisp.Evaluation
{
    public class ProcedureApplicationVisitor : IProcedureApplicationVisitor
    {
        private readonly Func<EvaluationResult, EvaluationResult> _evalPrimitiveCallback;

        public ProcedureApplicationVisitor(Func<EvaluationResult, EvaluationResult> evalPrimitiveCallback)
            => _evalPrimitiveCallback = evalPrimitiveCallback ?? throw new ArgumentNullException(nameof(evalPrimitiveCallback));

        public EvaluationResult ApplyUserProcedure(UserProcedure procedure, EvaluationResult[]? evaluatedArguments, IEvaluatorVisitor evaluatorVisitor)
        {
            var extendedEnvironment = procedure.Environment.ExtendEnvironment(procedure.Parameters, evaluatedArguments);

            return evaluatorVisitor.EvaluateBody(procedure.Body, extendedEnvironment);
        }

        public EvaluationResult ApplyPrimitiveProcedure(PrimitiveProcedure procedure, EvaluationResult[]? evaluatedArguments)
        {
            // Dispatch to different methods based on enum (message-passing style) instead of different procedure runtime types to avoid class explosion for primitive operations.
            return procedure.ProcedureType switch
            {
                PrimitiveProcedureType.Add => Add(evaluatedArguments),
                PrimitiveProcedureType.Subtract => Subtract(evaluatedArguments),
                PrimitiveProcedureType.Multiply => Multiply(evaluatedArguments),
                PrimitiveProcedureType.Divide => Divide(evaluatedArguments),
                PrimitiveProcedureType.Modulo => Modulo(evaluatedArguments),
                PrimitiveProcedureType.GreaterThan => GreaterThan(evaluatedArguments),
                PrimitiveProcedureType.GreaterThanOrEqualTo => GreaterThanOrEqualTo(evaluatedArguments),
                PrimitiveProcedureType.LessThan => LessThan(evaluatedArguments),
                PrimitiveProcedureType.LessThanOrEqualTo => LessThanOrEqualTo(evaluatedArguments),
                PrimitiveProcedureType.EqualTo => EqualTo(evaluatedArguments),
                PrimitiveProcedureType.LogicalXor => LogicalXor(evaluatedArguments),
                PrimitiveProcedureType.LogicalNot => LogicalNot(evaluatedArguments),
                PrimitiveProcedureType.Cons => Cons(evaluatedArguments),
                PrimitiveProcedureType.Car => Car(evaluatedArguments),
                PrimitiveProcedureType.Cdr => Cdr(evaluatedArguments),
                PrimitiveProcedureType.List => List(evaluatedArguments),
                PrimitiveProcedureType.IsNull => IsNull(evaluatedArguments),
                PrimitiveProcedureType.SetCar => SetCar(evaluatedArguments),
                PrimitiveProcedureType.SetCdr => SetCdr(evaluatedArguments),
                PrimitiveProcedureType.StringLength => StringLength(evaluatedArguments),
                PrimitiveProcedureType.Substring => Substring(evaluatedArguments),
                PrimitiveProcedureType.IndexOfString => IndexOfString(evaluatedArguments),
                PrimitiveProcedureType.ReplaceString => ReplaceString(evaluatedArguments),
                PrimitiveProcedureType.ToLower => ToLower(evaluatedArguments),
                PrimitiveProcedureType.ToUpper => ToUpper(evaluatedArguments),
                PrimitiveProcedureType.Display => Display(evaluatedArguments),
                PrimitiveProcedureType.Debug => Debug(evaluatedArguments),
                PrimitiveProcedureType.Trace => Trace(evaluatedArguments),
                PrimitiveProcedureType.NewLine => NewLine(evaluatedArguments),
                PrimitiveProcedureType.Error => Error(evaluatedArguments),
                PrimitiveProcedureType.Now => Now(evaluatedArguments),
                PrimitiveProcedureType.UtcNow => UtcNow(evaluatedArguments),
                PrimitiveProcedureType.MakeDate => MakeDate(evaluatedArguments),
                PrimitiveProcedureType.MakeDateTime => MakeDateTime(evaluatedArguments),
                PrimitiveProcedureType.Year => Year(evaluatedArguments),
                PrimitiveProcedureType.Month => Month(evaluatedArguments),
                PrimitiveProcedureType.Day => Day(evaluatedArguments),
                PrimitiveProcedureType.Hour => Hour(evaluatedArguments),
                PrimitiveProcedureType.Minute => Minute(evaluatedArguments),
                PrimitiveProcedureType.Second => Second(evaluatedArguments),
                PrimitiveProcedureType.Millisecond => Millisecond(evaluatedArguments),
                PrimitiveProcedureType.IsUtc => IsUtc(evaluatedArguments),
                PrimitiveProcedureType.ToLocal => ToLocal(evaluatedArguments),
                PrimitiveProcedureType.ToUtc => ToUtc(evaluatedArguments),
                PrimitiveProcedureType.AddYears => AddYears(evaluatedArguments),
                PrimitiveProcedureType.AddMonths => AddMonths(evaluatedArguments),
                PrimitiveProcedureType.AddDays => AddDays(evaluatedArguments),
                PrimitiveProcedureType.AddHours => AddHours(evaluatedArguments),
                PrimitiveProcedureType.AddMinutes => AddMinutes(evaluatedArguments),
                PrimitiveProcedureType.AddSeconds => AddSeconds(evaluatedArguments),
                PrimitiveProcedureType.AddMilliseconds => AddMilliseconds(evaluatedArguments),
                PrimitiveProcedureType.DaysDiff => DaysDiff(evaluatedArguments),
                PrimitiveProcedureType.HoursDiff => HoursDiff(evaluatedArguments),
                PrimitiveProcedureType.MinutesDiff => MinutesDiff(evaluatedArguments),
                PrimitiveProcedureType.SecondsDiff => SecondsDiff(evaluatedArguments),
                PrimitiveProcedureType.MillisecondsDiff => MillisecondsDiff(evaluatedArguments),
                PrimitiveProcedureType.ParseDateTime => ParseDateTime(evaluatedArguments),
                PrimitiveProcedureType.DateTimeToString => DateTimeToString(evaluatedArguments),
                PrimitiveProcedureType.NumberToString => NumberToString(evaluatedArguments),
                PrimitiveProcedureType.ParseNumber => ParseNumber(evaluatedArguments),
                PrimitiveProcedureType.Round => Round(evaluatedArguments),
                PrimitiveProcedureType.Eval => Eval(evaluatedArguments, _evalPrimitiveCallback),
                _ => throw new NotImplementedException()
            };
        }
    }
}
