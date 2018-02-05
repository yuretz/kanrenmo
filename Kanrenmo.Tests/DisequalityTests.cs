using System.Collections.Generic;
using JetBrains.Annotations;
using Xunit;
using static Kanrenmo.Context;
using System.Linq;

namespace Kanrenmo.Tests
{
    /// <summary>
    /// Disequality tests from cKanren
    /// </summary>
    /// <remarks>
    /// See original racket tests at https://github.com/calvis/cKanren/blob/master/cKanren/tests/neq.rkt 
    /// Copyright (C) 2013-4 Claire Alvis
    /// Copyright (C) 2011-2013 Daniel P. Friedman, Oleg Kiselyov, Claire E. Alvis, Jeremiah J. Willcock, Kyle M. Carter, William E. Byrd
    /// </remarks>
    public class DisequalityTests
    {
/*
        (test (run* (q) (=/= 5 6)) '(_.0))
*/
        [Fact]
        public void Simple1()
        {
            AssertOneUnbound(Solve(q => Var(5) != Var(6)));
            Assert.Equal("(_0)", ToSExpression(Solve(q => Var(5) != Var(6))));
        }

/*
        (test (run* (q) (=/= 3 3)) '())
*/
        [Fact]
        public void Simple2()
        {
            // ReSharper disable EqualExpressionComparison
            Assert.Empty(Solve(q => Var(3) != Var(3)));
            Assert.Equal("()", ToSExpression(Solve(q => Var(3) != Var(3))));
            // ReSharper enable EqualExpressionComparison
        }
/*        
        (test (run* (q) (== q 3) (=/= 3 q))
            '())
*/
        [Fact]
        public void Simple3()
        {
            Assert.Equal("()", ToSExpression(Solve(q => q == 3 & 3 != q)));
        }
/*
        (test (run* (q) (=/= 3 q) (== q 3))
            '())
*/
        [Fact]
        public void Simple4()
        {
            Assert.Equal("()", ToSExpression(Solve(q => 3 != q & q == 3)));
        }

/*
        (test (run* (x y) (== x y) (=/= x y))
            '())
*/
        [Fact]
        public void Simple5()
        {
            Assert.Equal("()", ToSExpression(Solve((x, y) => x == y & x != y)));
        }

/*
        (test (run* (x y) (=/= x y) (== x y))
            '())
*/
        [Fact]
        public void Simple6()
        {
            Assert.Equal("()", ToSExpression(Solve((x, y) => x != y & x == y )));
        }

/*
        (test (run* (q) (=/= q q))
            '())
*/
        [Fact]
        public void Simple7()
        {
            #pragma warning disable CS1718 // Comparison made to same variable
            Assert.Equal("()", ToSExpression(Solve(q => q != q)));
            #pragma warning restore CS1718 // Comparison made to same variable
        }


/*
        (test (run* (q) (fresh (a) (=/= a a)))
            '())
*/
        [Fact]
        public void Simple8()
        {
            #pragma warning disable CS1718 // Comparison made to same variable
            Assert.Equal("()", ToSExpression(Solve(q => Declare(a => a != a))));
            #pragma warning restore CS1718 // Comparison made to same variable
        }

/*
        (test
        (run* (x y)
            (=/= x y)
            (== 3 x)
            (== 3 y))
        '())
*/
        [Fact]
        public void Simple9()
        {
            Assert.Equal("()", ToSExpression(Solve((x, y) => x != y & 3 == x & 3 == y)));
        }

/*
        (test
        (run* (x y)
            (== 3 x)
            (=/= x y)
            (== 3 y))
        '())
*/
        [Fact]
        public void Simple10()
        {
            Assert.Equal("()", ToSExpression(Solve((x, y) => 3 == x & x != y & 3 == y)));
        }

/*
        (test
        (run* (x y)
            (== 3 x)
            (== 3 y)
            (=/= x y))
        '())
*/
        [Fact]
        public void Simple11()
        {
            Assert.Equal("()", ToSExpression(Solve((x, y) => 3 == x & 3 == y & x != y)));
        }
/*
        (test
        (run* (x y)
            (== 3 x)
            (== 3 y)
            (=/= y x))
        '())
*/
        [Fact]
        public void Simple12()
        {
            Assert.Equal("()", ToSExpression(Solve((x, y) => 3 == x & 3 == y & y != x)));
        }

/*
  
        (test
        (run* (x y z)
            (== x y)
            (== y z)
            (=/= x 4)
            (== z (+ 2 2)))
        '())
  
        (test
        (run* (x y z)
            (== x y)
            (== y z)
            (== z (+ 2 2))
            (=/= x 4))
        '())
  
        (test
        (run* (x y z)
            (=/= x 4)
            (== y z)
            (== x y)
            (== z (+ 2 2)))
        '())
  
        (test
        (run* (x y z)
            (=/= x y)
            (== x `(0 ,z 1))
            (== y `(0 1 1))
            (== z 1))
        '())
  
        (test
        (run* (x y z)
            (== z 1)
            (=/= x y)
            (== x `(0 ,z 1))
            (== y `(0 1 1)))
        '())
  
        (test
        (run* (x y z)
            (== z 1)
            (== x `(0 ,z 1))
            (== y `(0 1 1))
            (=/= x y))
        '())
  
        (test
        (run* (q)
            (fresh (x y z)
            (=/= x y)
            (== x `(0 ,z 1))
            (== y `(0 1 1))
            (== z 0)))
        '(_.0))
  
        (test
        (run* (x y)
            (=/= `(,x 1) `(2 ,y))
            (== x 2)
            (== y 1))
        '())

        (test
        (run* (a x z)
            (=/= a `(,x 1))
            (== a `(,z 1))
            (== x z))
        '())

        (test
        (run* (x y)
            (=/= `(,x 1) `(2 ,y))
            (== x 2)
            (== y 1))
        '())
  
        (test
        (run* (q)
            (fresh (x y z)
            (== z 0)
            (=/= x y)
            (== x `(0 ,z 1))
            (== y `(0 1 1))))
        '(_.0))
  
        (test
        (run* (q)
            (fresh (x y z)
            (== x `(0 ,z 1))
            (== y `(0 1 1))
            (=/= x y)))
        '(_.0))
  
        (test
        (run* (q)
            (fresh (x y)
            (=/= `(,x 1) `(2 ,y))
            (== x 2)))
        '(_.0))
  
        (test
        (run* (q)
            (fresh (x y)
            (=/= `(,x 1) `(2 ,y))
            (== y 1)))
        '(_.0))
  
        (test
        (run* (x y z)
            (=/= `(,x 2 ,z) `(1 ,z 3))
            (=/= `(,x 6 ,z) `(4 ,z 6))
            (=/= `(,x ,y ,z) `(7 ,z 9))
            (== x z))
        '((_.0 _.1 _.0)))
  
        (test
        (run* (x y)
            (=/= `(,x 1) `(2 ,y))
            (== x 2)
            (== y 9))
        '((2 9)))
  
        (test
        (run* (q)
            (fresh (a)
            (== 3 a)
            (=/= a 4)))
        '(_.0))

        ;; MEDIUM

        ;; these test reification
        (test
        (run* (q) (=/= q #f))
        '((_.0 : (=/= ((_.0 . #f))))))
  
        (test
        (run* (x y) (=/= x y))
        '(((_.0 _.1) : (=/= ((_.0 . _.1))))))
  
        ;; this tests the constraint-interaction
        (test
        (run* (q) 
            (=/= q 5)
            (=/= 5 q))
        '((_.0 : (=/= ((_.0 . 5))))))

        (test
        (run* (x y)
            (=/= y x))
        '(((_.0 _.1) : (=/= ((_.0 . _.1))))))

        (test
        (run* (x y)
            (=/= x y)
            (=/= x y))
        '(((_.0 _.1) : (=/= ((_.0 . _.1))))))
  
        (test
        (run* (x y)
            (=/= x y)
            (=/= y x))
        '(((_.0 _.1) : (=/= ((_.0 . _.1)))))) 
  
        (test
        (run* (x y)
            (=/= `(,x 1) `(2 ,y)))
        '(((_.0 _.1) : (=/= ((_.0 . 2) (_.1 . 1))))))
  
        (test
        (run* (q)
            (=/= 4 q)
            (=/= 3 q))
        '((_.0 : (=/= ((_.0 . 3)) ((_.0 . 4))))))

        (test
        (run* (q) (=/= q 5) (=/= q 5))
        '((_.0 : (=/= ((_.0 . 5))))))

        ;; HARD
  
        (test
        (run* (x y)
            (=/= `(,x 1) `(2 ,y))
            (== x 2))
        '(((2 _.0) : (=/= ((_.0 . 1))))))
  
        (test
        (run* (q)
            (fresh (a x z)
            (=/= a `(,x 1))
            (== a `(,z 1))
            (== x 5)
            (== `(,x ,z) q)))
        '(((5 _.0) : (=/= ((_.0 . 5))))))
  
        (test
        (run* (x y)
            (=/= `(,x ,y) `(5 6))
            (=/= x 5))
        '(((_.0 _.1) : (=/= ((_.0 . 5))))))
  
        (test
        (run* (x y)
            (=/= x 5)
            (=/= `(,x ,y) `(5 6)))
        '(((_.0 _.1) : (=/= ((_.0 . 5))))))
  
        (test
        (run* (x y)
            (=/= 5 x)
            (=/= `( ,y ,x) `(6 5)))
        '(((_.0 _.1) : (=/= ((_.0 . 5))))))
  
        (test
        (run* (x)
            (fresh (y z)
            (=/= x `(,y 2))
            (== x `(,z 2))))
        '((_.0 2)))
  
        (test
        (run* (x)
            (fresh (y z)
            (=/= x `(,y 2))
            (== x `((,z) 2))))
        '(((_.0) 2)))
  
        (test
        (run* (x)
            (fresh (y z)
            (=/= x `((,y) 2))
            (== x `(,z 2))))
        '((_.0 2)))
  
        (test
        (run* (q)
            (distincto `(2 3 ,q)))
        '((_.0 : (=/= ((_.0 . 2)) ((_.0 . 3))))))
  
        (test
        (run* (q) (rembero 'x '() q))
        '(()))

        (test
        (run* (q) (rembero 'x '(x) '()))
        '(_.0))
  
        (test
        (run* (q) (rembero 'a '(a b a c) q))
        '((b c)))
  
        (test
        (run* (q) (rembero 'a '(a b c) '(a b c)))
        '())

        (test
        (run* (w x y z)
            (=/= `(,w . ,x) `(,y . ,z)))
        '(((_.0 _.1 _.2 _.3)
            :
            (=/= ((_.0 . _.2) (_.1 . _.3))))))

        (test
        (run* (w x y z)
            (=/= `(,w . ,x) `(,y . ,z))
            (== w y))
        '(((_.0 _.1 _.0 _.2)
            :
            (=/= ((_.1 . _.2))))))
        )


*/

        [AssertionMethod]
        private void AssertOneUnbound([NotNull] IEnumerable<IReadOnlyList<Var>> results)
        {
            var list = results.ToList();
            Assert.Single(list);
            Assert.Single(list[0]);
            var r = list[0][0];
            Assert.False(r.Bound);
        }
    }
}
