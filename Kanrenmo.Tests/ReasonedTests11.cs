using System;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Kanrenmo.Context;

namespace Kanrenmo.Tests
{
    /// <summary>
    /// Tests from Reasoned Schemer
    /// </summary>
    /// <remarks>
    /// See original tests at https://github.com/miniKanren/simple-miniKanren/blob/master/mktests.scm
    /// </remarks>
    public class ReasonedTests11
    {
/*
        (test-check "testc11.tex-1" 
        (run* (q)
          fail)

        `())

*/
        [Fact]
        public void Test11_1()
        {
            Assert.Empty(Run(_fail, _q));
        }


/*
        (test-check "testc11.tex-2"   
        (run* (q)
          (== #t q))

        `(#t))
*/
        [Fact]
        public void Test11_2()
        {
            AssertSingleBound(true, Run(_q == true, _q), _q);
        }

/*
        (test-check "testc11.tex-3"   
        (run* (q) 
          fail
          (== #t q))

        `())

        (test-check "testc11.tex-8"   
        (run* (r)
          fail
          (== 'corn r))

        `())
*/
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData("corn")]
        [InlineData(42)]
        public void Test11_3_8<T>(T value)
        {
            Assert.Empty(Run(_fail & _q == (ValueVar<T>)value, _q));
        }

/*
        (test-check "testc11.tex-5"   
        (run* (q) 
          succeed 
          (== #t q))

        `(#t))

        (test-check "testc11.tex-7"   
        (run* (r) 
          succeed
          (== 'corn r))

        `(corn))


        (test-check "testc11.tex-9"   
        (run* (q) 
          succeed 
          (== #f q))

        `(#f))
*/
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData("corn")]
        [InlineData(42)]
        public void Test11_5_7_9<T>(T value)
        {
            AssertSingleBound(value, Run(_succeed & _q == (ValueVar<T>)value, _q), _q);
        }

/*
        (test-check "testc11.tex-11" 
        (run* (q)
          (fresh (x)
            (== #t x)
            (== #t q)))

        (list #t))
*/
        [Fact]
        public void Test11_11()
        {
            AssertSingleBound(true, Run(Fresh(true == _x & true == _q, _x), _q), _q);
        }

/*
        (test-check "testc11.tex-12" 
        (run* (q)
          (fresh (x)
            (== x #t)
            (== #t q)))

        (list #t))
*/
        [Fact]
        public void Test11_12()
        {
            AssertSingleBound(true, Run(Fresh(_x == true & true == _q, _x), _q), _q);
        }

/*

        (test-check "testc11.tex-13" 
        (run* (q)
          (fresh (x)
            (== x #t)
            (== q #t)))

        (list #t))
*/
        [Fact]
        public void Test11_13()
        {
            AssertSingleBound(true, Run(Fresh(_x == true & _q == true, _x), _q), _q);
        }

/*

        (test-check "testc11.tex-14"   
        (run* (x)
          succeed)

        (list `_.0))
*/
        [Fact]
        public void Test11_14()
        {
            AssertSingleUnbound(Run(_succeed, _x), _x);
        }

/*
        (test-check "testc11.tex-15"   
        (run* (x)
          (let ((x #f))
            (fresh (x)
              (== #t x))))

        `(_.0))
*/
        [Fact()]
        public void Test11_15()
        {
            AssertSingleUnbound(Run(Invoke(x => Fresh(true == x, x), false), _x), _x);
        }

/*
        (test-check "testc11.tex-16" 
        (run* (r)
          (fresh (x y)
            (== (cons x (cons y '())) r)))

        (list `(_.0 _.1)))
*/
        [Fact(Skip = "TODO")]
        public void Test11_16()
        {
            // TODO 
        }

/*

        (test-check "testc11.tex-17" 
        (run* (s)
          (fresh (t u)
            (== (cons t (cons u '())) s)))

        (list `(_.0 _.1)))
*/
        [Fact(Skip = "TODO")]
        public void Test11_17()
        {
            // TODO 
        }

/*

        (test-check "testc11.tex-18" 
        (run* (r)
          (fresh (x)
            (let ((y x))
              (fresh (x)
                (== (cons y (cons x (cons y '()))) r)))))

        (list `(_.0 _.1 _.0)))
*/
        [Fact(Skip = "TODO")]
        public void Test11_18()
        {
            // TODO 
        }

/*
        (test-check "testc11.tex-19" 
        (run* (r)
          (fresh (x)
            (let ((y x))
              (fresh (x)
                (== (cons x (cons y (cons x '()))) r)))))

        (list `(_.0 _.1 _.0)))
*/
        [Fact(Skip = "TODO")]
        public void Test11_19()
        {
            // TODO 
        }

/*

        (test-check "testc11.tex-20" 
        (run* (q) 
          (== #f q)
          (== #t q))

        `())
*/
        [Fact]
        public void Test11_20()
        {
            Assert.Empty(Run(_q == true & _q == false, _q)); 
        }

/*
        (test-check "testc11.tex-21"   
        (run* (q) 
          (== #f q)
          (== #f q))

        '(#f))
*/

        [Fact]
        public void Test11_21()
        {
            AssertSingleBound(false, Run(_q == false & _q == false, _q), _q);
        }

/*
        (test-check "testc11.tex-22" 
        (run* (q)
          (let ((x q))
            (== #t x)))

        (list #t))
*/
        [Fact]
        public void Test11_22()
        {
            AssertSingleBound(true, Run(Invoke(x => x == true, _q), _q), _q);
        }

/*
        (test-check "testc11.tex-23" 
        (run* (r)
          (fresh (x)
            (== x r)))

        (list `_.0))
*/
        [Fact]
        public void Test11_23()
        {
            AssertSingleUnbound(Run(Fresh(_x == _r, _x), _r), _r);
        }



/*        
        (test-check "testc11.tex-24" 
        (run* (q)
          (fresh (x)
            (== #t x)
            (== x q)))

        (list #t))
*/

        [Fact]
        public void Test11_24()
        {
            AssertSingleBound(true, Run(Fresh(true == _x & _x == _q, _x), _q), _q);
        }

/*
        (test-check "testc11.tex-25" 
        (run* (q)
            (fresh (x)
            (== x q)
            (== #t x)))

        (list #t))
*/
        [Fact]
        public void Test11_25()
        {
            AssertSingleBound(true, Run(Fresh(_x == _q & true == _x, _x), _q), _q);
        }

/*
        (test-check "testc11.tex-26" 
        (run* (q)
            (fresh (x)
            (== (eq? x q) q)))


            (list #f))
*/
        [Fact]
        public void Test11_26()
        {
            AssertSingleBound(false, Run(Fresh(_q.Equals(_x) == _q, _x), _q), _q);
        }
        
/*

        (test-check "testc11.tex-27" 
        (run* (q)
            (let ((x q))
            (fresh (q)
                (== (eq? x q) x))))

        (list #f))
*/
        [Fact]
        public void Test11_27()
        {
            AssertSingleBound(
                false,
                Run(Invoke(x => Fresh(x == Equals(x, _q), _q), _q),
                    _q),
                _q);
        }



        
/*
    (test-check "testc13.tex-fail1" (run* (q)

        (conde
            (fail succeed)
            (succeed fail)) 


            ) '())

*/

        [Fact]
        void Test13_fail1()
        {
            Assert.Empty(Run(_fail &_succeed | _succeed & _fail, _q));
        }

/*
            (test-check "testc13.tex-succeed1" (not (null? (run* (q)

        (conde
            (fail fail)
            (succeed succeed))


            ))) #t)

*/
        [Fact]
        void Test13_succeed1()
        {
            AssertSingleUnbound(Run(_fail & _fail | _succeed & _succeed, _q), _q);
        }
/*
            (test-check "testc13.tex-succeed2" (not (null? (run* (q)


        (conde
            (succeed succeed)
            (succeed fail))


            ))) #t)
*/
        [Fact]
        void Test13_succeed2()
        {
            AssertSingleUnbound(Run(_succeed & _succeed | _succeed & _fail, _q), _q);
        }

/*
        (test-check "testc11.tex-30" 
        (run* (x)
            (conde
            ((== 'olive x) succeed)
            ((== 'oil x) succeed)))

        `(olive oil))
*/
        [Fact]
        void Test11_30()
        {
            AssertMultiple(
                new object[] {"olive", "oil"}, 
                Run(_x == "olive" & _succeed | _x == "oil" & _succeed, _x),
                _x);
        }
/*
        (test-check "testc11.tex-31" 
        (run1 (x)
            (conde
            ((== 'olive x) succeed)
            ((== 'oil x) succeed)))

        `(olive))

*/
        [Fact]
        void Test11_31()
        {
            AssertSingle(
                v => (v as ValueVar<string>)?.Value == "olive" || (v as ValueVar<string>)?.Value == "oil", 
                Run1(_x == "olive" & _succeed | _x == "oil" & _succeed, _x),
                _x);
        }
/*
        (test-check "testc11.tex-32" 
        (run* (x)
            (conde
            ((== 'virgin x) fail)
            ((== 'olive x) succeed)
            (succeed succeed)
            ((== 'oil x) succeed)))

        `(olive _.0 oil))

            (test-check "testc13.tex-conde1" (run* (x)


        (conde
            ((== 'olive x) succeed)
            (succeed succeed)
            ((== 'oil x) succeed))


            ) `(olive _.0 oil))


        (test-check "testc11.tex-33" 
        (run2 (x)
            (conde
            ((== 'extra x) succeed)
            ((== 'virgin x) fail)
            ((== 'olive x) succeed)
            ((== 'oil x) succeed)))

        `(extra olive))

        (test-check "testc11.tex-34" 
        (run* (r)
            (fresh (x y)
            (== 'split x)
            (== 'pea y)
            (== (cons x (cons y '())) r)))

        (list `(split pea)))

        (test-check "testc11.tex-35" 
        (run* (r)
            (fresh (x y)
            (conde
                ((== 'split x) (== 'pea y))
                ((== 'navy x) (== 'bean y)))
            (== (cons x (cons y '())) r)))

        `((split pea) (navy bean)))

        (test-check "testc11.tex-36" 
        (run* (r)
            (fresh (x y)
            (conde
                ((== 'split x) (== 'pea y))
                ((== 'navy x) (== 'bean y)))
            (== (cons x (cons y (cons 'soup '()))) r)))

        `((split pea soup) (navy bean soup)))

        (define teacupo
            (lambda (x)
            (conde
                ((== 'tea x) succeed)
                ((== 'cup x) succeed))))


        (test-check "testc11.tex-37"   
        (run* (x)
            (teacupo x))

        `(tea cup))

        (test-check "testc11.tex-38"   
        (run* (r)
            (fresh (x y)
            (conde
                ((teacupo x) (== #t y) succeed)
                ((== #f x) (== #t y)))
            (== (cons x (cons y '())) r)))

        `((#f #t) (tea #t) (cup #t)))

        (test-check "testc11.tex-39"   
        (run* (r)
            (fresh (x y z)                                                              
            (conde
                ((== y x) (fresh (x) (== z x)))                                         
                ((fresh (x) (== y x)) (== z x)))                                        
            (== (cons y (cons z '())) r)))

        `((_.0 _.1) (_.0 _.1)))

        (test-check "testc11.tex-40"   
        (run* (r)
            (fresh (x y z)                                                              
            (conde
                ((== y x) (fresh (x) (== z x)))                                         
                ((fresh (x) (== y x)) (== z x)))
            (== #f x)
            (== (cons y (cons z '())) r)))

        `((#f _.0) (_.0 #f)))

        (test-check "testc11.tex-41" 
        (run* (q)
            (let ((a (== #t q))
                (b (== #f q)))
            b))

        '(#f))

        (test-check "testc11.tex-42" 
        (run* (q)
            (let ((a (== #t q))
                (b (fresh (x)
                        (== x q)
                        (== #f x)))
                (c (conde
                        ((== #t q) succeed)
                        (succeed (== #f q)))))
            b))

        '(#f))

*/

        private IEnumerable<IReadOnlyDictionary<Var, Var>> Run1(Relation relation, params Var[] variables)
        {
            var d = Run(relation, variables).FirstOrDefault();
            if (d != null)
            {
                yield return d;
            }
        }
            

        private Func<T, TRet> Lambda<T, TRet>(Func<T, TRet> lambda) => lambda;

        [AssertionMethod]
        private void AssertSingle(Predicate<Var> check, IEnumerable<IReadOnlyDictionary<Var, Var>> results, Var v)
        {
            var list = results.ToList();
            Assert.Single(list);
            Assert.Single(list[0]);
            Assert.True(list[0].TryGetValue(v, out var r));
            Assert.True(check(r));
        }

        [AssertionMethod]
        private void AssertSingleBound<T>(T value, IEnumerable<IReadOnlyDictionary<Var, Var>> results, Var v)
        {
            var list = results.ToList();
            Assert.Single(list);
            Assert.Single(list[0]);
            Assert.True(list[0].TryGetValue(v, out var r));
            Assert.IsType<ValueVar<T>>(r);
            Assert.Equal(value, ((ValueVar<T>)r).Value);
        }

        [AssertionMethod]
        private void AssertSingleUnbound(IEnumerable<IReadOnlyDictionary<Var, Var>> results, Var v)
        {
            var list = results.ToList();
            Assert.Single(list);
            Assert.Single(list[0]);
            Assert.True(list[0].TryGetValue(v, out var r));
            Assert.False(r.Bound);
        }

        [AssertionMethod]
        private void AssertMultiple(object[] values, IEnumerable<IReadOnlyDictionary<Var, Var>> results, Var v)
        {
            var list = results.ToList();
            Assert.Equal(values.Length, list.Count);
            Assert.All(list, d => Assert.Single(d));
            foreach (var value in values)
            {
                Assert.Contains(
                    list,
                    d => d.TryGetValue(v, out var x)
                         && (value == null && !x.Bound
                             || Equals((x as ValueVar)?.UntypedValue, value)));
            }
        }

        private readonly Relation _fail = (Var) true == false;
        private readonly Relation _succeed = (Var) false == false;
        private readonly Var _q = new Var();
        private readonly Var _x = new Var();
        private readonly Var _r = new Var();
    }
}
