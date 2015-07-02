using Quark.Contexts;
using UnityEngine;

namespace Quark.Conditions
{
    public interface ICondition : IContextful
    {
        /// <summary>
        /// Checks whether a condition is met in the context
        /// </summary>
        bool Check();

        /// <summary>
        /// Check whether a condition is met in the context with the specified point.
        /// </summary>
        /// <param name="point">The Point.</param>
        bool Check(Vector3 point);

        /// <summary>
        /// Check whether a condition is met in the context with the specified character.
        /// </summary>
        /// <param name="character">The Character.</param>
        bool Check(Character character);

        /// <summary>
        /// Check whether a condition is met in the context with the specified target.
        /// </summary>
        /// <param name="target">The Targetable.</param>
        bool Check(Targetable target);
        
    }

    public interface ICondition<in T> : ICondition where T : IContext
    {
    }

    public class Condition<T> : ICondition<T> where T : IContext
    {
        /// <summary>
        /// The Context of this Condition.
        /// </summary>
        protected T Context { get; private set; }

        public virtual void SetContext(IContext context)
        {
            Context = (T)context;
        }

        public virtual bool Check()
        {
            return true;
        }

        public virtual bool Check(Vector3 point)
        {
            return true;
        }

        public virtual bool Check(Character character)
        {
            return true;
        }

        public virtual bool Check(Targetable target)
        {
            return true;
        }
    }

    class FalseCondition : Condition<IContext>
    {
        public override bool Check()
        {
            return false;
        }

        public override bool Check(Character character)
        {
            return false;
        }

        public override bool Check(Vector3 point)
        {
            return false;
        }

        public override bool Check(Targetable target)
        {
            return false;
        }
    }
}