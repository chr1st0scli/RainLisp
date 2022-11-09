namespace RainLisp
{
    public static class CommonLibraries
    {
        public const string LIBS = @"
(define (cadr sequence)
  (car (cdr sequence)))

(define (cddr sequence)
  (cdr (cdr sequence)))

(define (caddr sequence)
  (car (cdr (cdr sequence))))

(define (cdddr sequence)
  (cdr (cdr (cdr sequence))))

(define (cadddr sequence)
  (car (cdr (cdr (cdr sequence)))))

(define (map proc sequence)
  (if (null? sequence)
      nil
      (cons (proc (car sequence)) (map proc (cdr sequence)))))

(define (filter predicate sequence)
  (cond ((null? sequence) nil)
        ((predicate (car sequence))
         (cons (car sequence) (filter predicate (cdr sequence))))
        (else (filter predicate (cdr sequence)))))

(define (fold-left op initial sequence)
  (define (iter result rest)
    (if (null? rest)
        result
        (iter (op result (car rest)) (cdr rest))))
  (iter initial sequence))

(define (fold-right op initial sequence)
  (if (null? sequence)
      initial
      (op (car sequence) (fold-right op initial (cdr sequence)))))

(define (reduce op sequence)
  (cond ((null? sequence) nil)
        ((null? (cdr sequence)) (car sequence))
        (else (fold-left op (car sequence) (cdr sequence)))))

(define (append list1 list2)
  (fold-right cons list2 list1))

(define (reverse sequence)
  (fold-left (lambda (x y) (cons y x)) nil sequence))

(define (length sequence)
  (define (length-iter seq count)
    (if (null? seq)
        count
        (length-iter (cdr seq) (+ 1 count))))
  (length-iter sequence 0))
";
    }
}
