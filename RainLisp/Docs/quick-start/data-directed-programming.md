# Data-directed Programming
What if we wanted to have some code that does different things depending on the type of the object it operates on?

Suppose we have some code about pets, e.g. a "sound" procedure that produces different sounds based on the animal's kind.
A cat would say "meow" and a dog would say "woof". The idea is that we want the "sound" procedure to be generic, reusable
independently of the actual type of the pet. It should not need to change to support different kinds of pets or know anything
about them.

This is called data-directed programming and if you are familiar with object oriented programming, you have probably
spotted the resemblance with *polymorphism* already.

## Prerequisites
To make this work, we need a two-dimensional lookup table, i.e. a table where a value, i.e. the actual procedure to call,
is searched based on two keys. The first key is the operation, e.g. "sound" or other things animals do.
The second one is the type of the object, e.g. "cat" or "dog".

> Similarly, in a statically typed, object oriented programming language, virtual methods are implemented with virtual tables.

### 1-D Table
First, we can create a one-dimensional table that the two-dimensional one will be based on.

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

A one-dimensional table is implemented as a list of pairs. Each pair is the actual key-value pair.
Apart from the pairs, the table always starts with the special quote symbol `'*table*`. This is just a placeholder
that enables us to easily insert a new pair at the start of the table.

The `lookup` procedure searches for `key` in the `table`. If it is found, it returns the associated value; otherwise, `false`.

The `assoc` procedure is the one that does the actual recursive search among the pairs. If the key is found, the respective key-value
pair is returned; otherwise, `false`.

The `insert!` procedure searches for the `key` in the `table`. If it is found, it simply updates its respective value.
Otherwise, it creates a new key-value pair and inserts it at the start of the table, just after `'*table*`.

### 2-D Table
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

A two-dimensional table is a list of one-dimensional ones, each of which does not start with `'*table*`, but rather the first key.
So, the first key points to the correct one-dimensional table and the second key points to the correct value within it, as we have already seen.
The two-dimensional table overall, still starts with `'*table*`.

The `lookup-2d` procedure searches for the first key to find the correct subtable. If found, the second key is searched in the
subtable to get its respective value. Notice how the `assoc` and `lookup` procedures, from the one-dimensional table, are reused.

The `insert-2d!` searches for the first key to find the correct subtable. If it does, it uses the rules of `insert!` to either
update or create a key-value pair. If the first key is not found, a new one-dimensional table is created and put at the start
of the two-dimensional one, just after `'*table*`.

Our code will only access the table via just two helper methods, `put` and `get`, that create and access records in a specific `dispatch-table`.

If you want a deeper insight or explanation about tables, have a look at
[SICP](https://mitp-content-server.mit.edu/books/content/sectbyfn/books_pres_0/6515/sicp.zip/full-text/book/book-Z-H-22.html#%_sec_3.3.3).
But the essence of this section is what follows. You can take the table facility for granted and come back later if you want.

### Helpers
We define some helper procedures that enable us to work with tagged objects. We intend to tag cats with `'cat` and dogs with `'dog`.

```scheme
; Tagging helpers.
(define (set-tag tag object)
  (cons tag object))

(define (get-tag object)
  (car object))

(define (contents object)
  (cdr object))
```

## User Packages
Now, we have everything we need to tackle with the actual problem and see data-directed programming in action.

### Cat Package
We define a procedure that installs a cat package, when called. It defines procedures with functionality specific to cats
and registers them in the table. The first key is the operation's name, e.g. `sound` or `likes` and the second key is `'cat`.
The value associated with both keys is the respective inner procedure, the right code to call for the cat's operation.

```scheme
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
  ; Return a user procedure that creates a cat and attaches the appropriate tag to it.
  (lambda (name age) (set-tag type (make name age))))
```

Note that `install-cat-package` returns a cat's constructor, which applies the tag `'cat` to the created object.

### Dog Package
Similarly, we define a procedure that installs a dog package, when called. The operation names, used as the first key, are the same
with the ones we used for cats. But this time, the second key is `'dog` and the values are procedures with functionality specific to dogs.

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
  (define (likes) "meat")
  (define (kind) "dog")

  ; Install the procedure-type bindings in the table.
  (put 'sound type sound)
  (put 'likes type likes)
  (put 'kind type kind)
  (put 'get-name type get-name)
  (put 'get-age type get-age)
  ; Return a user procedure that creates a dog and attaches the appropriate tag to it.
  (lambda (name age) (set-tag type (make name age))))
```

> Note that package authors only need to adhere to a couple of common rules. They need to use the same keys for the operation
names and the respective procedures must be compatible in terms of their parameters and return values.
That way, reusable code can be written as we will see below.

## Package Interface Helpers
Let's define some helper procedures that will allow client code to easily interface with possible user packages.
They will simplify the packages' usage and mask implementation details, like the use of quote symbols.

```scheme
; Package interface helpers.
(define (pet-operation pet operation)
  ; Retrieve the right procedure based on the pet's type.
  (let ((proc (get operation (get-tag pet))))
    (if proc
        proc
        (error "Unknown operation."))))

(define (sound pet)
  ((pet-operation pet 'sound)))

(define (likes pet)
  ((pet-operation pet 'likes)))

(define (kind pet)
  ((pet-operation pet 'kind)))

(define (get-name pet)
  ((pet-operation pet 'get-name) (contents pet)))

(define (get-age pet)
  ((pet-operation pet 'get-age) (contents pet)))
```

Notice that they retrieve the correct procedure from the table and call it.

## Reusable Code
Now let's see a sample code that can be reusable, no matter what pets we have. The following procedure presents a pet, by writing
information about it to the standard output. When we call `(sound pet)` or `(likes pet)`, different procedures are called,
based on the runtime type of the parameter `pet`.

```scheme
; Code that does not need to change no matter how many user packages are installed.
(define (present pet)
  (display (+ "My pet's name is " (get-name pet) ". "))
  (display (+ "It's a " (kind pet) ", "))
  (display (+ (number-to-string (get-age pet) "") " years of age, "))
  (display (+ "it says \"" (sound pet) "\" "))
  (display (+ "and likes " (likes pet) ".\n\n")))
```

## Package Installation And Execution
So, it's time to introduce my pets to you!

```scheme
(define make-cat (install-cat-package))
(define make-dog (install-dog-package))

; Present my two pets to the world.
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

; User packages.
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
  ; Return a user procedure that creates a cat and attaches the appropriate tag to it.
  (lambda (name age) (set-tag type (make name age))))

; Dog type.
(define (install-dog-package)
  (define type 'dog)
  ; constructor and getters.
  (define (make name age) (cons name age))
  (define (get-name dog) (car dog))
  (define (get-age dog) (cdr dog))
  ; dog specific operations.
  (define (sound) "woof")
  (define (likes) "meat")
  (define (kind) "dog")

  ; Install the procedure-type bindings in the table.
  (put 'sound type sound)
  (put 'likes type likes)
  (put 'kind type kind)
  (put 'get-name type get-name)
  (put 'get-age type get-age)
  ; Return a user procedure that creates a dog and attaches the appropriate tag to it.
  (lambda (name age) (set-tag type (make name age))))

; Package interface helpers.
(define (pet-operation pet operation)
  ; Retrieve the right procedure based on the pet's type.
  (let ((proc (get operation (get-tag pet))))
    (if proc
        proc
        (error "Unknown operation."))))

(define (sound pet)
  ((pet-operation pet 'sound)))

(define (likes pet)
  ((pet-operation pet 'likes)))

(define (kind pet)
  ((pet-operation pet 'kind)))

(define (get-name pet)
  ((pet-operation pet 'get-name) (contents pet)))

(define (get-age pet)
  ((pet-operation pet 'get-age) (contents pet)))

; Code that does not need to change no matter how many user packages are installed.
(define (present pet)
  (display (+ "My pet's name is " (get-name pet) ". "))
  (display (+ "It's a " (kind pet) ", "))
  (display (+ (number-to-string (get-age pet) "") " years of age, "))
  (display (+ "it says \"" (sound pet) "\" "))
  (display (+ "and likes " (likes pet) ".\n\n")))

(define make-cat (install-cat-package))
(define make-dog (install-dog-package))

; Present my two pets to the world.
(define my-cat (make-cat "Ruby" 8))
(define my-dog (make-dog "August" 16))

(present my-cat)
(present my-dog)
```
->
```
My pet's name is Ruby. It's a cat, 8 years of age, it says "meow" and likes fish.

My pet's name is August. It's a dog, 16 years of age, it says "woof" and likes meat.
```

Let's see another interesting capability, [metaprogramming](metaprogramming.md).
