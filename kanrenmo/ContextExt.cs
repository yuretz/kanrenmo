using System;

namespace Kanrenmo
{
    public partial class Context
    {
        /// <summary>
        /// Invokes the specified function.
        /// </summary>
        /// <param name="function">The function to invoke.</param>
        /// <returns>Resulting relation</returns>
        public static Relation Invoke(Func<Relation> function) =>
            new Relation(context => function().Execute(context));
      

        /// <summary>
        /// Invokes the specified function.
        /// </summary>
        /// <param name="function">The function to invoke.</param>
        /// <param name="argument">The argument value to pass to the function.</param>
        /// <returns>Resulting relation</returns>
        public static Relation Invoke(Func<Var, Relation> function, Var argument)
        {
            var param = new Var();
            return new Relation(context => Fresh((param == argument) & function(param), param).Execute(context));
        }

        /// <summary>
        /// Invokes the specified function.
        /// </summary>
        /// <param name="function">The function to invoke.</param>
        /// <param name="argument1">The first argument value to pass to the function.</param>
        /// <param name="argument2">The second argument value to pass to the function.</param>
        /// <returns>
        /// Resulting relation
        /// </returns>
        public static Relation Invoke(Func<Var, Var, Relation> function, Var argument1, Var argument2)
        {
            var param1 = new Var();
            var param2 = new Var();
            return new Relation(context => 
                Fresh((param1 == argument1) 
                        & (param2 == argument2)
                        & function(param1, param2), 
                    param1, 
                    param2).Execute(context));
        }

        /// <summary>
        /// Invokes the specified function.
        /// </summary>
        /// <param name="function">The function to invoke.</param>
        /// <param name="argument1">The first argument value to pass to the function.</param>
        /// <param name="argument2">The second argument value to pass to the function.</param>
        /// <param name="argument3">The third argument value to pass to the function.</param>
        /// <returns>
        /// Resulting relation
        /// </returns>
        public static Relation Invoke(Func<Var, Var, Var, Relation> function, Var argument1, Var argument2, 
            Var argument3)
        {
            var param1 = new Var();
            var param2 = new Var();
            var param3 = new Var();
            return new Relation(context =>
                Fresh((param1 == argument1)
                      & (param2 == argument2)
                      & (param3 == argument3)
                      & function(param1, param2, param3),
                    param1,
                    param2,
                    param3).Execute(context));
        }

        /// <summary>
        /// Invokes the specified function.
        /// </summary>
        /// <param name="function">The function to invoke.</param>
        /// <param name="argument">The argument value to pass to the function.</param>
        /// <returns>Resulting relation</returns>
        public static Relation Invoke(Func<Relation, Relation> function, Relation argument) =>
            new Relation(context => function(argument).Execute(context));

        /// <summary>
        /// Invokes the specified function.
        /// </summary>
        /// <param name="function">The function to invoke.</param>
        /// <param name="argument1">The first argument value to pass to the function.</param>
        /// <param name="argument2">The second argument value to pass to the function.</param>
        /// <returns>Resulting relation</returns>
        public static Relation Invoke(
            Func<Relation, Relation, Relation> function, 
            Relation argument1,
            Relation argument2) =>
            new Relation(context => function(argument1, argument2).Execute(context));

        /// <summary>
        /// Invokes the specified function.
        /// </summary>
        /// <param name="function">The function to invoke.</param>
        /// <param name="argument1">The first argument value to pass to the function.</param>
        /// <param name="argument2">The second argument value to pass to the function.</param>
        /// <param name="argument3">The third argument value to pass to the function.</param>
        /// <returns>Resulting relation</returns>
        public static Relation Invoke(
            Func<Relation, Relation, Relation, Relation> function,
            Relation argument1,
            Relation argument2,
            Relation argument3) =>
            new Relation(context => function(argument1, argument2, argument3).Execute(context));

    }
}
