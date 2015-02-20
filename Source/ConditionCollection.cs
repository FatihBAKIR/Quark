using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Quark
{
    public class ConditionCollection: Condition, IEnumerable<Condition>
    {
        private List<Condition> _conditions;

        /// <summary>
        /// Initialize a new condition collection
        /// </summary>
        public ConditionCollection()
        {
            _conditions = new List<Condition>();
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

