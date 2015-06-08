using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Quark.Targeting;

namespace Quark
{
    public class TargetCollection : IEnumerable<TargetUnion>
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

        private TargetUnion[] Targets
        {
            get
            {
                TargetUnion[] targets = new TargetUnion[_points.Count + _targetables.Count + _characters.Count];
                int i = 0;
                foreach (Vector3 point in _points)
                    targets[i++] = new TargetUnion(point);

                foreach (Targetable target in _targetables)
                    targets[i++] = new TargetUnion(target);

                foreach (Character character in _characters)
                    targets[i++] = new TargetUnion(character);
                return targets;
            }
        }

        public IEnumerator<TargetUnion> GetEnumerator()
        {
            return ((IEnumerable<TargetUnion>)Targets).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Targets.GetEnumerator();
        }

    }

    /// <summary>
    /// This structure is the generic target type used in Quark framework.
    /// </summary>
    public struct TargetUnion
    {
        /// <summary>
        /// Type of the target of this union.
        /// </summary>
        public TargetType Type { get; private set; }

        /// <summary>
        /// The target Character of this union.
        /// </summary>
        public Character Character { get; private set; }

        /// <summary>
        /// The target Targetable of this union.
        /// </summary>
        public Targetable Targetable { get; private set; }

        /// <summary>
        /// The target Point of this union.
        /// </summary>
        public Vector3 Point { get; private set; }

        /// <summary>
        /// Initializes a TargetUnion instance with a point target.
        /// </summary>
        /// <param name="point">Target point.</param>
        public TargetUnion(Vector3 point)
            : this()
        {
            Point = point;
            Type = TargetType.Point;
        }

        /// <summary>
        /// Initializes a TargetUnion instance with a character target.
        /// </summary>
        /// <param name="character">Target character.</param>
        public TargetUnion(Character character)
            : this()
        {
            Character = character;
            Type = TargetType.Character;
        }

        /// <summary>
        /// Initializes a TargetUnion instance with a targetable target.
        /// </summary>
        /// <param name="targetable">Target targetable.</param>
        public TargetUnion(Targetable targetable)
            : this()
        {
            Targetable = targetable;
            Type = TargetType.Targetable;
        }

        /// <summary>
        /// This method returns appropriate point from its target.
        /// </summary>
        /// <returns>Position of the target.</returns>
        public Vector3 AsPoint()
        {
            switch (Type)
            {
                case TargetType.Point:
                    return Point;
                case TargetType.Targetable:
                    return Targetable.transform.position;
                case TargetType.Character:
                    return Character.transform.position;
            }
            return Vector3.zero;
        }

        /// <summary>
        /// This method returns appropriate Targetable from its target.
        /// </summary>
        /// <returns>Targetable.</returns>
        public Targetable AsTargetable()
        {
            switch (Type)
            {
                case TargetType.Targetable:
                    return Targetable;
                case TargetType.Character:
                    return Character;
            }
            return null;
        }
    }
}

