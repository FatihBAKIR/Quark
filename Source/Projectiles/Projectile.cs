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
    public sealed class Projectile : MonoBehaviour
    {
        /// <summary>
        /// The near enough distance constant which indicates that a missile will consider itself reached to a target point.
        /// </summary>
        public static float NearEnough = 0.1F;

        /// <summary>
        /// Rotation of the caster Character when the projectile stage began.
        /// </summary>
        public Vector3 CastRotation;

        /// <summary>
        /// The Controller of this projectile object.
        /// </summary>
        public ProjectileController Controller { get; private set; }

        /// <summary>
        /// This field stores the Y axis (ie. height) offset to properly hit the target in a proper point.
        /// </summary>
        public Vector3 TargetOffset;

        /// <summary>
        /// The context this Projectile travels in.
        /// </summary>
        public Cast Context { get; private set; }

        /// <summary>
        /// Gets the initial position for this missile.
        /// </summary>
        /// <value>The initial position.</value>
        public Vector3 InitialPosition { get; private set; }

        /// <summary>
        /// Current target point of this Projectile instance.
        /// 
        /// <remarks>The target may change over time.</remarks>
        /// </summary>
        public TargetUnion Target { get; private set; }

        /// <summary>
        /// The time in seconds that this Projectile instance was created.
        /// </summary>
        public float InitialTime { get; private set; }

        bool HasReached
        {
            get { return Controller.Finished; }
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
            controller.SetContext(context);
            Vector3 initialPoint = controller.InitialPoint(target);

            GameObject instantiatedObject = (GameObject)Instantiate(prefab, initialPoint, Quaternion.identity);

            Projectile projectileComponent = instantiatedObject.AddComponent<Projectile>();
            projectileComponent.Context = context;
            projectileComponent.Controller = controller;
            projectileComponent.SetTarget(target);

            projectileComponent.Controller.SetProjectile(projectileComponent);

            return projectileComponent;
        }

        internal void SetTarget(TargetUnion target)
        {
            Target = target;
            switch (target.Type)
            {
                case TargetType.Targetable:
                    TargetOffset = new Vector3(0, Target.Targetable.HeightOffset, 0);
                    break;
                case TargetType.Character:
                    TargetOffset = new Vector3(0, Target.Character.HeightOffset, 0);
                    break;
                default:
                    TargetOffset = Vector3.zero;
                    break;
            }
        }
        #endregion

        void Start()
        {
            InitialPosition = transform.position;
            InitialTime = Time.timeSinceLevelLoad;
        }

        void OnTriggerEnter(Collider c)
        {
            Targetable hit = c.gameObject.GetComponent<Targetable>();

            if (IsHitValid(hit))
            {
                if (hit is Character)
                    Context.Spell.OnHit(hit as Character);
                else
                    Context.Spell.OnHit(hit);

                if ((Target.Type != TargetType.Point && hit.Equals(Target.AsTargetable())) || (Context.Spell.TargetForm == TargetForm.Singular))
                {
                    Context.Spell.CollectProjectile(this);
                    Destroy(gameObject);
                }
            }
            Logger.Debug("Collision: " + c.gameObject.name + "\nTarget Was" + (hit == null ? " Not" : "") + " A Targetable");
        }

        bool IsHitValid(Targetable hit)
        {
            bool result = hit != null && hit.IsTargetable && Controller.Validate(hit);

            if (result)
                Context.HitCount++;
            return result;
        }

        private Vector3 _lastTravel;
        void Update()
        {
            Controller.Control();

            if (Target.Type == TargetType.Point && HasReached)
            {
                Context.Spell.OnHit(Target.AsPoint());
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