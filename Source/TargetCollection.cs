using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Quark
{
    public class TargetCollection
    {
        List<Vector3> _points = new List<Vector3>();
        List<Targetable> _targetables = new List<Targetable>();
        List<Character> _characters = new List<Character>();

        public void Add(Vector3 point)
        {
            _points.Add(point);
        }

        public void Add(Targetable target)
        {
            _targetables.Add(target);
        }

        public void Add(Character target)
        {
            _characters.Add(target);
        }
            
        public Vector3[] Points
        {
            get
            {
                return _points.ToArray();
            }
        }

        public Targetable[] Targetables
        {
            get
            {
                return _targetables.ToArray();
            }
        }

        public Character[] Characters
        {
            get
            {
                return _characters.ToArray();
            }
        }

        public int Count
        {
            get
            {
                return _points.Count + _targetables.Count + _characters.Count;
            }
        }
    }
}

