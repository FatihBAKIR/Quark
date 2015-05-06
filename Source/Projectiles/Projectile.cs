using Quark.Spells;
using Quark.Targeting;
using Quark.Utilities;
using UnityEngine;

namespace Quark.Projectiles
{
    /// <summary>
    /// Projectile class provides interface for MissileController objects to access to properties about the projectile
    /// It also retrieves necessary movement vector or position vector and moves the carrier object appropriately
    /// It is also responsible for handling the collisions and target checks
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        /// <summary>
        /// The near enough distance constant which indicates that a missile will consider itself reached to a target point
        /// </summary>
        public static float NearEnough = 0.1F;
        float _initialTime;
        Vector3 _initialPosition;
        Vector3 _targetPosition;
        Targetable _target;
        Cast _context;

        ProjectileController _controller;

        /// <summary>
        /// This property denotes the total hit count of this Spell Cast
        /// </summary>
        uint HitCount
        {
            get
            {
                return _context.HitCount;
            }
            set
            {
                _context.HitCount = value;
            }
        }

        /// <summary>
        /// Gets the initial position for this missile.
        /// </summary>
        /// <value>The initial position.</value>
        public Vector3 InitPosition
        {
            get
            {
                return _initialPosition;
            }
        }

        Vector3 HeightOffset
        {
            get { return new Vector3(0, _target.transform.localScale.y / 2, 0); }
        }

        public Vector3 Target
        {
            get
            {
                return _toPos ? _targetPosition : (_target.transform.position + HeightOffset);
            }
        }

        public Vector3 CastRotation
        {
            get;
            set;
        }

        public float InitTime
        {
            get
            {
                return _initialTime;
            }
        }

        bool HasReached
        {
            get { return _controller.HasReached(); }
        }

        #region Initialization

        public static Projectile Make(GameObject prefab, ProjectileController controller, Cast context, TargetUnion target)
        {
            GameObject obj = (GameObject)Instantiate(prefab, controller.CalculateInitial(target, context), Quaternion.identity);
            Projectile m = obj.AddComponent<Projectile>();
            m._context = context;
            m._controller = controller;
            m.SetTarget(target);
            m._controller.Set(m);
            return m;
        }

        bool _toPos;

        void SetTarget(TargetUnion target)
        {
            switch (target.Type)
            {
                case TargetType.Point:
                    Set(target.Point);
                    break;
                case TargetType.Targetable:
                    Set(target.Targetable);
                    break;
                case TargetType.Character:
                    Set(target.Character);
                    break;
            }
        }

        void Set(Vector3 target)
        {
            CastRotation = target - _context.CastBeginPoint;
            _toPos = true;
            _targetPosition = target;
        }

        void Set(Character target)
        {
            CastRotation = target.transform.position - _context.CastBeginPoint;
            _target = target;
        }

        void Set(Targetable target)
        {
            CastRotation = target.transform.position - _context.CastBeginPoint;
            _target = target;
        }

        #endregion

        void Start()
        {
            _initialPosition = transform.position;
            _initialTime = Time.timeSinceLevelLoad;
        }

        void OnTriggerEnter(Collider c)
        {
            Character hit = c.gameObject.GetComponent<Character>();
            if (IsHitValid(hit))
            {
                _context.Spell.OnHit(hit);

                if ((!_toPos && hit.Equals(_target)) || (_context.Spell.TargetForm == TargetForm.Singular))
                {
                    _context.Spell.CollectProjectile(this);
                    Destroy(gameObject);
                }
            }
            Logger.Debug("Hit: " + c.gameObject.name + "\nTarget Was" + (hit == null ? " Not" : "") + " A Character");
        }

        protected virtual bool IsHitValid(Character hit)
        {
            bool result = hit != null && !hit.Equals(_context.Caster) && hit.IsTargetable;
            if (result)
                HitCount++;
            return result;
        }

        private Vector3 _lastTravel;
        void Update()
        {
            if (!_toPos)
                _targetPosition = _target.transform.position;

            _controller.Control();

            if (_toPos && HasReached)
            {
                _context.Spell.OnHit(_targetPosition);
                _context.Spell.CollectProjectile(this);
                Destroy(gameObject);
                return;
            }

            if (Utils.Distance2(transform.position, _lastTravel) >= _context.Spell.TravelingInterval)
            {
                _lastTravel = transform.position;
                _context.Spell.OnTravel(transform.position);
            }
        }
    }
}