using System;
using System.Collections.Generic;
using System.Collections;
using Quark.Attributes;

namespace Quark
{
    public enum Source
    {
        Caster,
        Target
    }

    /// <summary>
    /// This class is the building block of expressions.
    /// 
    /// It can be an interaction, constant or other custom calculation such as another expression.
    /// </summary>
    public abstract class Symbol
    {
        public abstract float Get (Character caster, Character target);
    }

    /// <summary>
    /// This class represents only a constant value.
    /// </summary>
    public class ConstantSymbol : Symbol
    {
        float _value;

        public ConstantSymbol(float value)
        {
            _value = value;
        }

        public override float Get (Character caster, Character target)
        {
            return _value;
        }
    }

    /// <summary>
    /// This class represents an attribute interaction of a given source.
    /// 
    /// It depends either on the caster or target depending on the source, 
    /// and the value it yields is the interaction result.
    /// </summary>
    public class InteractionSymbol : Symbol
    {
        Source _source;
        string _tag;
        float _multiplier;

        public InteractionSymbol(Source source, string tag, float multiplier)
        {
            _source = source;
            _tag = tag;
            _multiplier = multiplier;
        }

        public override float Get (Character caster, Character target)
        {
            return new AttributeModifier(_tag, _multiplier).GetValue(_source == Source.Caster ? caster : target);
        }
    }

    /// <summary>
    /// This class intends to provide a modular to create, store and execute numeric calculations just before they are needed.
    /// 
    /// An expression is a combination of arbitrary symbols like a constant, an Attribute Interaction, another expression or another custom calculation.
    /// 
    /// Expressions implement IEnumerable<T> interface, therefore they support collection initializers.
    /// </summary>
    public class Expression : IEnumerable<Symbol>
    {
        List<Symbol> _symbols = new List<Symbol>();
        public Expression ()
        {
        }

        /// <summary>
        /// Adds a constant symbol to this expression.
        /// </summary>
        /// <param name="constant">Constant.</param>
        public void Add(float constant)
        {
            _symbols.Add (new ConstantSymbol (constant));
        }

        /// <summary>
        /// Adds an arbitrary symbol to this expression.
        /// </summary>
        /// <param name="symbol">Symbol.</param>
        public void Add (Symbol symbol)
        {
            _symbols.Add (symbol);
        }

        /// <summary>
        /// Adds an attribute interaction to this expression.
        /// </summary>
        /// <param name="source">Origin character.</param>
        /// <param name="tag">Attribute or stat tag.</param>
        /// <param name="multiplier">Multiplier.</param>
        public void Add(Source source, string tag, float multiplier)
        {
            _symbols.Add (new InteractionSymbol(source, tag, multiplier));
        }

        /// <summary>
        /// Executes the expression and returns the calculated value.
        /// </summary>
        /// <param name="caster">Caster.</param>
        /// <param name="target">Target.</param>
        public float Execute(Character caster, Character target)
        {
            float val = 0;
            foreach (Symbol symbol in _symbols) 
            {
                val += symbol.Get (caster, target);     
            }
            return val;
        }

        public IEnumerator<Symbol> GetEnumerator()
        {
            return (IEnumerator<Symbol>)_symbols.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _symbols.GetEnumerator();
        }
    }

}

