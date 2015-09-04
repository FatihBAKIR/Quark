using System;
using Quark.Contexts;
using Quark.Spells;
using Quark.Utilities;
using UnityEngine;

namespace Quark.Projectiles
{
    /// <summary>
    /// ProjectileController is the class that controls the Projectile entities in a scene.
    /// 
    /// Their main responsibilities include:
    ///     + Calculating the initial point of a new Projectile object.
    ///     + Controlling the object every frame depending on its type of control. <see cref="ControlType"/>
    ///     + Determining whether the object has reached its final destination.
    ///     + Determining whether a collision is a valid hit or not in order to decide whether the hit logic of a Spell should be executed or not.
    /// 
    /// The reason this class is seperate from the actual <see cref="Projectile"/> class is because ideally, we are trying to keep 
    /// the physical existance of a Projectile and the logic that controls it seperated.
    /// </summary>
    public class ProjectileController
    {
        /// <summary>
        /// Context of this controller.
        /// </summary>
        //protected Cast Context { get; private set; }
        protected IProjectileContext Context { get; private set; }

        /// <summary>
        /// This method sets the context this controller runs in.
        /// </summary>
        /// <param name="context">The controller.</param>
        public virtual void SetContext(IProjectileContext context)
        {
            Context = context;
        }

        /// <summary>
        /// This method should calculate the initial position of the Projectile object.
        /// </summary>
        /// <param name="target">Target of the Projectile.</param>
        /// <returns>Initial position of Projectile.</returns
        public virtual Vector3 InitialPoint(TargetUnion target)
        {
            Vector3 point = Context.Source.transform.position + Context.Source.transform.forward;
            point += new Vector3(0, Context.Source.HeightOffset, 0);
            return point;
        }

        /// <summary>
        /// This method may perform any necessarry initialization logic with the Projectile.
        /// </summary>
        public virtual void Initialize()
        {

        }

        protected ControlType Type;

        /// <summary>
        /// This method controls the projectile object.
        /// 
        /// There are primarily 2 ways of controlling a projectile:
        ///     
        ///     - Calculating the change of position every frame.
        ///     - Calculating the absolute position every frame.
        /// 
        /// This method gets the appropriate property and updates the position of the Projectile by default.
        /// </summary>
        public virtual void Control()
        {
            Vector3 previous = Projectile.transform.position;
            switch (Type)
            {
                case ControlType.Movement:
                    Projectile.transform.Translate(CalculateMovement, Space.World);
                    break;
                case ControlType.Absolute:
                    Projectile.transform.position = 
                        Utils.Align2(CalculateAbsolute, Quaternion.Euler(Context.TravelBeginRotation) * Vector3.forward) 
                        + Context.TravelBeginPosition;
                    break;
            }
            Projectile.transform.rotation = Quaternion.LookRotation(Projectile.transform.position - previous);
        }

        /// <summary>
        /// This property should calculate the change of the position a Projectile.
        /// 
        /// <exception cref="NotImplementedException">If not implemented.</exception>
        /// </summary>
        protected virtual Vector3 CalculateMovement
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// This property should calculate the absolute position a Projectile should be at any given time.
        /// 
        /// <exception cref="NotImplementedException">If not implemented.</exception>
        /// </summary>
        protected virtual Vector3 CalculateAbsolute
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// This method should determine whether a hit to a Targetable object is valid or not.
        /// </summary>
        /// <param name="hitObject">The hit object.</param>
        /// <returns>Whether the hit is valid or not.</returns
        public virtual bool Validate(Targetable hitObject)
        {
            // By default, we don't let projectiles hit the caster.
            return !hitObject.Equals(Context.Source);
        }

        /// <summary>
        /// The Projectile object this Controller controls.
        /// </summary>
        protected Projectile Projectile { get; private set; }

        /// <summary>
        /// Sets the Projectile object of this Controller.
        /// </summary>
        /// <param name="projectile">The projectile.</param>
        public void SetProjectile(Projectile projectile)
        {
            Projectile = projectile;
            Initialize();
        }

        /// <summary>
        /// Calculated target point of this Controller.
        /// </summary>
        protected Vector3 TargetPoint
        {
            get { return Context.Target.AsPoint() + Context.TargetOffset; }
        }

        /// <summary>
        /// This method changes the target of this Controller instance and the Projectile that's bound with it.
        /// </summary>
        /// <param name="newTarget">The new target.</param>
        public virtual void ChangeTarget(TargetUnion newTarget)
        {
            Context.Target = newTarget;
        }

        /// <summary>
        /// This property determines whether the Projectile reached it its destination, and is ready for destruction.
        /// </summary>
        public virtual bool Finished
        {
            get
            {
                return Utils.Distance2(Projectile.transform.position, TargetPoint) <= Projectile.NearEnough;
            }
        }
        
        /// <summary>
        /// This method is called when this projectile misses.
        /// </summary>
        public virtual void OnMiss()
        {}

        /// <summary>
        /// This method is called when this projectile hits a target.
        /// </summary>
        /// <param name="hit">Hit context.</param>
        public virtual void OnHit(IHitContext hit)
        {}

        /// <summary>
        /// Gets the time in seconds since the projectile was created.
        /// </summary>
        /// <value>Travel time in seconds.</value>
        protected float TravelTime
        {
            get { return Context.TravelTime; }
        }

        /// <summary>
        /// Gets the distance from the beginning position to the current projectile position.
        /// </summary>
        /// <value>The distance.</value>
        protected float TravelDistance
        {
            get { return Context.TravelDistance; }
        }
    }

    /// <summary>
    /// This enumeration represents how a ProjectileController calculates motion of a Projectile.
    /// </summary>
    public enum ControlType
    {
        /// <summary>
        /// Projectile calculates change from the previous position of the Projectile every frame.
        /// </summary>
        Movement,
        /// <summary>
        /// Projectile calculates absolute position relative to the beginning position.
        /// </summary>
        Absolute
    }
}
