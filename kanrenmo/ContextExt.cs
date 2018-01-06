
//  
// This is an auto-generated file. DO NOT EDIT IT DIRECTLY unless you know what you are doing 
//
 
using System;
using System.Collections.Generic;
using Kanrenmo.Annotations;
namespace Kanrenmo
{
	public partial class Context
	{
        /// <summary>
        /// Executes the relation specified by function body querying the variables provided by the function arguments
        /// </summary>
        /// <param name="function">The relation function.</param>
        /// <returns>The enumeration of execution results</returns>
        [NotNull, Pure]
		public static IEnumerable<IReadOnlyList<Var>> Run([NotNull]Func<Var, Relation> function)
		{
            var v1 = new Var();
            return Run(function(v1), v1);
		}
        
        /// <summary>
        /// Creates fresh variables specified by the function parameters for the relation 
        /// specified by the function body
        /// </summary>
        /// <param name="function">The relation function.</param>
        /// <returns>The new relation with fresh variables</returns>
        [NotNull, Pure]
		public static Relation Fresh([NotNull]Func<Var, Relation> function)
		{
            var v1 = new Var();
            return Fresh(function(v1), v1);
		}

        /// <summary>
        /// Invokes the specified relation function with the specified variable arguments.
        /// </summary>
        [NotNull, Pure]    
        public static Relation Invoke([NotNull]Func<Var, Relation> function, Var a1)
        {
            var v1 = new Var();
            return new Relation(
                context => 
                    Fresh(a1 == v1 & function(v1),
                        v1).Execute(context));
        }

        /// <summary>
        /// Invokes the specified relation function with the specified relation arguments.
        /// </summary>
        [NotNull, Pure] 
        public static Relation Invoke([NotNull]Func<Relation, Relation> function, Relation r1) =>
            new Relation(context => function(r1).Execute(context));

        /// <summary>
        /// Executes the relation specified by function body querying the variables provided by the function arguments
        /// </summary>
        /// <param name="function">The relation function.</param>
        /// <returns>The enumeration of execution results</returns>
        [NotNull, Pure]
		public static IEnumerable<IReadOnlyList<Var>> Run([NotNull]Func<Var, Var, Relation> function)
		{
            var v1 = new Var();
            var v2 = new Var();
            return Run(function(v1, v2), v1, v2);
		}
        
        /// <summary>
        /// Creates fresh variables specified by the function parameters for the relation 
        /// specified by the function body
        /// </summary>
        /// <param name="function">The relation function.</param>
        /// <returns>The new relation with fresh variables</returns>
        [NotNull, Pure]
		public static Relation Fresh([NotNull]Func<Var, Var, Relation> function)
		{
            var v1 = new Var();
            var v2 = new Var();
            return Fresh(function(v1, v2), v1, v2);
		}

        /// <summary>
        /// Invokes the specified relation function with the specified variable arguments.
        /// </summary>
        [NotNull, Pure]    
        public static Relation Invoke([NotNull]Func<Var, Var, Relation> function, Var a1, Var a2)
        {
            var v1 = new Var();
            var v2 = new Var();
            return new Relation(
                context => 
                    Fresh(a1 == v1 & a2 == v2 & function(v1, v2),
                        v1, v2).Execute(context));
        }

        /// <summary>
        /// Invokes the specified relation function with the specified relation arguments.
        /// </summary>
        [NotNull, Pure] 
        public static Relation Invoke([NotNull]Func<Relation, Relation, Relation> function, Relation r1, Relation r2) =>
            new Relation(context => function(r1, r2).Execute(context));

        /// <summary>
        /// Executes the relation specified by function body querying the variables provided by the function arguments
        /// </summary>
        /// <param name="function">The relation function.</param>
        /// <returns>The enumeration of execution results</returns>
        [NotNull, Pure]
		public static IEnumerable<IReadOnlyList<Var>> Run([NotNull]Func<Var, Var, Var, Relation> function)
		{
            var v1 = new Var();
            var v2 = new Var();
            var v3 = new Var();
            return Run(function(v1, v2, v3), v1, v2, v3);
		}
        
        /// <summary>
        /// Creates fresh variables specified by the function parameters for the relation 
        /// specified by the function body
        /// </summary>
        /// <param name="function">The relation function.</param>
        /// <returns>The new relation with fresh variables</returns>
        [NotNull, Pure]
		public static Relation Fresh([NotNull]Func<Var, Var, Var, Relation> function)
		{
            var v1 = new Var();
            var v2 = new Var();
            var v3 = new Var();
            return Fresh(function(v1, v2, v3), v1, v2, v3);
		}

        /// <summary>
        /// Invokes the specified relation function with the specified variable arguments.
        /// </summary>
        [NotNull, Pure]    
        public static Relation Invoke([NotNull]Func<Var, Var, Var, Relation> function, Var a1, Var a2, Var a3)
        {
            var v1 = new Var();
            var v2 = new Var();
            var v3 = new Var();
            return new Relation(
                context => 
                    Fresh(a1 == v1 & a2 == v2 & a3 == v3 & function(v1, v2, v3),
                        v1, v2, v3).Execute(context));
        }

        /// <summary>
        /// Invokes the specified relation function with the specified relation arguments.
        /// </summary>
        [NotNull, Pure] 
        public static Relation Invoke([NotNull]Func<Relation, Relation, Relation, Relation> function, Relation r1, Relation r2, Relation r3) =>
            new Relation(context => function(r1, r2, r3).Execute(context));

        /// <summary>
        /// Executes the relation specified by function body querying the variables provided by the function arguments
        /// </summary>
        /// <param name="function">The relation function.</param>
        /// <returns>The enumeration of execution results</returns>
        [NotNull, Pure]
		public static IEnumerable<IReadOnlyList<Var>> Run([NotNull]Func<Var, Var, Var, Var, Relation> function)
		{
            var v1 = new Var();
            var v2 = new Var();
            var v3 = new Var();
            var v4 = new Var();
            return Run(function(v1, v2, v3, v4), v1, v2, v3, v4);
		}
        
        /// <summary>
        /// Creates fresh variables specified by the function parameters for the relation 
        /// specified by the function body
        /// </summary>
        /// <param name="function">The relation function.</param>
        /// <returns>The new relation with fresh variables</returns>
        [NotNull, Pure]
		public static Relation Fresh([NotNull]Func<Var, Var, Var, Var, Relation> function)
		{
            var v1 = new Var();
            var v2 = new Var();
            var v3 = new Var();
            var v4 = new Var();
            return Fresh(function(v1, v2, v3, v4), v1, v2, v3, v4);
		}

        /// <summary>
        /// Invokes the specified relation function with the specified variable arguments.
        /// </summary>
        [NotNull, Pure]    
        public static Relation Invoke([NotNull]Func<Var, Var, Var, Var, Relation> function, Var a1, Var a2, Var a3, Var a4)
        {
            var v1 = new Var();
            var v2 = new Var();
            var v3 = new Var();
            var v4 = new Var();
            return new Relation(
                context => 
                    Fresh(a1 == v1 & a2 == v2 & a3 == v3 & a4 == v4 & function(v1, v2, v3, v4),
                        v1, v2, v3, v4).Execute(context));
        }

        /// <summary>
        /// Invokes the specified relation function with the specified relation arguments.
        /// </summary>
        [NotNull, Pure] 
        public static Relation Invoke([NotNull]Func<Relation, Relation, Relation, Relation, Relation> function, Relation r1, Relation r2, Relation r3, Relation r4) =>
            new Relation(context => function(r1, r2, r3, r4).Execute(context));

        /// <summary>
        /// Executes the relation specified by function body querying the variables provided by the function arguments
        /// </summary>
        /// <param name="function">The relation function.</param>
        /// <returns>The enumeration of execution results</returns>
        [NotNull, Pure]
		public static IEnumerable<IReadOnlyList<Var>> Run([NotNull]Func<Var, Var, Var, Var, Var, Relation> function)
		{
            var v1 = new Var();
            var v2 = new Var();
            var v3 = new Var();
            var v4 = new Var();
            var v5 = new Var();
            return Run(function(v1, v2, v3, v4, v5), v1, v2, v3, v4, v5);
		}
        
        /// <summary>
        /// Creates fresh variables specified by the function parameters for the relation 
        /// specified by the function body
        /// </summary>
        /// <param name="function">The relation function.</param>
        /// <returns>The new relation with fresh variables</returns>
        [NotNull, Pure]
		public static Relation Fresh([NotNull]Func<Var, Var, Var, Var, Var, Relation> function)
		{
            var v1 = new Var();
            var v2 = new Var();
            var v3 = new Var();
            var v4 = new Var();
            var v5 = new Var();
            return Fresh(function(v1, v2, v3, v4, v5), v1, v2, v3, v4, v5);
		}

        /// <summary>
        /// Invokes the specified relation function with the specified variable arguments.
        /// </summary>
        [NotNull, Pure]    
        public static Relation Invoke([NotNull]Func<Var, Var, Var, Var, Var, Relation> function, Var a1, Var a2, Var a3, Var a4, Var a5)
        {
            var v1 = new Var();
            var v2 = new Var();
            var v3 = new Var();
            var v4 = new Var();
            var v5 = new Var();
            return new Relation(
                context => 
                    Fresh(a1 == v1 & a2 == v2 & a3 == v3 & a4 == v4 & a5 == v5 & function(v1, v2, v3, v4, v5),
                        v1, v2, v3, v4, v5).Execute(context));
        }

        /// <summary>
        /// Invokes the specified relation function with the specified relation arguments.
        /// </summary>
        [NotNull, Pure] 
        public static Relation Invoke([NotNull]Func<Relation, Relation, Relation, Relation, Relation, Relation> function, Relation r1, Relation r2, Relation r3, Relation r4, Relation r5) =>
            new Relation(context => function(r1, r2, r3, r4, r5).Execute(context));

        /// <summary>
        /// Executes the relation specified by function body querying the variables provided by the function arguments
        /// </summary>
        /// <param name="function">The relation function.</param>
        /// <returns>The enumeration of execution results</returns>
        [NotNull, Pure]
		public static IEnumerable<IReadOnlyList<Var>> Run([NotNull]Func<Var, Var, Var, Var, Var, Var, Relation> function)
		{
            var v1 = new Var();
            var v2 = new Var();
            var v3 = new Var();
            var v4 = new Var();
            var v5 = new Var();
            var v6 = new Var();
            return Run(function(v1, v2, v3, v4, v5, v6), v1, v2, v3, v4, v5, v6);
		}
        
        /// <summary>
        /// Creates fresh variables specified by the function parameters for the relation 
        /// specified by the function body
        /// </summary>
        /// <param name="function">The relation function.</param>
        /// <returns>The new relation with fresh variables</returns>
        [NotNull, Pure]
		public static Relation Fresh([NotNull]Func<Var, Var, Var, Var, Var, Var, Relation> function)
		{
            var v1 = new Var();
            var v2 = new Var();
            var v3 = new Var();
            var v4 = new Var();
            var v5 = new Var();
            var v6 = new Var();
            return Fresh(function(v1, v2, v3, v4, v5, v6), v1, v2, v3, v4, v5, v6);
		}

        /// <summary>
        /// Invokes the specified relation function with the specified variable arguments.
        /// </summary>
        [NotNull, Pure]    
        public static Relation Invoke([NotNull]Func<Var, Var, Var, Var, Var, Var, Relation> function, Var a1, Var a2, Var a3, Var a4, Var a5, Var a6)
        {
            var v1 = new Var();
            var v2 = new Var();
            var v3 = new Var();
            var v4 = new Var();
            var v5 = new Var();
            var v6 = new Var();
            return new Relation(
                context => 
                    Fresh(a1 == v1 & a2 == v2 & a3 == v3 & a4 == v4 & a5 == v5 & a6 == v6 & function(v1, v2, v3, v4, v5, v6),
                        v1, v2, v3, v4, v5, v6).Execute(context));
        }

        /// <summary>
        /// Invokes the specified relation function with the specified relation arguments.
        /// </summary>
        [NotNull, Pure] 
        public static Relation Invoke([NotNull]Func<Relation, Relation, Relation, Relation, Relation, Relation, Relation> function, Relation r1, Relation r2, Relation r3, Relation r4, Relation r5, Relation r6) =>
            new Relation(context => function(r1, r2, r3, r4, r5, r6).Execute(context));

        /// <summary>
        /// Executes the relation specified by function body querying the variables provided by the function arguments
        /// </summary>
        /// <param name="function">The relation function.</param>
        /// <returns>The enumeration of execution results</returns>
        [NotNull, Pure]
		public static IEnumerable<IReadOnlyList<Var>> Run([NotNull]Func<Var, Var, Var, Var, Var, Var, Var, Relation> function)
		{
            var v1 = new Var();
            var v2 = new Var();
            var v3 = new Var();
            var v4 = new Var();
            var v5 = new Var();
            var v6 = new Var();
            var v7 = new Var();
            return Run(function(v1, v2, v3, v4, v5, v6, v7), v1, v2, v3, v4, v5, v6, v7);
		}
        
        /// <summary>
        /// Creates fresh variables specified by the function parameters for the relation 
        /// specified by the function body
        /// </summary>
        /// <param name="function">The relation function.</param>
        /// <returns>The new relation with fresh variables</returns>
        [NotNull, Pure]
		public static Relation Fresh([NotNull]Func<Var, Var, Var, Var, Var, Var, Var, Relation> function)
		{
            var v1 = new Var();
            var v2 = new Var();
            var v3 = new Var();
            var v4 = new Var();
            var v5 = new Var();
            var v6 = new Var();
            var v7 = new Var();
            return Fresh(function(v1, v2, v3, v4, v5, v6, v7), v1, v2, v3, v4, v5, v6, v7);
		}

        /// <summary>
        /// Invokes the specified relation function with the specified variable arguments.
        /// </summary>
        [NotNull, Pure]    
        public static Relation Invoke([NotNull]Func<Var, Var, Var, Var, Var, Var, Var, Relation> function, Var a1, Var a2, Var a3, Var a4, Var a5, Var a6, Var a7)
        {
            var v1 = new Var();
            var v2 = new Var();
            var v3 = new Var();
            var v4 = new Var();
            var v5 = new Var();
            var v6 = new Var();
            var v7 = new Var();
            return new Relation(
                context => 
                    Fresh(a1 == v1 & a2 == v2 & a3 == v3 & a4 == v4 & a5 == v5 & a6 == v6 & a7 == v7 & function(v1, v2, v3, v4, v5, v6, v7),
                        v1, v2, v3, v4, v5, v6, v7).Execute(context));
        }

        /// <summary>
        /// Invokes the specified relation function with the specified relation arguments.
        /// </summary>
        [NotNull, Pure] 
        public static Relation Invoke([NotNull]Func<Relation, Relation, Relation, Relation, Relation, Relation, Relation, Relation> function, Relation r1, Relation r2, Relation r3, Relation r4, Relation r5, Relation r6, Relation r7) =>
            new Relation(context => function(r1, r2, r3, r4, r5, r6, r7).Execute(context));

        /// <summary>
        /// Executes the relation specified by function body querying the variables provided by the function arguments
        /// </summary>
        /// <param name="function">The relation function.</param>
        /// <returns>The enumeration of execution results</returns>
        [NotNull, Pure]
		public static IEnumerable<IReadOnlyList<Var>> Run([NotNull]Func<Var, Var, Var, Var, Var, Var, Var, Var, Relation> function)
		{
            var v1 = new Var();
            var v2 = new Var();
            var v3 = new Var();
            var v4 = new Var();
            var v5 = new Var();
            var v6 = new Var();
            var v7 = new Var();
            var v8 = new Var();
            return Run(function(v1, v2, v3, v4, v5, v6, v7, v8), v1, v2, v3, v4, v5, v6, v7, v8);
		}
        
        /// <summary>
        /// Creates fresh variables specified by the function parameters for the relation 
        /// specified by the function body
        /// </summary>
        /// <param name="function">The relation function.</param>
        /// <returns>The new relation with fresh variables</returns>
        [NotNull, Pure]
		public static Relation Fresh([NotNull]Func<Var, Var, Var, Var, Var, Var, Var, Var, Relation> function)
		{
            var v1 = new Var();
            var v2 = new Var();
            var v3 = new Var();
            var v4 = new Var();
            var v5 = new Var();
            var v6 = new Var();
            var v7 = new Var();
            var v8 = new Var();
            return Fresh(function(v1, v2, v3, v4, v5, v6, v7, v8), v1, v2, v3, v4, v5, v6, v7, v8);
		}

        /// <summary>
        /// Invokes the specified relation function with the specified variable arguments.
        /// </summary>
        [NotNull, Pure]    
        public static Relation Invoke([NotNull]Func<Var, Var, Var, Var, Var, Var, Var, Var, Relation> function, Var a1, Var a2, Var a3, Var a4, Var a5, Var a6, Var a7, Var a8)
        {
            var v1 = new Var();
            var v2 = new Var();
            var v3 = new Var();
            var v4 = new Var();
            var v5 = new Var();
            var v6 = new Var();
            var v7 = new Var();
            var v8 = new Var();
            return new Relation(
                context => 
                    Fresh(a1 == v1 & a2 == v2 & a3 == v3 & a4 == v4 & a5 == v5 & a6 == v6 & a7 == v7 & a8 == v8 & function(v1, v2, v3, v4, v5, v6, v7, v8),
                        v1, v2, v3, v4, v5, v6, v7, v8).Execute(context));
        }

        /// <summary>
        /// Invokes the specified relation function with the specified relation arguments.
        /// </summary>
        [NotNull, Pure] 
        public static Relation Invoke([NotNull]Func<Relation, Relation, Relation, Relation, Relation, Relation, Relation, Relation, Relation> function, Relation r1, Relation r2, Relation r3, Relation r4, Relation r5, Relation r6, Relation r7, Relation r8) =>
            new Relation(context => function(r1, r2, r3, r4, r5, r6, r7, r8).Execute(context));

	}
}