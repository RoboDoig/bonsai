﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Bonsai.Expressions
{
    /// <summary>
    /// Represents an expression builder which uses a specified condition instance
    /// to process an observable sequence.
    /// </summary>
    [WorkflowElementCategory(ElementCategory.Condition)]
    [XmlType("Condition", Namespace = Constants.XmlNamespace)]
    public class ConditionBuilder : WorkflowExpressionBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionBuilder"/> class.
        /// </summary>
        public ConditionBuilder()
            : this(new ExpressionBuilderGraph())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionBuilder"/> class
        /// with the specified expression builder workflow.
        /// </summary>
        /// <param name="workflow">
        /// The expression builder workflow instance that will be used by this builder
        /// to generate the output expression tree.
        /// </param>
        public ConditionBuilder(ExpressionBuilderGraph workflow)
            : base(workflow, minArguments: 1, maxArguments: 1)
        {
        }

        /// <summary>
        /// Generates an <see cref="Expression"/> node from a collection of input arguments.
        /// The result can be chained with other builders in a workflow.
        /// </summary>
        /// <param name="arguments">
        /// A collection of <see cref="Expression"/> nodes that represents the input arguments.
        /// </param>
        /// <returns>An <see cref="Expression"/> tree node.</returns>
        public override Expression Build(IEnumerable<Expression> arguments)
        {
            // Assign input
            var source = arguments.Single();
            var selectorParameter = Expression.Parameter(source.Type);
            return BuildWorflow(arguments, selectorParameter, selectorBody =>
            {
                var selector = Expression.Lambda(selectorBody, selectorParameter);
                var selectorObservableType = selector.ReturnType.GetGenericArguments()[0];
                if (selectorObservableType != typeof(bool))
                {
                    throw new InvalidOperationException("The specified condition workflow must have a single boolean output.");
                }

                return Expression.Call(GetType(), "Process", source.Type.GetGenericArguments(), source, selector);
            });
        }

        static IObservable<TSource> Process<TSource>(IObservable<TSource> source, Func<IObservable<TSource>, IObservable<bool>> condition)
        {
            return Observable.Defer(() =>
            {
                var filter = false;
                return source.Publish(ps => ps.CombineLatest(condition(ps), (xs, ys) => { filter = ys; return xs; }).Sample(ps).Where(xs => filter));
            });
        }
    }
}
