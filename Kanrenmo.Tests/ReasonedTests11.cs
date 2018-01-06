using System;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Kanrenmo.Context;

namespace Kanrenmo.Tests
{
    /// <summary>
    /// Tests from Reasoned Schemer (Chapter 1)
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
            Assert.Empty(Run(Fail, _q));
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
            AssertOneBound(true, Run(_q == true, _q), _q);
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
            Assert.Empty(Run(Fail & _q == (ValueVar<T>)value, _q));
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
            AssertOneBound(value, Run(Succeed & _q == (ValueVar<T>)value, _q), _q);
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
            AssertOneBound(true, Run(Fresh(true == _x & true == _q, _x), _q), _q);
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
            AssertOneBound(true, Run(Fresh(_x == true & true == _q, _x), _q), _q);
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
            AssertOneBound(true, Run(Fresh(_x == true & _q == true, _x), _q), _q);
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
            AssertOneUnbound(Run(Succeed, _x), _x);
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
            AssertOneUnbound(Run(Invoke(x => Fresh(true == x, x), false), _x), _x);
        }

/*
        (test-check "testc11.tex-16" 
        (run* (r)
          (fresh (x y)
            (== (cons x (cons y '())) r)))

        (list `(_.0 _.1)))
*/
        [Fact]
        public void Test11_16()
        {
            AssertSingle(
                CheckList(null, new int?[] {0, 1}), 
                Run(Fresh(_r == new[] {_x, _y}, _x, _y), _r),
                _r);
        }

/*

        (test-check "testc11.tex-17" 
        (run* (s)
          (fresh (t u)
            (== (cons t (cons u '())) s)))

        (list `(_.0 _.1)))
*/
        [Fact]
        public void Test11_17()
        {
            AssertSingle(
                CheckList(null, new int?[] { 0, 1 }),
                Run(Fresh(_s == new[] { _t, _u }, _t, _u), _s),
                _s);
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
        [Fact]
        public void Test11_18()
        {
            AssertSingle(
                CheckList(null, new int? [] {0, 1, 0}),
                Run(Fresh(Invoke(y => Fresh(_r == new [] {y, _x, y}, _x), _x), _x), _r),
                _r);
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
        [Fact]
        public void Test11_19()
        {
            AssertSingle(
                CheckList(null, new int?[] { 0, 1, 0 }),
                Run(Fresh(Invoke(y => Fresh(_r == new[] { _x, y, _x }, _x), _x), _x), _r),
                _r);
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
            AssertOneBound(false, Run(_q == false & _q == false, _q), _q);
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
            AssertOneBound(true, Run(Invoke(x => x == true, _q), _q), _q);
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
            AssertOneUnbound(Run(Fresh(_x == _r, _x), _r), _r);
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
            AssertOneBound(true, Run(Fresh(true == _x & _x == _q, _x), _q), _q);
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
            AssertOneBound(true, Run(Fresh(_x == _q & true == _x, _x), _q), _q);
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
            AssertOneBound(false, Run(Fresh(_q.Equals(_x) == _q, _x), _q), _q);
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
            AssertOneBound(
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
        void Test13Fail1()
        {
            Assert.Empty(Run(Fail &Succeed | Succeed & Fail, _q));
        }

/*
            (test-check "testc13.tex-succeed1" (not (null? (run* (q)

        (conde
            (fail fail)
            (succeed succeed))


            ))) #t)

*/
        [Fact]
        void Test13Succeed1()
        {
            AssertOneUnbound(Run(Fail & Fail | Succeed & Succeed, _q), _q);
        }

/*
            (test-check "testc13.tex-succeed2" (not (null? (run* (q)


        (conde
            (succeed succeed)
            (succeed fail))


            ))) #t)
*/
        [Fact]
        void Test13Succeed2()
        {
            AssertOneUnbound(Run(Succeed & Succeed | Succeed & Fail, _q), _q);
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
            AssertAll(
                new object[] {"olive", "oil"}, 
                Run(_x == "olive" & Succeed | _x == "oil" & Succeed, _x),
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
                Run1(_x == "olive" & Succeed | _x == "oil" & Succeed, _x),
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
*/
        [Fact]
        void Test11_32()
        {
            AssertAll(
                new object [] {"olive", null, "oil"},
                Run(
                    _x == "virgin" & Fail 
                    | _x == "olive" & Succeed 
                    | Succeed & Succeed
                    | _x == "oil" & Succeed,
                    _x), 
                _x);          
        }

/*
            (test-check "testc13.tex-conde1" (run* (x)


        (conde
            ((== 'olive x) succeed)
            (succeed succeed)
            ((== 'oil x) succeed))


            ) `(olive _.0 oil))
*/
       [Fact]
        void Test13_conde1()
        {
            AssertAll(
                new object [] {"olive", null, "oil"},
                Run(_x == "olive" & Succeed 
                    | Succeed & Succeed
                    | _x == "oil" & Succeed,
                    _x), 
                _x);          
        }

/*
        (test-check "testc11.tex-33" 
        (run2 (x)
            (conde
            ((== 'extra x) succeed)
            ((== 'virgin x) fail)
            ((== 'olive x) succeed)
            ((== 'oil x) succeed)))

        `(extra olive))
*/
        [Fact]
        public void Test11_33()
        {
            AssertExists(
                new object[] {"extra", "olive","oil" },
                Run2(_x == "extra" & Succeed
                    | _x == "virgin" & Fail
                    | _x == "olive" & Succeed
                    | _x == "oil" & Succeed,
                    _x),
                _x, 2);
        }

/*
        (test-check "testc11.tex-34" 
        (run* (r)
            (fresh (x y)
            (== 'split x)
            (== 'pea y)
            (== (cons x (cons y '())) r)))

        (list `(split pea)))
*/
        [Fact]
        public void Test11_34()
        {
            AssertSingle(
                CheckList(new object[] {"split", "pea"}, null),
                Run(Fresh(_x == "split" & _y == "pea" & _r == new[] {_x, _y}, _x, _y), _r),
                _r);
        }

/*
        (test-check "testc11.tex-35" 
        (run* (r)
            (fresh (x y)
            (conde
                ((== 'split x) (== 'pea y))
                ((== 'navy x) (== 'bean y)))
            (== (cons x (cons y '())) r)))

        `((split pea) (navy bean)))
*/
        [Fact]
        public void Test11_35()
        {
            AssertAll(
                new object[] { CheckList(new object[] {"split", "pea"}, null),
                    CheckList(new object[] { "navy", "bean" }, null) },
                Run(
                    Fresh(
                        (_x == "split" & _y == "pea" | _x == "navy" & _y == "bean") 
                            & _r == new [] {_x, _y}, 
                        _x, _y),
                    _r),
                _r);
        }

/*
        (test-check "testc11.tex-36" 
        (run* (r)
            (fresh (x y)
            (conde
                ((== 'split x) (== 'pea y))
                ((== 'navy x) (== 'bean y)))
            (== (cons x (cons y (cons 'soup '()))) r)))

        `((split pea soup) (navy bean soup)))
*/
        [Fact]
        public void Test11_36()
        {
            AssertAll(
                new object[] { CheckList(new object[] {"split", "pea", "soup"}, null),
                    CheckList(new object[] {"navy", "bean", "soup"}, null) },
                Run(
                    Fresh(
                        (_x == "split" & _y == "pea" | _x == "navy" & _y == "bean") 
                            & _r == new [] {_x, _y, "soup"}, 
                        _x, _y),
                    _r),
                _r);
        }

/*
        (define teacupo
            (lambda (x)
            (conde
                ((== 'tea x) succeed)
                ((== 'cup x) succeed))))
*/
        public Relation TeaCupo(Var x) => x == "tea" & Succeed | x == "cup" & Succeed;

/*

        (test-check "testc11.tex-37"   
        (run* (x)
            (teacupo x))

        `(tea cup))
*/
        [Fact]
        public void Test11_37()
        {
            AssertAll(new object[] {"tea", "cup"}, Run(Invoke(TeaCupo, _x), _x), _x);
        }

 /*
        (test-check "testc11.tex-38"   
        (run* (r)
            (fresh (x y)
            (conde
                ((teacupo x) (== #t y) succeed)
                ((== #f x) (== #t y)))
            (== (cons x (cons y '())) r)))

        `((#f #t) (tea #t) (cup #t)))
 */
        [Fact]
        public void Test11_38()
        {
            AssertAll(
                new object[]
                {
                    CheckList(new object[] {false, true}, null),
                    CheckList(new object[] {"tea", true}, null),
                    CheckList(new object[] {"cup", true}, null)
                }, 
                Run(
                    Fresh(
                        (Invoke(TeaCupo, _x) & _y == true & Succeed | _x == false & _y == true)
                            & _r == new [] {_x, _y}, 
                        _x, 
                        _y),
                    _r),
                _r);
        }

/*
        (test-check "testc11.tex-39"   
        (run* (r)
            (fresh (x y z)                                                              
            (conde
                ((== y x) (fresh (x) (== z x)))                                         
                ((fresh (x) (== y x)) (== z x)))                                        
            (== (cons y (cons z '())) r)))

        `((_.0 _.1) (_.0 _.1)))
*/
        [Fact]
        public void Test11_39()
        {
            AssertAll(
                new object[]
                {
                    CheckList(null, new int?[] {0, 1}),
                    CheckList(null, new int?[] {0, 1})
                },
                Run(
                    Fresh(
                        (_y == _x & Fresh(_z == _x, _x) | Fresh(_y == _x, _x) & _z == _x)
                            & new[] {_y, _z} == _r,
                        _x, 
                        _y, 
                        _z), 
                    _r),
                _r);
        }

/*
        (test-check "testc11.tex-40"   
        (run* (r)
            (fresh (x y z)                                                              
            (conde
                ((== y x) (fresh (x) (== z x)))                                         
                ((fresh (x) (== y x)) (== z x)))
            (== #f x)
            (== (cons y (cons z '())) r)))

        `((#f _.0) (_.0 #f)))
*/
        [Fact]
        public void Test11_40()
        {
            AssertAll(
                new object[]
                {
                    CheckList(new object[] {false, null}, new int?[] {null, 1}),
                    CheckList(new object[] {null, false}, new int?[] {0, null})
                },
                Run(
                    Fresh(
                        (_y == _x & Fresh(_z == _x, _x) | Fresh(_y == _x, _x) & _z == _x)
                        & _x == false
                        & new[] { _y, _z } == _r,
                        _x,
                        _y,
                        _z),
                    _r),
                _r);
        }

/*
        (test-check "testc11.tex-41" 
        (run* (q)
            (let ((a (== #t q))
                (b (== #f q)))
            b))

        '(#f))
*/
        [Fact]
        public void Test11_41()
        {
            AssertOneBound(false, Run(Invoke((a, b) => b, _q == true, _q == false), _q), _q);
        }

/*
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
        [Fact]
        public void Test11_42()
        {
            AssertOneBound(
                false, 
                Run(
                    Invoke(
                        (a, b, c) => b, 
                        _q == true, 
                        Fresh(_x == _q & _x == false, _x), 
                        _q == true & Succeed | Succeed & _q == false), 
                    _q), 
                _q);
        }

        private IEnumerable<IReadOnlyDictionary<Var, Var>> Run1(Relation relation, params Var[] variables) => 
            Run(relation, variables).Take(1);

        private IEnumerable<IReadOnlyDictionary<Var, Var>> Run2(Relation relation, params Var[] variables) => 
            Run(relation, variables).Take(2);

        //private Func<T, TRet> Lambda<T, TRet>(Func<T, TRet> lambda) => lambda;

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
        private void AssertOneBound<T>(T value, IEnumerable<IReadOnlyDictionary<Var, Var>> results, Var v)
        {
            var list = results.ToList();
            Assert.Single(list);
            Assert.Single(list[0]);
            Assert.True(list[0].TryGetValue(v, out var r));
            Assert.IsType<ValueVar<T>>(r);
            Assert.Equal(value, ((ValueVar<T>)r).Value);
        }

        [AssertionMethod]
        private void AssertOneUnbound(IEnumerable<IReadOnlyDictionary<Var, Var>> results, Var v)
        {
            var list = results.ToList();
            Assert.Single(list);
            Assert.Single(list[0]);
            Assert.True(list[0].TryGetValue(v, out var r));
            Assert.False(r.Bound);
        }

        [AssertionMethod]
        private void AssertAll(object[] values, IEnumerable<IReadOnlyDictionary<Var, Var>> results, Var v)
        {
            var list = results.ToList();
            Assert.Equal(values.Length, list.Count);
            Assert.All(list, d => Assert.Single(d));
            foreach (var value in values)
            {
                if (value is Predicate<Var> predicate)
                {
                    Assert.Contains(
                        list,
                        d => d.TryGetValue(v, out var x)
                             && predicate(x));
                }
                else
                {
                    Assert.Contains(
                        list,
                        d => d.TryGetValue(v, out var x)
                             && (value == null && !x.Bound
                                 || Equals((x as ValueVar)?.UntypedValue, value)));
                }
            }
        }

        [AssertionMethod]
        private void AssertExists(object[] values, IEnumerable<IReadOnlyDictionary<Var, Var>> results, Var v, int count = 1)
        {
            var list = results.ToList();            
            Assert.All(list, d => Assert.Single(d));
            var total = 0;
            foreach (var value in values)
            {
                if (value is Predicate<Var> predicate)
                {
                    if (list.Exists(d => d.TryGetValue(v, out var x)
                                         && predicate(x)))
                    {
                        total++;
                    }

                } else if (list.Exists(d => d.TryGetValue(v, out var x)
                                     && (value == null && !x.Bound
                                         || Equals((x as ValueVar)?.UntypedValue, value))))
                {
                    total++;
                }
            }
            Assert.Equal(count, total);
        }

        private Predicate<Var> CheckList(object[] values, int?[] indices) => variable =>
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
        

        private readonly Var _q = new Var();
        private readonly Var _x = new Var();
        private readonly Var _y = new Var();
        private readonly Var _z = new Var();
        private readonly Var _r = new Var();
        private readonly Var _t = new Var();
        private readonly Var _u = new Var();
        private readonly Var _s = new Var();
    }
}
