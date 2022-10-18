namespace RainLisp
{
    public static class CommonLibraries
    {
        public static string[] FunctionNames { get; } = new[] { "map", "filter", "fold-left", "fold-right" };

        public const string LIBS = @"
(define (map op list)
  (if (null? list)
      nil
  	(cons (op (car list)) (map op (cdr list)))))

(define (filter op list)
  (if (null? list)
      nil
      (let ((first (car list))
            (rest (cdr list)))
        (if (op first)
            (cons first (filter op rest))
            (filter op rest)))))

(define (fold-left op list initial)
  (if (null? list)
      initial
      (fold-left op (cdr list) (op (car list) initial))))

(define (fold-right op list initial)
  (if (null? list)
      initial
      (op (car list) (fold-right op (cdr list) initial))))
";
    }
}
