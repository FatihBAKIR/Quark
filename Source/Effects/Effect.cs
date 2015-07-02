using Quark.Contexts;
using UnityEngine;

namespace Quark.Effects
{
    public interface IEffect : IContextful
    {
        /// <summary>
        /// Applies this effect without a target.
        /// </summary>
        void Apply();

        /// <summary>
        /// Applies this effect on the specified target Character.
        /// </summary>
        /// <param name='target'>
        /// Target Character.
        /// </param>
        void Apply(Character target);

        /// <summary>
        /// Applies this effect on the specified target Vector3.
        /// </summary>
        /// <param name='point'>  
        /// Target Point.
        /// </param>
        void Apply(Vector3 point);

        /// <summary>
        /// Applies this effect on the specified non character targetable
        /// </summary>
        /// <param name="target">Targetable.</param>
        void Apply(Targetable target);
    }

    public interface IEffect<in T> : IEffect, IContextful<T> where T : IContext
    {
    }

    public abstract class Effect<T> : IEffect<T>, ITagged where T : IContext
    {
        /// <summary>
        /// The context this Effect should apply in.
        /// </summary>
        public T Context { get; private set; }

        public virtual void SetContext(IContext context)
        {
            Context = (T)context;
        }

        /// <summary>
        /// Applies this effect without a target.
        /// </summary>
        public virtual void Apply()
        {
        }

        /// <summary>
        /// Applies this effect on the specified target Character.
        /// </summary>
        /// <param name='target'>
        /// Target Character.
        /// </param>
        public virtual void Apply(Character target)
        {
        }

        /// <summary>
        /// Applies this effect on the specified target Vector3.
        /// </summary>
        /// <param name='point'>  
        /// Target Point.
        /// </param>
        public virtual void Apply(Vector3 point)
        {
        }

        /// <summary>
        /// Applies this effect on the specified non character targetable
        /// </summary>
        /// <param name="target">Targetable.</param>
        public virtual void Apply(Targetable target)
        {
        }

        #region Tagging
        /// <summary>
        /// The tags of this Effect.
        /// </summary>
        public StaticTags Tags { get; protected set; }

        /// <summary>
        /// Checks whether this Effect is tagged with a particular string or not.
        /// </summary>
        /// <param name="tag">The string to check.</param>
        /// <returns>Whether this Effect is tagged or not.</returns>
        public bool IsTagged(string tag)
        {
            return Tags.Has(tag);
        }
        #endregion
    }
}