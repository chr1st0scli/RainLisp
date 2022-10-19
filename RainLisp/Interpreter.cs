﻿using RainLisp.AbstractSyntaxTree;
using RainLisp.Environment;
using RainLisp.Evaluation;
using RainLisp.Parsing;
using RainLisp.Tokenization;
using static RainLisp.Grammar.Primitives;

namespace RainLisp
{
    public class Interpreter
    {
        private readonly ITokenizer _tokenizer;
        private readonly IParser _parser;
        private readonly IEvaluatorVisitor _evaluator;
        private readonly IEnvironmentFactory? _environmentFactory;

        public Interpreter(ITokenizer? tokenizer = null, IParser? parser = null, IEvaluatorVisitor? evaluator = null, IEnvironmentFactory? environmentFactory = null)
        {
            _tokenizer = tokenizer ?? new Tokenizer();
            _parser = parser ?? new Parser();
            _evaluator = evaluator ?? new EvaluatorVisitor(new ProcedureApplicationVisitor());
            _environmentFactory = environmentFactory;
        }

        public object Evaluate(string expression)
        {
            IEvaluationEnvironment? environment = null;
            return Evaluate(expression, ref environment);
        }

        public object Evaluate(Program program)
        {
            IEvaluationEnvironment? environment = null;
            return Evaluate(program, ref environment);
        }

        public object Evaluate(string expression, ref IEvaluationEnvironment? environment)
        {
            var tokens = _tokenizer.Tokenize(expression);
            var programAST = _parser.Parse(tokens);

            return Evaluate(programAST, ref environment);
        }

        public object Evaluate(Program program, ref IEvaluationEnvironment? environment)
        {
            environment ??= CreateGlobalEnvironment();

            return _evaluator.EvaluateProgram(program, environment);
        }

        public void ReadEvalPrintLoop(Func<string> read, Action<string> print)
        {
            ArgumentNullException.ThrowIfNull(read, nameof(read));
            ArgumentNullException.ThrowIfNull(print, nameof(print));

            var environment = CreateGlobalEnvironment();

            while (true)
            {
                try
                {
                    string expression = read();

                    var tokens = _tokenizer.Tokenize(expression);
                    var programAST = _parser.Parse(tokens);

                    var result = _evaluator.EvaluateProgram(programAST, environment);

                    print(result.ToString()!);
                }
                catch (Exception ex)
                {
                    print(ex.ToString());
                }
            }
        }

        private IEvaluationEnvironment CreateGlobalEnvironment()
        {
            var environment = _environmentFactory?.CreateEnvironment() ?? new EvaluationEnvironment();

            // Define primitive procedures.
            environment.DefineIdentifier(PLUS, new PrimitiveProcedure(PrimitiveProcedureType.Add));
            environment.DefineIdentifier(MINUS, new PrimitiveProcedure(PrimitiveProcedureType.Subtract));
            environment.DefineIdentifier(MULTIPLY, new PrimitiveProcedure(PrimitiveProcedureType.Multiply));
            environment.DefineIdentifier(DIVIDE, new PrimitiveProcedure(PrimitiveProcedureType.Divide));
            environment.DefineIdentifier(MODULO, new PrimitiveProcedure(PrimitiveProcedureType.Modulo));
            environment.DefineIdentifier(GREATER, new PrimitiveProcedure(PrimitiveProcedureType.GreaterThan));
            environment.DefineIdentifier(GREATER_OR_EQUAL, new PrimitiveProcedure(PrimitiveProcedureType.GreaterThanOrEqualTo));
            environment.DefineIdentifier(LESS, new PrimitiveProcedure(PrimitiveProcedureType.LessThan));
            environment.DefineIdentifier(LESS_OR_EQUAL, new PrimitiveProcedure(PrimitiveProcedureType.LessThanOrEqualTo));
            environment.DefineIdentifier(EQUAL, new PrimitiveProcedure(PrimitiveProcedureType.EqualTo));
            environment.DefineIdentifier(XOR, new PrimitiveProcedure(PrimitiveProcedureType.LogicalXor));
            environment.DefineIdentifier(NOT, new PrimitiveProcedure(PrimitiveProcedureType.LogicalNot));
            environment.DefineIdentifier(CONS, new PrimitiveProcedure(PrimitiveProcedureType.Cons));
            environment.DefineIdentifier(CAR, new PrimitiveProcedure(PrimitiveProcedureType.Car));
            environment.DefineIdentifier(CDR, new PrimitiveProcedure(PrimitiveProcedureType.Cdr));
            environment.DefineIdentifier(LIST, new PrimitiveProcedure(PrimitiveProcedureType.List));
            environment.DefineIdentifier(IS_NULL, new PrimitiveProcedure(PrimitiveProcedureType.IsNull));

            environment.DefineIdentifier(NIL, new Nil());

            // Install common libraries in the environment.
            IEvaluationEnvironment? env = environment;
            Evaluate(CommonLibraries.LIBS, ref env);

            return environment;
        }
    }
}
