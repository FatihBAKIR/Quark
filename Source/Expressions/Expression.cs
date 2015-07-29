using System;
using Quark.Contexts;
using UnityEngine;

namespace Quark.Expressions
{
    public interface IExpression : IContextful
    {
        /// <summary>
        /// This method should calculate the expression's value without a target.
        /// </summary>
        /// <returns>Value of the expression</returns>
        float Calculate();

        /// <summary>
        /// This method should calculate the expression's value respective with a point target.
        /// </summary>
        /// <param name="point">The point</param>
        /// <returns>Value of the expression</returns>
        float Calculate(Vector3 point);

        /// <summary>
        /// This method should calculate the expression's value respective with a targetable.
        /// </summary>
        /// <param name="target">The targetable</param>
        /// <returns>Value of the expression</returns>
        float Calculate(Targetable target);

        /// <summary>
        /// This method should calculate the expression's value respective with a character.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        float Calculate(Character target);
    }

    public interface IExpression<in T> : IExpression, IContextful<T> where T : IContext
    { }

    /// <summary>
    /// This class should calculate a float value depending on the context and the given target.
    /// </summary>
    /// <typeparam name="T">Context requirement of the expression.</typeparam>
    public abstract class Expression<T> : IExpression<T> where T : IContext
    {
        protected T Context;

        public virtual void SetContext(IContext context)
        {
            Context = (T)context;
        }

        public virtual float Calculate()
        {
            throw new NotImplementedException("Calculate method is not implemented!");
        }

        public virtual float Calculate(Vector3 point)
        {
            throw new NotImplementedException("Calculate method is not implemented!");
        }

        public virtual float Calculate(Targetable target)
        {
            throw new NotImplementedException("Calculate method is not implemented!");
        }

        public virtual float Calculate(Character target)
        {
            throw new NotImplementedException("Calculate method is not implemented!");
        }
    }

}
