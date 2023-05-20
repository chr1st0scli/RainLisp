# Data-directed Programming
What if we wanted to have some code that does different things depending on the type of the object it operates on?

Suppose we have some code about pets, e.g. a "sound" procedure that produces different sounds based on the kind of the animal.
A cat would say "meow" and a dog would say "woof". The idea is that we want the "sound" procedure to be reusable
independently of the actual type of pet. It should not need to change to support different kinds of pets or know anything
about them.

This is called data-directed programming and if you are familiar with object oriented programming, you have probably
spotted the resemblance with *polymorphism* already.

To make this work, we need a two-dimensional lookup table, i.e. a table where a value (the actual procedure to call)
is searched based on two keys. The first key is the operation (e.g. sound) and the second one is the type of the object (e.g. cat).

> Similarly, in a statically typed, object oriented programming language, virtual methods are implemented with virtual tables.

## Prerequisites

### 1-d Table
In order to create a two-dimensional table, let's start with one dimension.

```scheme
(define (make-table)
  (list '*table*))

; 1D table.
(define (lookup key table)
  (let ((record (assoc key (cdr table))))
    (if record
        (cdr record)
        false)))

(define (assoc key records)
  (cond ((null? records) false)
        ((= key (caar records)) (car records))
        (else (assoc key (cdr records)))))

(define (insert! key value table)
  (let ((record (assoc key (cdr table))))
    (if record
        (set-cdr! record value)
        (set-cdr! table 
                  (cons (cons key value) (cdr table))))))
```

### 2-d Table
Let's build the two-dimensional table.

```scheme
; 2D table.
(define (lookup-2d key1 key2 table)
  (let ((subtable (assoc key1 (cdr table))))
    (if subtable
        (lookup key2 subtable)
        false)))

(define (insert-2d! key1 key2 value table)
  (let ((subtable (assoc key1 (cdr table))))
    (if subtable
        (insert! key2 value subtable)

        (set-cdr! table
                  (cons (list key1 (cons key2 value))
                        (cdr table))))))

(define dispatch-table (make-table))

(define (put key1 key2 value)
  (insert-2d! key1 key2 value dispatch-table))

(define (get key1 key2)
  (lookup-2d key1 key2 dispatch-table))
```

If you want a deeper insight or explanation about tables, have a look at
[SICP](https://mitp-content-server.mit.edu/books/content/sectbyfn/books_pres_0/6515/sicp.zip/full-text/book/book-Z-H-22.html#%_sec_3.3.3).
But the essence of this section follows. You can take the table facility for granted and come back later if you want.

### Helpers

```scheme
; Tagging helpers.
(define (set-tag tag object)
  (cons tag object))

(define (get-tag object)
  (car object))

(define (contents object)
  (cdr object))
```

## User Modules
Now, we have everything we need to tackle with the actual problem and see data-directed programming in action.

### Cat Module

```scheme
; User modules.
; Cat type.
(define (install-cat-package)
  (define type 'cat)
  ; constructor and getters.
  (define (make name age) (cons name age))
  (define (get-name cat) (car cat))
  (define (get-age cat) (cdr cat))
  ; cat specific operations.
  (define (sound) "meow")
  (define (likes) "fish")
  (define (kind) "cat")

  ; Install the procedure-type bindings in the table.
  (put 'sound type sound)
  (put 'likes type likes)
  (put 'kind type kind)
  (put 'get-name type get-name)
  (put 'get-age type get-age)
  ; Registers a user procedure that creates a cat and attaches the appropriate tag to it.
  (put 'make type (lambda (name age) (set-tag type (make name age))))
  'ok)
```

### Dog Module

```scheme
; Dog type.
(define (install-dog-package)
  (define type 'dog)
  ; constructor and getters.
  (define (make name age) (cons name age))
  (define (get-name dog) (car dog))
  (define (get-age dog) (cdr dog))
  ; dog specific operations.
  (define (sound) "woof")
  (define (likes) "bone")
  (define (kind) "dog")

  ; Install the procedure-type bindings in the table.
  (put 'sound type sound)
  (put 'likes type likes)
  (put 'kind type kind)
  (put 'get-name type get-name)
  (put 'get-age type get-age)
  ; Registers a user procedure that creates a dog and attaches the appropriate tag to it.
  (put 'make type (lambda (name age) (set-tag type (make name age))))
  'ok)
```

### Reusable Code

```scheme
; Code that does not need to change no matter how many user packages are installed.
(define (apply-to-pet operation pet supply-pet-as-arg)
  ; Retrieve the right procedure based on the pet's type.
  (let ((proc (get operation (get-tag pet))))
    (if proc
        (if supply-pet-as-arg
            (proc (contents pet))
            (proc))
        (error "Unknown operation."))))

(define (sound pet)
  (apply-to-pet 'sound pet false))

(define (likes pet)
  (apply-to-pet 'likes pet false))

(define (kind pet)
  (apply-to-pet 'kind pet false))

(define (get-name pet)
  (apply-to-pet 'get-name pet true))

(define (get-age pet)
  (apply-to-pet 'get-age pet true))

(define (present pet)
  (display (+ "My pet's name is " (get-name pet) "."))
  (newline)
  (display (+ "It's a " (kind pet) ", "))
  (display (+ "it's " (number-to-string (get-age pet) "") " years old, "))
  (display (+ "it says " (sound pet) " "))
  (display (+ "and it likes " (likes pet) "."))
  (newline))
```

### Installation And Execution

```scheme
(install-cat-package)
(install-dog-package)

; Helper constructors.
(define (make-cat name age)
  ((get 'make 'cat) name age))

(define (make-dog name age)
  ((get 'make 'dog) name age))

; Present two different pets.
(define my-cat (make-cat "Ruby" 8))
(define my-dog (make-dog "August" 16))

(present my-cat)
(present my-dog)
```

## Complete Program Listing

```scheme
(define (make-table)
  (list '*table*))

; 1D table.
(define (lookup key table)
  (let ((record (assoc key (cdr table))))
    (if record
        (cdr record)
        false)))

(define (assoc key records)
  (cond ((null? records) false)
        ((= key (caar records)) (car records))
        (else (assoc key (cdr records)))))

(define (insert! key value table)
  (let ((record (assoc key (cdr table))))
    (if record
        (set-cdr! record value)
        (set-cdr! table 
                  (cons (cons key value) (cdr table))))))

; 2D table.
(define (lookup-2d key1 key2 table)
  (let ((subtable (assoc key1 (cdr table))))
    (if subtable
        (lookup key2 subtable)
        false)))

(define (insert-2d! key1 key2 value table)
  (let ((subtable (assoc key1 (cdr table))))
    (if subtable
        (insert! key2 value subtable)

        (set-cdr! table
                  (cons (list key1 (cons key2 value))
                        (cdr table))))))

(define dispatch-table (make-table))

(define (put key1 key2 value)
  (insert-2d! key1 key2 value dispatch-table))

(define (get key1 key2)
  (lookup-2d key1 key2 dispatch-table))

; Tagging helpers.
(define (set-tag tag object)
  (cons tag object))

(define (get-tag object)
  (car object))

(define (contents object)
  (cdr object))

; User modules.
; Cat type.
(define (install-cat-package)
  (define type 'cat)
  ; constructor and getters.
  (define (make name age) (cons name age))
  (define (get-name cat) (car cat))
  (define (get-age cat) (cdr cat))
  ; cat specific operations.
  (define (sound) "meow")
  (define (likes) "fish")
  (define (kind) "cat")

  ; Install the procedure-type bindings in the table.
  (put 'sound type sound)
  (put 'likes type likes)
  (put 'kind type kind)
  (put 'get-name type get-name)
  (put 'get-age type get-age)
  ; Registers a user procedure that creates a cat and attaches the appropriate tag to it.
  (put 'make type (lambda (name age) (set-tag type (make name age))))
  'ok)

; Dog type.
(define (install-dog-package)
  (define type 'dog)
  ; constructor and getters.
  (define (make name age) (cons name age))
  (define (get-name dog) (car dog))
  (define (get-age dog) (cdr dog))
  ; dog specific operations.
  (define (sound) "woof")
  (define (likes) "bone")
  (define (kind) "dog")

  ; Install the procedure-type bindings in the table.
  (put 'sound type sound)
  (put 'likes type likes)
  (put 'kind type kind)
  (put 'get-name type get-name)
  (put 'get-age type get-age)
  ; Registers a user procedure that creates a dog and attaches the appropriate tag to it.
  (put 'make type (lambda (name age) (set-tag type (make name age))))
  'ok)

; Code that does not need to change no matter how many user packages are installed.
(define (apply-to-pet operation pet supply-pet-as-arg)
  ; Retrieve the right procedure based on the pet's type.
  (let ((proc (get operation (get-tag pet))))
    (if proc
        (if supply-pet-as-arg
            (proc (contents pet))
            (proc))
        (error "Unknown operation."))))

(define (sound pet)
  (apply-to-pet 'sound pet false))

(define (likes pet)
  (apply-to-pet 'likes pet false))

(define (kind pet)
  (apply-to-pet 'kind pet false))

(define (get-name pet)
  (apply-to-pet 'get-name pet true))

(define (get-age pet)
  (apply-to-pet 'get-age pet true))

(define (present pet)
  (display (+ "My pet's name is " (get-name pet) "."))
  (newline)
  (display (+ "It's a " (kind pet) ", "))
  (display (+ "it's " (number-to-string (get-age pet) "") " years old, "))
  (display (+ "it says " (sound pet) " "))
  (display (+ "and it likes " (likes pet) "."))
  (newline))

(install-cat-package)
(install-dog-package)

; Helper constructors.
(define (make-cat name age)
  ((get 'make 'cat) name age))

(define (make-dog name age)
  ((get 'make 'dog) name age))

; Present two different pets.
(define my-cat (make-cat "Ruby" 8))
(define my-dog (make-dog "August" 16))

(present my-cat)
(present my-dog)
```
->
```
ok
ok
My pet's name is Ruby.
It's a cat, it's 8 years old, it says meow and it likes fish.
My pet's name is August.
It's a dog, it's 16 years old, it says woof and it likes bone.
```

Let's see another interesting capability, [metaprogramming](metaprogramming.md).
