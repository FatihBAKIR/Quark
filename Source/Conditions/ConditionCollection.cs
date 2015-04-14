using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Quark.Utilities;

namespace Quark
{
    public class ConditionCollection: Condition, IEnumerable<Condition>, IDeepCopiable<ConditionCollection>, IDisposable
    {
        private List<Condition> _conditions;

        /// <summary>
        /// Initialize a new condition collection
        /// </summary>
        public ConditionCollection()
        {
            _conditions = new List<Condition>();
        }

        ~ConditionCollection()
        {
            Dispose();
        }

        public void Dispose()
        {
            foreach (Condition condition in _conditions)
                condition.SetContext(null);
            _conditions.Clear();
            _conditions = null;
        }

        public ConditionCollection DeepCopy()
        {
            ConditionCollection newCollection = new ConditionCollection();
            newCollection._conditions = new List<Condition>(_conditions);
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
        public void Add(Condition condition)
        {
            _conditions.Add(condition);
        }

        public IEnumerator<Condition> GetEnumerator()
        {
            return _conditions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _conditions.GetEnumerator();
        }

        public override bool Check()
        {
            foreach (Condition condition in _conditions)
            {
                condition.SetContext(Context);
                if (!condition.Check())
                    return false;
            }
            return true;
        }

        public override bool Check(Character character)
        {
            foreach (Condition condition in _conditions)
            {
                condition.SetContext(Context);
                if (!condition.Check(character))
                    return false;
            }
            return true;
        }

        public override bool Check(Targetable target)
        {
            foreach (Condition condition in _conditions)
            {
                condition.SetContext(Context);
                if (!condition.Check(target))
                    return false;
            }
            return true;
        }

        public override bool Check(Vector3 point)
        {
            foreach (Condition condition in _conditions)
            {
                condition.SetContext(Context);
                if (!condition.Check(point))
                    return false;
            }
            return true;
        }
    }
}

