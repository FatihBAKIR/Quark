using System;
using System.Collections.Generic;
using System.Collections;
using Quark.Conditions;
using Quark.Contexts;
using UnityEngine;
using Quark.Utilities;

namespace Quark
{
    public class ConditionCollection<T> : Condition<T>, IEnumerable<ICondition>, IDeepCopiable<ConditionCollection<T>>, IDisposable where T : class, IContext
    {
        private List<ICondition> _conditions;

        /// <summary>
        /// Initialize a new condition collection
        /// </summary>
        public ConditionCollection()
        {
            _conditions = new List<ICondition>();
        }

        ~ConditionCollection()
        {
            Dispose();
        }

        public void Dispose()
        {
            foreach (ICondition condition in _conditions)
                condition.SetContext(null);
            _conditions.Clear();
            _conditions = null;
        }

        public ConditionCollection<T> DeepCopy()
        {
            ConditionCollection<T> newCollection = new ConditionCollection<T>();
            newCollection._conditions = new List<ICondition>(_conditions);
            return newCollection;
        }

        object IDeepCopiable.DeepCopy()
        {
            return DeepCopy();
        }

        /// <summary>
        /// Add a new condition to this collection
        /// </summary>
        /// <param name="condition">The condition to be added</param>
        public void Add(ICondition<T> condition)
        {
            _conditions.Add(condition);
        }

        public IEnumerator<ICondition> GetEnumerator()
        {
            return _conditions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _conditions.GetEnumerator();
        }

        public override bool Check()
        {
            foreach (ICondition condition in _conditions)
            {
                condition.SetContext(Context);
                if (!condition.Check())
                    return false;
            }
            return true;
        }

        public override bool Check(Character character)
        {
            foreach (ICondition condition in _conditions)
            {
                condition.SetContext(Context);
                if (!condition.Check(character))
                    return false;
            }
            return true;
        }

        public override bool Check(Targetable target)
        {
            foreach (ICondition condition in _conditions)
            {
                condition.SetContext(Context);
                if (!condition.Check(target))
                    return false;
            }
            return true;
        }

        public override bool Check(Vector3 point)
        {
            foreach (ICondition condition in _conditions)
            {
                condition.SetContext(Context);
                if (!condition.Check(point))
                    return false;
            }
            return true;
        }
    }
}

