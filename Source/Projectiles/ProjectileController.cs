﻿using Quark.Spells;
using Quark.Targeting;
using Quark.Utilities;
using UnityEngine;

namespace Quark.Projectiles
{
    public class ProjectileController
    {
        /// <summary>
        /// Gets the position update type for the missile
        /// </summary>
        /// <value>Update type.</value>
        protected virtual ControlType Type { get { return ControlType.Movement; } }

        /// <summary>
        /// The Projectile component this controller belongs to.
        /// </summary>
        protected Projectile Projectile { get; set; }

        /// <summary>
        /// This method should calculate the initial position of the Projectile object.
        /// </summary>
        /// <param name="target">Target of the Projectile.</param>
        /// <param name="context">Context to calculate in.</param>
        /// <returns>Initial position of Projectile.</returns>
        public virtual Vector3 CalculateInitial(TargetUnion target, Cast context)
        {
            Vector3 point = context.CastBeginPoint;
            point += Utils.VectorOnPlane(context.Caster.transform.forward, Planes.XZ) + new Vector3(0, 1, 0);
            return point;
        }

        /// <summary>
        /// This field stores the movement speed of the Projectile object.
        /// </summary>
        public float Speed;

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
            switch (Type)
            {
                case ControlType.Movement:
                    Projectile.transform.Translate(Movement);
                    break;
                case ControlType.Absolute:
                    Projectile.transform.position = Utils.AlignVector(Position, Projectile.CastRotation) + InitialPosition;
                    break;
            }
        }

        /// <summary>
        /// This method determines whether the projectile arrived it its destination,
        /// and is ready for destruction.
        /// </summary>
        /// <returns>Whether the projectile should be destroyed.</returns>
        public virtual bool HasReached()
        {
            return Utils.Distance2(Projectile.transform.position, Target.AsPoint()) <= Projectile.NearEnough;
        }

        /// <summary>
        /// SetProjectile the Projectile object of this ProjectileController.
        /// </summary>
        /// <param name="obj">The Projectile object.</param>
        public void SetProjectile(Projectile obj)
        {
            Projectile = obj;
            OnProjectileSet();
        }

        public virtual void OnProjectileSet()
        {            
        }

        /// <summary>
        /// Calculates the change in the position of the projectile in this frame.
        /// </summary>
        /// <value>The movement vector.</value>
        protected virtual Vector3 Movement
        {
            get
            {
                return Vector3.zero;
            }
        }

        /// <summary>
        /// Calculates the absolute position of the projectile in this frame.
        /// </summary>
        /// <value>The absolute position, relative to the starting point.</value>
        protected virtual Vector3 Position
        {
            get
            {
                return Vector3.zero;
            }
        }

        /// <summary>
        /// Gets current projectile position
        /// </summary>
        /// <value>Current position.</value>
        protected Vector3 CurrentPosition
        {
            get
            {
                return Projectile.transform.position;
            }
        }

        /// <summary>
        /// Gets the target position of the projectile.
        /// </summary>
        /// <value>The target.</value>
        protected Vector3 TargetPoint
        {
            get
            {
                return Target.Type == TargetType.Point ? Target.Point : (Target.AsPoint() + Projectile.TargetOffset);
            }
        }

        /// <summary>
        /// Target of this Controller.
        /// </summary>
        protected TargetUnion Target
        {
            get { return Projectile.Target; }
        }

        /// <summary>
        /// Gets the initial position of the projectile.
        /// </summary>
        /// <value>The initial position.</value>
        protected Vector3 InitialPosition
        {
            get
            {
                return Projectile.InitialPosition;
            }
        }

        /// <summary>
        /// Gets the time in seconds since the projectile was created.
        /// </summary>
        /// <value>The alive.</value>
        protected float AliveSeconds
        {
            get
            {
                return Time.timeSinceLevelLoad - Projectile.InitialTime;
            }
        }

        /// <summary>
        /// Gets the distance from the beginning position to the current projectile position.
        /// </summary>
        /// <value>The distance.</value>
        protected float TravelAmount
        {
            get
            {
                return Vector3.Distance(Projectile.transform.position, Projectile.InitialPosition);
            }
        }

        /// <summary>
        /// The context this ProjectileController works in.
        /// </summary>
        protected Cast Context
        {
            get
            {
                return Projectile.Context;
            }
        }

        /// <summary>
        /// This method should determine whether a hit to a Targetable object is valid or not.
        /// </summary>
        /// <param name="target">The hit object.</param>
        /// <returns>Whether the hit is valid or not.</returns>
        public virtual bool ValidateHit(Targetable target)
        {
            // By default, don't let projectiles hit the caster.
            return !target.Equals(Context.Caster);
        }

        /// <summary>
        /// This method changes the target of this Controller instance and the Projectile that's bound with it.
        /// </summary>
        /// <param name="target">The new target.</param>
        public virtual void ChangeTarget(TargetUnion target)
        {
            Projectile.SetTarget(target); 
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