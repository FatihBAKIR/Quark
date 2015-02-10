using Quark.Spell;
using Quark.Targeting;
using Quark.Utilities;
using UnityEngine;

namespace Quark.Missile
{
    /// <summary>
    /// Missile class provides interface for MissileController objects to access to properties about the projectile
    /// It also retrieves necessary movement vector or position vector and moves the carrier object appropriately
    /// It is also responsible for handling the collisions and target checks
    /// </summary>
    public class Missile : MonoBehaviour
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

        MissileController _controller;

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
                return this._initialPosition;
            }
        }

        public Vector3 Target
        {
            get
            {
                return ToPos ? this._targetPosition : this._target.transform.position;
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
                return this._initialTime;
            }
        }

        bool HasReached
        {
            get
            {
                Vector3 a = transform.position, b = _targetPosition;
                a.y = 0;
                b.y = 0;
                return Vector3.Distance(a, b) <= NearEnough;
            }   
        }

        #region Initialization

        public static Missile Make(GameObject prefab, MissileController controller, Cast context)
        {
            GameObject obj = (GameObject)MonoBehaviour.Instantiate(prefab, context.CastBeginPoint, Quaternion.identity);
            Missile m = obj.AddComponent<Missile>();
            m._context = context;
            m._controller = controller;
            m._controller.Set(m);
            return m;
        }

        bool ToPos = false;

        public void Set(Vector3 target)
        {
            this.CastRotation = target - _context.CastBeginPoint;
            this.ToPos = true;
            this._targetPosition = target;
        }

        public void Set(Character target)
        {
            this.CastRotation = target.transform.position - _context.CastBeginPoint;
            this._target = target;
        }

        public void Set(Targetable target)
        {
            this.CastRotation = target.transform.position - _context.CastBeginPoint;
            this._target = target;
        }

        #endregion

        void Start()
        {
            _initialPosition = this.transform.position;
            _initialTime = Time.timeSinceLevelLoad;
        }

        void OnTriggerEnter(Collider c)
        {
            Character hit = c.gameObject.GetComponent<Character>();
            if (IsHitValid(hit))
            {
                _context.Spell.OnHit(hit);

                if ((!ToPos && hit.Equals(this._target)) || (this._context.Spell.TargetForm == TargetForm.Singular))
                {
                    _context.Spell.CollectProjectile(this);
                    Destroy(this.gameObject);
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

        void Update()
        {
            if (!ToPos)
                _targetPosition = _target.transform.position;
        
            _controller.Control();
            _context.Spell.OnTravel(transform.position);

            if (ToPos && HasReached)
            {
                _context.Spell.OnHit(_targetPosition);
                _context.Spell.CollectProjectile(this);
                Destroy(gameObject);
            }
        }
    }
}