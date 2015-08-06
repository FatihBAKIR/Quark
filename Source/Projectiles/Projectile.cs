using System.Collections.Generic;
using Quark.Contexts;
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
        /// The Controller of this projectile object.
        /// </summary>
        public ProjectileController Controller { get; private set; }

        /// <summary>
        /// The Context of this projectile.
        /// </summary>
        public IProjectileContext Context { get; set; }

        /// <summary>
        /// This property calculates the latest position change of this Projectile.
        /// </summary>
        public Vector3 LastMovement
        {
            get { return transform.position - _previousPosition; }
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
        public static Projectile Make(GameObject prefab, ProjectileController controller, ICastContext context, TargetUnion target)
        {
            GameObject instantiatedObject = (GameObject)Instantiate(prefab, Vector3.zero, Quaternion.identity);
            Projectile projectileComponent = instantiatedObject.AddComponent<Projectile>();
            ProjectileContext pContext = new ProjectileContext(context, projectileComponent, target);

            controller.SetContext(pContext);

            instantiatedObject.transform.position = controller.InitialPoint(target);

            projectileComponent.Context = pContext;
            projectileComponent.Controller = controller;
            projectileComponent.Controller.SetProjectile(projectileComponent);

            return projectileComponent;
        }

        #endregion

        private Dictionary<int, bool> _collided;

        void FixedUpdate()
        {
            _collided = new Dictionary<int, bool>();
        }

        void OnTriggerEnter(Collider c)
        {
            // Unity calls OnTriggerEnter for every collider on the hit object.
            // We store the hit objects in a dictionary every frame to 
            // mistakenly processing one hit more than once.
            if (_collided.ContainsKey(c.gameObject.GetInstanceID()))
                return;

            Targetable hit = c.gameObject.GetComponent<Targetable>();

            if (hit == null) // Hit object wasn't a Quark object.
                return;

            if (hit is Character)
                Context.OnHit(new TargetUnion(hit as Character));
            else
                Context.OnHit(new TargetUnion(hit));

            _collided.Add(c.gameObject.GetInstanceID(), true);
        }

        /// <summary>
        /// This field stores the position of the Projectile when the OnTravel event was raised the last time.
        /// </summary>
        private Vector3 _lastTravel;

        /// <summary>
        /// This field stores the position of the Projectile before the controller is called this frame.
        /// 
        /// Used for determining the last movement vector upon hitting.
        /// </summary>
        private Vector3 _previousPosition;

        void Update()
        {
            _previousPosition = transform.position;

            Controller.Control();

            if (Context.Target.Type == TargetType.Point && Controller.Finished)
            {
                Context.OnHit(Context.Target);
                return;
            }

            if (Utils.Distance2(transform.position, _lastTravel) >= Context.Spell.TravelingInterval)
            {
                _lastTravel = transform.position;
                Context.OnTravel();
            }
        }
    }
}