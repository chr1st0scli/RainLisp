namespace RainLisp
{
    /// <summary>
    /// Common LISP libraries defined in the language itself.
    /// </summary>
    internal static class LispLibraries
    {
        internal const string LIBS = @"

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

(define (flatmap proc sequence)
  (fold-right append nil (map proc sequence)))

(define (length sequence)
  (fold-left (lambda (count n) (+ count 1)) 0 sequence))

; Stream related procedures.
(define (force proc)
  (proc))

(define (cdr-stream pair)
  (force (cdr pair)))

(define (make-range-stream start end)
  (if (> start end)
      nil
      (cons-stream start (make-range-stream (+ start 1) end))))

; car and cdr flavors.
; 2 levels.
(define (cddr sequence)
  (cdr (cdr sequence)))

(define (cadr sequence)
  (car (cdr sequence)))

(define (caar sequence)
  (car (car sequence)))

(define (cdar sequence)
  (cdr (car sequence)))

; 3 levels.
(define (cdddr sequence)
  (cdr (cddr sequence)))

(define (caddr sequence)
  (car (cddr sequence)))

(define (caadr sequence)
  (car (cadr sequence)))

(define (caaar sequence)
  (car (caar sequence)))

(define (cdaar sequence)
  (cdr (caar sequence)))

(define (cddar sequence)
  (cdr (cdar sequence)))

(define (cdadr sequence)
  (cdr (cadr sequence)))

(define (cadar sequence)
  (car (cdar sequence)))

; 4 levels.
(define (cddddr sequence)
  (cdr (cdddr sequence)))

(define (cadddr sequence)
  (car (cdddr sequence)))

(define (caaddr sequence)
  (car (caddr sequence)))

(define (caaadr sequence)
  (car (caadr sequence)))

(define (caaaar sequence)
  (car (caaar sequence)))

(define (cdaaar sequence)
  (cdr (caaar sequence)))

(define (cddaar sequence)
  (cdr (cdaar sequence)))

(define (cdddar sequence)
  (cdr (cddar sequence)))

(define (cdadar sequence)
  (cdr (cadar sequence)))

(define (cadadr sequence)
  (car (cdadr sequence)))

(define (cdaddr sequence)
  (cdr (caddr sequence)))

(define (cddadr sequence)
  (cdr (cdadr sequence)))

(define (cadaar sequence)
  (car (cdaar sequence)))

(define (caadar sequence)
  (car (cadar sequence)))

(define (cdaadr sequence)
  (cdr (caadr sequence)))

(define (caddar sequence)
  (car (cddar sequence)))
";
    }
}
