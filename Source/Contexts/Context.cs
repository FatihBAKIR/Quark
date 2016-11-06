using System.Collections.Generic;
using Quark.Utilities;

namespace Quark.Contexts
{
    /// <summary>
    /// This interface provides an abstract way of setting the context of a contextful behaviors.
    /// </summary>
    public interface IContextful
    {
        /// <summary>
        /// Sets the context of this Effect.
        /// </summary>
        /// <param name="context">The context.</param>
        void SetContext(IContext context);
    }

    /// <summary>
    /// This interface provides an abstract way of setting the context of a contextful behaviors.
    /// </summary>
    /// <typeparam name="T">Context type.</typeparam>
    public interface IContextful<in T> : IContextful where T : IContext
    {
    }

    /// <summary>
    /// This interface provides the basic properties of a Context class.
    /// </summary>
    public interface IContext : Identifiable
    {
        /// <summary>
        /// Source of this Context.
        /// </summary>
        Character Source { get; }

        /// <summary>
        /// This property stores the parent Context of this Context instance.
        /// </summary>
        IContext Parent { get; set; }
    }

    /// <summary>
    /// Contexts provide states for the various behaviors in Quark pattern.
    /// This is the most basic Context. 
    /// Since every Context must originate from a Character this Context only stores a source Character state.
    /// </summary>
    public class Context : IContext
    {
        /// <summary>
        /// Creates a new Context instance from a given Character source.
        /// </summary>
        /// <param name="source">The source Character.</param>
        public Context(Character source)
        {
            Source = source;
            Identifier = GetHashCode() + "@" + Source.Identifier;
            _children = new List<IContext>();
        }

        /// <summary>
        /// Creates a new Context instance from a given parent Context.
        /// </summary>
        /// <param name="parent">Parent context.</param>
        public Context(IContext parent)
        {
            Parent = parent;
            Source = parent.Source;
            Identifier = GetHashCode() + "@" + Source.Identifier;
            _children = new List<IContext>();
        }

        public Character Source
        {
            get;
            private set;
        }

        /// <summary>
        /// Identifier of this Context.
        /// </summary>
        public string Identifier { get; protected set; }

        public IContext Parent { get; set; }

        /// <summary>
        /// This field stores the children Contexts of this Context instance.
        /// </summary>
        private readonly List<IContext> _children;

        public void AddChild(IContext child)
        {
            _children.Add(child);
        }

        public IEnumerable<IContext> Children
        {
            get { return _children; }
        }
    }
}
