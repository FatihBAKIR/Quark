using System.Collections.Generic;
using UnityEngine;
using Quark.Targeting;

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

        public Vector3 FirstPoint
        {
            get
            {
                return _points[0];
            }
        }

        public Targetable FirstTargetable
        {
            get
            {
                return _targetables[0];
            }
        }

        public Character FirstCharacter
        {
            get
            {
                return _characters[0];
            }
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

        public bool IsEmpty
        {
            get
            {
                return Count < 1;
            }
        }
    }

    public struct TargetUnion
    {
        public TargetType Type { get; private set; }
        public Character Character { get; private set; }
        public Targetable Targetable { get; private set; }
        public Vector3 Point  { get; private set; }

        public TargetUnion(Vector3 point) : this()
        {
            Point = point;
            Type = TargetType.Point;
        }

        public TargetUnion(Character character) : this()
        {
            Character = character;
            Type = TargetType.Character;
        }

        public TargetUnion(Targetable targetable) : this()
        {
            Targetable = targetable;
            Type = TargetType.Targetable;
        }
    }
}

