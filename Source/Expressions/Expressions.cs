using System;
using Quark.Contexts;
using UnityEngine;

namespace Quark.Expressions
{
    /// <summary>
    /// A constant symbol for expressions
    /// </summary>
    public class Constant : Expression<IContext>
    {
        private readonly float _const;

        /// <summary>
        /// Initializes a new constant expression instance
        /// </summary>
        /// <param name="constant">Constant value</param>
        public Constant(float constant)
        {
            _const = constant;
        }

        public override float Calculate()
        {
            return _const;
        }

        public override float Calculate(Character target)
        {
            return Calculate();
        }

        public override float Calculate(Targetable target)
        {
            return Calculate();
        }

        public override float Calculate(Vector3 point)
        {
            return Calculate();
        }
    }

    class BinaryOp<T> : Expression<T> where T : IContext
    {
        private readonly IExpression<T> _exp1;
        private readonly IExpression<T> _exp2;
        private readonly Func<float, float, float> _func;

        public BinaryOp(IExpression<T> exp1, IExpression<T> exp2, Func<float, float, float> functor)
        {
            _exp1 = exp1;
            _exp2 = exp2;
            _func = functor;
        }

        public override void SetContext(IContext context)
        {
            base.SetContext(context);
            _exp1.SetContext(context);
            _exp2.SetContext(context);
        }

        public override float Calculate()
        {
            return _func(_exp1.Calculate(), _exp2.Calculate());
        }

        public override float Calculate(Vector3 point)
        {
            return _func(_exp1.Calculate(point), _exp2.Calculate(point));
        }

        public override float Calculate(Targetable target)
        {
            return _func(_exp1.Calculate(target), _exp2.Calculate(target));
        }

        public override float Calculate(Character target)
        {
            return _func(_exp1.Calculate(target), _exp2.Calculate(target));
        }
    }
}
