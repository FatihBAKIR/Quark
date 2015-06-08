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

        /// <summary>
        /// Rotation of the caster Character when the projectile stage began.
        /// </summary>
        public Vector3 CastRotation;

        ProjectileController _controller;

        Vector3 _yOffset;

        Vector3 _targetPosition;
        Targetable _target;
        bool _toPos;

        /// <summary>
        /// The context this Projectile travels in.
        /// </summary>
        public Cast Context { get; private set; }

        /// <summary>
        /// Gets the initial position for this missile.
        /// </summary>
        /// <value>The initial position.</value>
        public Vector3 InitPosition { get; private set; }

        /// <summary>
        /// Current target point of this Projectile instance.
        /// 
        /// <remarks>The target may update.</remarks>
        /// </summary>
        public Vector3 Target
        {
            get
            {
                return _toPos ? _targetPosition : (_target.transform.position + _yOffset);
            }
        }

        /// <summary>
        /// The time in seconds that this Projectile instance was created.
        /// </summary>
        public float InitialTime { get; private set; }

        bool HasReached
        {
            get { return _controller.HasReached(); }
        }

        #region Initialization

        /// <summary>
        /// This function creates a new projectile object from a prefab and a controller in a context towards a target.
        /// 
        /// The created projectile will immediately start travelling towars its target.
        /// </summary>
        /// <param name="prefab">The prefab of a Projectile. The prefab shouldn't contain a Projectile component.</param>
        /// <param name="controller">A controller object to control the Projectile instance.</param>
        /// <param name="context">Context to create the Projectile instance in.</param>
        /// <param name="target">Target of the Projectile instance.</param>
        /// <returns>The new Projectile instance.</returns>
        public static Projectile Make(GameObject prefab, ProjectileController controller, Cast context, TargetUnion target)
        {
            GameObject obj = (GameObject)Instantiate(prefab, controller.CalculateInitial(target, context), Quaternion.identity);
            Projectile m = obj.AddComponent<Projectile>();
            m.Context = context;
            m._controller = controller;
            m.SetTarget(target);
            m._controller.Set(m);
            return m;
        }

        internal void SetTarget(TargetUnion target)
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
            CastRotation = target - Context.CastBeginPoint;
            _toPos = true;
            _targetPosition = target;
        }

        void Set(Character target)
        {
            Set((Targetable)target);
        }

        void Set(Targetable target)
        {
            CastRotation = target.transform.position - Context.CastBeginPoint;
            _toPos = false;
            _target = target;
            _yOffset = new Vector3(0, _target.HeightOffset, 0);
        }

        #endregion

        void Start()
        {
            InitPosition = transform.position;
            InitialTime = Time.timeSinceLevelLoad;
        }

        void OnTriggerEnter(Collider c)
        {
            Targetable hit = c.gameObject.GetComponent<Targetable>();

            if (IsHitValid(hit))
            {
                Context.Spell.OnHit(hit);

                if ((!_toPos && hit.Equals(_target)) || (Context.Spell.TargetForm == TargetForm.Singular))
                {
                    Context.Spell.CollectProjectile(this);
                    Destroy(gameObject);
                }
            }

            Logger.Debug("Collision: " + c.gameObject.name + "\nTarget Was" + (hit == null ? " Not" : "") + " A Targetable");
        }

        bool IsHitValid(Targetable hit)
        {
            bool result = hit != null && hit.IsTargetable && _controller.ValidateHit(hit);

            if (result)
                Context.HitCount++;
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
                Context.Spell.OnHit(_targetPosition);
                Context.Spell.CollectProjectile(this);
                Destroy(gameObject);
                return;
            }

            if (Utils.Distance2(transform.position, _lastTravel) >= Context.Spell.TravelingInterval)
            {
                _lastTravel = transform.position;
                Context.Spell.OnTravel(transform.position);
            }
        }
    }
}