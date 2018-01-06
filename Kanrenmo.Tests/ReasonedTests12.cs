using System;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Kanrenmo.Context;

namespace Kanrenmo.Tests
{
    /// <summary>
    /// Tests from Reasoned Schemer (Chapter 2)
    /// </summary>
    /// <remarks>
    /// See original tests at https://github.com/miniKanren/simple-miniKanren/blob/master/mktests.scm
    /// </remarks>
    public class ReasonedTests12
    {

        
/*
        (test-check "testc12.tex-2" 
        (run* (r)
          (fresh (y x)
            (== `(,x ,y) r)))

        (list `(_.0 _.1)))
*/

        [Fact]
        public void Test12_2()
        {
            AssertOneBound(
                CheckList(indices: new int?[] {0, 1}),
                Run(r => Fresh((y, x) => new[] { x, y } == r)));
        }


/*

        (test-check "testc12.tex-3" 
        (run* (r)
          (fresh (v w)
            (== (let ((x v) (y w)) `(,x ,y)) r)))

        `((_.0 _.1)))
*/

        [Fact]
        public void Test12_3()
        {
            AssertOneBound(
                CheckList(indices: new int?[] { 0, 1 }),
                Run(r => Fresh((v, w) => ((Func<Var, Var, Var>)((x, y) => new [] {x, y}))(v, w) == r)));
        }
/*

        (test-check "testc12.tex-4"   
        (car `(grape raisin pear))

        `grape)
*/

        [Fact]
        public void Test12_4()
        {
            Assert.Equal("grape", Seq("grape", "rasin", "pear").Head()); 
        }

/*
        (test-check "testc12.tex-5" 
        (car `(a c o r n))

        'a)
*/
        [Fact]
        public void Test12_5()
        {
            Assert.Equal('a', Seq('a', 'c', 'o', 'r', 'n').Head()); 
        }

/*
        (define caro
          (lambda (p a)
            (fresh (d)
              (== (cons a d) p))))


        (test-check "testc12.tex-6" 
        (run* (r)
          (caro `(a c o r n) r))

        (list 'a))
*/

        [Fact]
        public void Test12_6()
        {
            AssertOneBound('a', Run(r => Seq('a', 'c', 'o', 'r', 'n').Heado(r)));
        }


/*
        (test-check "testc12.tex-8"   
        (run* (q) 
          (caro `(a c o r n) 'a)
          (== #t q))

        (list #t))
*/

        [Fact]
        public void Test12_8()
        {
            AssertOneBound(true, Run(q => Seq('a', 'c', 'o', 'r', 'n').Heado('a') & q == true));
        }

/*
        (test-check "testc12.tex-10" 
        (run* (r)
          (fresh (x y)
            (caro `(,r ,y) x)
            (== 'pear x)))

        (list 'pear))
*/
        [Fact]
        public void Test12_10()
        {
            AssertOneBound("pear", Run(r => Fresh((x, y) => Seq(r, y).Heado(x) & x == "pear")));
        }

/*
        (test-check "testc12.tex-11"   
        (cons 
          (car `(grape raisin pear))
          (car `((a) (b) (c))))

        `(grape a))
*/
        [Fact]
        public void Test12_11()
        {
            Assert.Equal(
                new Var[] {"grape", 'a'},
                Seq("grape", "raisin", "pear")
                    .Head()
                    .Cons(Seq(Seq('a'), Seq('b'), Seq('c')).Head()));
        }

/*

        (test-check "testc12.tex-12" 
        (run* (r)
          (fresh (x y)
            (caro `(grape raisin pear) x)
            (caro `((a) (b) (c)) y)
            (== (cons x y) r)))

        (list `(grape a)))
*/
        [Fact]
        void Test12_12()
        {
            AssertOneBound(
                CheckList(new object[] { "grape", 'a'}),
                Run(r => Fresh((x, y) => 
                    Seq("grape", "raisin", "pear").Heado(x) 
                    & Seq(Seq('a'), Seq('b'), Seq('c')).Heado(y)
                    & x.Cons(y) == r)));
        }

/*
        (test-check "testc12.tex-13"   
        (cdr `(grape raisin pear))

        `(raisin pear))
*/
        [Fact]
        public void Test12_13()
        {
            Assert.Equal(new Var[] {"raisin", "pear"}, Seq("grape", "raisin", "pear").Tail());
        }


/*
        (test-check "testc12.tex-14"   
        (car (cdr `(a c o r n)))

        'c)
*/
        [Fact]
        public void Test12_14()
        {
            Assert.Equal('c', Seq('a', 'c', 'o', 'r', 'n').Tail().Head());
        }
/*

        (define cdro
          (lambda (p d)
            (fresh (a)
              (== (cons a d) p))))


        (test-check "testc12.tex-15" 
        (run* (r)
          (fresh (v)
            (cdro `(a c o r n) v)
            (caro v r)))

        (list 'c))
*/
        [Fact]
        public void Test12_15()
        {
            AssertOneBound('c', Run(r => Fresh(v => Seq('a', 'c', 'o', 'r', 'n').Tailo(v) & v.Heado(r))));
        }

/*
        (test-check "testc12.tex-16"   
        (cons 
          (cdr `(grape raisin pear))
          (car `((a) (b) (c))))

        `((raisin pear) a))
*/
        [Fact]
        public void Test12_16()
        {
            Assert.Equal(
                Seq(Seq("raisin", "pear"), 'a'), 
                Seq("grape", "raisin", "pear").Tail().Cons(Seq(Seq('a'), Seq('b'), Seq('c')).Head()));
        }


/*
        (test-check "testc12.tex-17" 
        (run* (r)
          (fresh (x y)
            (cdro `(grape raisin pear) x)
            (caro `((a) (b) (c)) y)
            (== (cons x y) r)))

        (list `((raisin pear) a)))
*/
        [Fact]
        public void Test12_17()
        {
            AssertOneBound(
                CheckList(new object[] {CheckList(new [] {"raisin", "pear"}), 'a'}),
                Run(r => 
                    Fresh((x, y) => 
                        Seq("grape", "raisin", "pear").Tailo(x)
                        & Seq(Seq('a'), Seq('b'), Seq('c')).Heado(y)
                        & x.Cons(y) == r)));
        }

/*
        (test-check "testc12.tex-18"   
        (run* (q) 
          (cdro '(a c o r n) '(c o r n)) 
          (== #t q))

        (list #t))

        (test-check "testc12.tex-19" `(c o r n)

           (cdr 

        '(a c o r n)

           ))


        (test-check "testc12.tex-20" 
        (run* (x)
          (cdro '(c o r n) `(,x r n)))

        (list 'o))

        (test-check "testc12.tex-21" `(o r n)

           (cdr 

        `(c o r n)

           ))


        (test-check "testc12.tex-22" 
        (run* (l)
          (fresh (x) 
            (cdro l '(c o r n))
            (caro l x)
            (== 'a x)))

        (list `(a c o r n)))


        (define conso
          (lambda (a d p)
            (== (cons a d) p)))


        (test-check "testc12.tex-23" 
        (run* (l)
          (conso '(a b c) '(d e) l))

        (list `((a b c) d e)))

        (test-check "testc12.tex-24" 
        (run* (x)
          (conso x '(a b c) '(d a b c)))

        (list 'd))

        (test-check "testc12.tex-25" (cons 'd '(a b c))
        `(d a b c))

        (test-check "testc12.tex-26" 
        (run* (r)
          (fresh (x y z)
            (== `(e a d ,x) r)
            (conso y `(a ,z c) r)))

        (list `(e a d c)))

        (test-check "testc12.tex-27" 
        (run* (x)
          (conso x `(a ,x c) `(d a ,x c)))

        (list 'd))

                 (define x 'd)


        (test-check "testc12.tex-28" (cons x `(a ,x c))
        `(d a ,x c))

        (test-check "testc12.tex-29" 
        (run* (l)
          (fresh (x)
            (== `(d a ,x c) l)
            (conso x `(a ,x c) l)))

        (list `(d a d c)))

        (test-check "testc12.tex-30" 
        (run* (l)
          (fresh (x)
            (conso x `(a ,x c) l)
            (== `(d a ,x c) l)))

        (list `(d a d c)))


        (test-check "testc12.tex-31" 
        (run* (l)
          (fresh (d x y w s)
            (conso w '(a n s) s)
            (cdro l s)
            (caro l x)
            (== 'b x)
            (cdro l d)
            (caro d y)
            (== 'e y)))

        (list `(b e a n s)))

        (test-check "testc12.tex-32"   
        (null? `(grape raisin pear))

        #f)

        (test-check "testc12.tex-33"   
        (null? '())

        #t)


        (define nullo
          (lambda (x)
            (== '() x)))


        (test-check "testc12.tex-34" 
        (run* (q)
          (nullo `(grape raisin pear))
          (== #t q))

        `())

        (test-check "testc12.tex-35" 
        (run* (q)
          (nullo '())
          (== #t q))

        `(#t))

        (test-check "testc12.tex-36"   
        (run* (x) 
          (nullo x))

        `(()))


        (test-check "testc12.tex-37" 
        (eq? 'pear 'plum)

        #f)

        (test-check "testc12.tex-38"   
        (eq? 'plum 'plum)

        #t)


        (define eqo
          (lambda (x y)
            (== x y)))


        (test-check "testc12.tex-39" 
        (run* (q)
          (eqo 'pear 'plum)
          (== #t q))

        `())

        (test-check "testc12.tex-40" 
        (run* (q)
          (eqo 'plum 'plum)
          (== #t q))

        `(#t))


        (test-check "testc12.tex-41"   
        (pair? `((split) . pea))

        #t)

        (test-check "testc12.tex-42"   
        (pair? '())

        #f)

        (test-check "testc12.tex-43" 
        (car `(pear))

        `pear)

        (test-check "testc12.tex-44" 
        (cdr `(pear))

        `())

        (test-check "testc12.tex-45"   
        (cons `(split) 'pea)

        `((split) . pea))

        (test-check "testc12.tex-46"   
        (run* (r) 
          (fresh (x y)
            (== (cons x (cons y 'salad)) r)))

        (list `(_.0 _.1 . salad)))

        (define pairo
          (lambda (p)
            (fresh (a d)
              (conso a d p))))


        (test-check "testc12.tex-47" 
        (run* (q)
          (pairo (cons q q))
          (== #t q))

        `(#t))

        (test-check "testc12.tex-48" 
        (run* (q)
          (pairo '())
          (== #t q))

        `())

        (test-check "testc12.tex-49" 
        (run* (q)
          (pairo 'pair)
          (== #t q))

        `())

        (test-check "testc12.tex-50"   
        (run* (x) 
          (pairo x))

        (list `(_.0 . _.1)))

        (test-check "testc12.tex-51"   
        (run* (r) 
          (pairo (cons r 'pear)))

        (list `_.0))



*/


        private Predicate<Var> CheckList(object[] values = null, int?[] indices = null) => variable =>
        {
            if (!(variable is SequenceVar sequence))
            {
                return false;
            }

            var list = sequence.ToList();

            if (values != null)
            {
                if (list.Count < values.Length)
                {
                    return false;
                }

                for (int i = 0; i < values.Length; i++)
                {
                    if (values[i] == null)
                    {
                        continue;
                    }

                    if (values[i] is Predicate<Var> predicate)
                    {
                        Assert.True(predicate(list[i]));
                        continue;
                    }

                    var item = list[i] as ValueVar;
                    if (!Equals(item?.UntypedValue, values[i]))
                    {
                        return false;
                    }
                }
            }

            if (indices != null)
            {
                if (list.Count < indices.Length)
                {
                    return false;
                }

                for (int i = 0; i < indices.Length; i++)
                {
                    if (indices[i] == null)
                    {
                        continue;
                    }

                    if (!Equals(list[indices[i].Value], list[i]))
                    {
                        return false;
                    }
                }
            }

            return true;
        };

        [AssertionMethod]
        private void AssertOneBound(object value, IEnumerable<IReadOnlyList<Var>> results)
        {
            var list = results.ToList();
            Assert.Single(list);
            Assert.Single(list[0]);
            var r = list[0][0];

            if (value is Predicate<Var> predicate)
            {
                Assert.True(predicate(r));
            }
            else if (r is ValueVar v)
            {
                Assert.Equal(value, v.UntypedValue);
            }
            else
            {
                Assert.True(false, "Result is not a value");
            }
        }

        [AssertionMethod]
        private void AssertOneUnbound(IEnumerable<IReadOnlyList<Var>> results)
        {
            var list = results.ToList();
            Assert.Single(list);
            Assert.Single(list[0]);
            var r = list[0][0];
            Assert.False(r.Bound);
        }
    }
}
