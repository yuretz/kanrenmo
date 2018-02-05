using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Kanrenmo.Annotations;

namespace Kanrenmo
{
    /// <summary>
    /// Kanren execution context (variable scope)
    /// </summary>
    public partial class Context
    {
        /// <summary>
        /// The relation that always succeeds
        /// </summary>
        public static readonly Relation Succeed = Relation.Identity;

        /// <summary>
        /// The relation that always fails
        /// </summary>
        public static readonly Relation Fail = Relation.Failure;      

        /// <summary>
        /// Solve the specified relation and query the variables.
        /// </summary>
        /// <param name="relation">The relation to solve.</param>
        /// <param name="variables">The variables to query.</param>
        /// <returns>Variable bindings enumeration</returns>
        [NotNull, Pure]
        public static IEnumerable<Binding> Solve([NotNull] Relation relation, params Var[] variables) =>
            new Context()
                // add vars to context
                .With(variables)
                // run the relation in this context
                .Apply(relation)
                // query results from all the returned contexts
                .Select(context => context.QueryAll(variables));


        /// <summary>
        /// Adds scoped variables and relation to this context
        /// </summary>
        /// <param name="relation">The scoped relation.</param>
        /// <param name="variables">The scoped variables.</param>
        /// <returns>Resulting relation</returns>
        [NotNull, Pure]
        public static Relation Declare(Relation relation, params Var[] variables) =>
            new Relation(parent =>
                    // extend the existing scope with fresh vars
                    parent.With(variables)
                        // and apply the relation
                        .Apply(relation)
                        // restore the scope for each resulting context
                        .Select(child => new Context(parent._scope, child._environment, child._constraints)));

        /// <summary>
        /// Invokes the specified relation function.
        /// </summary>
        /// <param name="function">The function to invoke.</param>
        /// <returns>Resulting relation</returns>
        [NotNull, Pure, UsedImplicitly]
        public static Relation Invoke([NotNull] Func<Relation> function) =>
            new Relation(function);

        /// <summary>
        /// Makes a variable out of the specified value
        /// </summary>
        /// <typeparam name="T">the value type</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>variable instance</returns>
        [NotNull, Pure]
        public static Var Var<T>(T value) => (ValueVar<T>) value;

        [NotNull, Pure]
        public static Var Pair(Var head, Var tail) => new PairVar(head, tail);

        /// <summary>
        /// Converts variable enumeration to a sequence of nested <see cref="PairVar"/>
        /// </summary>
        /// <param name="variables">The variables enumeration.</param>
        /// <returns>new sequence of nested pair variable instances</returns>
        [NotNull, Pure, UsedImplicitly]
        public static Var Seq([CanBeNull] IEnumerable<Var> variables) => 
            variables == null ? (Kanrenmo.Var.Empty) : Seq(variables.GetEnumerator());

        /// <summary>
        /// Converts variables to a to a sequence of nested <see cref="PairVar"/>
        /// </summary>
        /// <param name="variables">The variables.</param>
        /// <returns>new sequence of nested pair variable instances</returns>
        [NotNull, Pure]
        public static Var Seq(params Var[] variables) => Seq(variables.AsEnumerable());

        /// <summary>
        /// Convert the variables enumeration to S-expression
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <returns>
        /// S-expression string
        /// </returns>
        [NotNull, UsedImplicitly]
        public static string ToSExpression([NotNull] Binding binding)
        {
            IReadOnlyList<Var> variables = binding;
            IReadOnlyList<Constraint> constraints = binding.Constraints;
            var unbound = new SortedList<int,Var>();
            var varExpr = string.Join(" ", variables.Select(v => v.ToSExpression(unbound)));  
            var reified = variables.Count != 1 ? "(" + varExpr + ")" : varExpr;
            var constraintExpr = string.Join(" ", constraints.Select(c => c.ToSExpression(unbound)));
            return constraints.Count != 0 ? "(" + reified + " : " + constraintExpr + ")" : reified;
        }

        /// <summary>
        /// Convert solutions to S-expression
        /// </summary>
        /// <param name="solutions">The solutions enumeration.</param>
        /// <returns>
        /// S-expression string
        /// </returns>
        [NotNull, UsedImplicitly]
        public static string ToSExpression(IEnumerable<Binding> solutions) =>
            "(" + string.Join(" ", solutions.Select(ToSExpression)) + ")";

        internal static readonly IEnumerable<Context> Nothing = Enumerable.Empty<Context>();

        [NotNull, Pure]
        internal static IEnumerable<Context> Just(Context context) => Enumerable.Repeat(context, 1);

        [NotNull, Pure]
        internal static IEnumerable<Context> Maybe([CanBeNull] Context context) => context != null ? Just(context) : Nothing;

        /// <summary>
        /// Unifies two variables.
        /// </summary>
        /// <param name="left">The left variable.</param>
        /// <param name="right">The right variable.</param>
        /// <returns>Resulting context enumeration</returns>
        [NotNull, Pure]
        internal IEnumerable<Context> Unify(Var left, Var right)
        {
            left = Reify(left);
            right = Reify(right);

            if (Equals(left, right))
            {
                // this is also useful to avoid circular bindings
                return Just(this);
            }

            if (!left.Bound)
            {
                return UnifyUnbound(left, right).SelectMany(c => c.CheckAllConstraints());
            }

            if (!right.Bound)
            {
                return UnifyUnbound(right, left).SelectMany(c => c.CheckAllConstraints());
            }
                
            if (left is PairVar leftSeq && right is PairVar rightSeq)
            {
                return UnifySequences(leftSeq, rightSeq).SelectMany(c => c.CheckAllConstraints());
            }

            return Nothing;
        }
        /// <summary>
        /// Reifies the specified variable.
        /// </summary>
        /// <param name="variable">The variable to reify.</param>
        /// <returns>Reification</returns>
        [NotNull]
        internal Var Reify(Var variable)
        {
            switch (variable)
            {
                case ValueVar value:
                    return ReifyImpl(value);
                case PairVar sequence:
                    return ReifyImpl(sequence);
                default:
                    return ReifyImpl(variable);
            }
        }

        /// <summary>
        /// Enforces the specified constraint on this context.
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        /// <returns></returns>
        [NotNull]
        internal IEnumerable<Context> Enforce(Constraint constraint) =>
            // TODO: reduce here
            Just(new Context(_scope, _environment, _constraints.Add(constraint)));  

        [NotNull]
        private static Var Seq([CanBeNull] IEnumerator<Var> variables) =>
            !(variables?.MoveNext() ?? false) ? Kanrenmo.Var.Empty : variables.Current.Combine(Seq(variables));

        /// <summary>
        /// Initializes a new instance of the <see cref="Context" /> class.
        /// </summary>
        /// <param name="scope">The scope variables.</param>
        /// <param name="environment">The environment containing existing variable bindings.</param>
        /// <param name="constraints">The constraints enforced on this context but not yet resolved.</param>
        private Context(
            [CanBeNull] ImmutableDictionary<Var, Var> scope = null, 
            [CanBeNull] ImmutableDictionary<Var, Var> environment = null,
            [CanBeNull] ImmutableList<Constraint> constraints = null)
        {
            _scope = scope ?? ImmutableDictionary<Var, Var>.Empty;
            _environment = environment ?? ImmutableDictionary<Var, Var>.Empty;
            _constraints = constraints ?? ImmutableList<Constraint>.Empty;
        }

        /// <summary>
        /// Adds the specified variables to the context
        /// </summary>
        /// <param name="variables">The variables to add.</param>
        /// <returns>The new context instance</returns>
        [NotNull]
        private Context With(params Var[] variables) =>
            new Context(
                _scope.SetItems(variables.Select(v => new KeyValuePair<Var, Var>(v, new Var()))),
                _environment,
                _constraints);

        /// <summary>
        /// Applies the specified relation to this context.
        /// </summary>
        /// <param name="relation">The relation to apply.</param>
        /// <returns>The resulting context enumeration</returns>
        [NotNull]
        private IEnumerable<Context> Apply([NotNull] Relation relation) => relation.Execute(this);

        /// <summary>
        /// Unifies two unbound variables.
        /// </summary>
        /// <param name="unbound">The unbound variable.</param>
        /// <param name="other">The other variable.</param>
        /// <returns>The resulting context</returns>
        [NotNull]
        private IEnumerable<Context> UnifyUnbound([NotNull] Var unbound, [NotNull] Var other) => 
            !other.Includes(unbound) 
                ? Just(new Context(_scope, _environment.Add(unbound, other), _constraints)) 
                : Nothing;

        /// <summary>
        /// Unifies two pair variables.
        /// </summary>
        /// <param name="left">The first pair variable.</param>
        /// <param name="right">The second pair variable.</param>
        /// <returns>The resulting context</returns>
        [NotNull]
        private IEnumerable<Context> UnifySequences([NotNull] PairVar left, [NotNull] PairVar right)
        {
            if (left.IsEmpty || right.IsEmpty)
            {
                return left.IsEmpty && right.IsEmpty ? Just(this) : Nothing;
            }
            
            return Unify(left.Head(), right.Head())
                .SelectMany(c => c.Unify(left.Tail(), right.Tail()));
        }
            
        /// <summary>
        /// Reifies the <see cref="ValueVar"/>.
        /// </summary>
        /// <param name="variable">The variable to reify.</param>
        /// <returns>Reified variable</returns>
        [NotNull]
        private Var ReifyImpl(ValueVar variable) => variable;

        /// <summary>
        /// Reifies the variable.
        /// </summary>
        /// <param name="variable">The variable to reify.</param>
        /// <returns>Reified variable</returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        [NotNull]
        private Var ReifyImpl(Var variable)
        {
            if (!_scope.TryGetValue(variable, out var bound))
            {
                bound = variable;
            }

            return !_environment.TryGetValue(bound, out var result) ? bound : Reify(result);
        }

        /// <summary>
        /// Reifies the pair variable.
        /// </summary>
        /// <param name="pair">The pair variable to reify.</param>
        /// <returns>reified pair</returns>
        [NotNull]
        private Var ReifyImpl([NotNull] PairVar pair) => 
            pair.IsEmpty ? Kanrenmo.Var.Empty : new PairVar(Reify(pair.Head()), Reify(pair.Tail()));

        /// <summary>
        /// Queries all the provided variables.
        /// </summary>
        /// <param name="variables">The variables.</param>
        /// <returns>A dictionary of variables with their bindings</returns>
        [NotNull]
        private Binding QueryAll(IEnumerable<Var> variables) =>
            new Binding(variables.Select(v => new KeyValuePair<Var, Var>(v, Reify(v))), _constraints);

        private IEnumerable<Context> CheckAllConstraints() =>
            _constraints.Aggregate(Just(new Context(_scope, _environment)),
                (contexts, constraint) => contexts.SelectMany(c => c.Apply(constraint)));

        private readonly ImmutableDictionary<Var, Var> _environment;
        private readonly ImmutableDictionary<Var, Var> _scope;
        private readonly ImmutableList<Constraint> _constraints;
    }
}
