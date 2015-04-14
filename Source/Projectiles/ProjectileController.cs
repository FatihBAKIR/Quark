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
        public virtual MovementType Type { get { return MovementType.ReturnsMovement; } }

        protected Projectile Obj { get; set; }

        /// <summary>
        /// Controls the missile object's position
        /// </summary>
        public virtual void Control()
        {
            switch (Type)
            {
                case MovementType.ReturnsMovement:
                    Obj.transform.Translate(Movement);
                    break;
                case MovementType.ReturnsPosition:
                    Obj.transform.position = Utils.RotateVector(Position, Obj.CastRotation) + InitialPosition;
                    break;
            }
            //Obj.transform.LookAt(Target);
        }

        public virtual bool HasReached()
        {
            return Utils.Distance2(Obj.transform.position, Target) <= Projectile.NearEnough;
        }

        public void Set(Projectile obj)
        {
            Obj = obj;
        }

        /// <summary>
        /// Calculates the movement vector for current frame
        /// </summary>
        /// <value>The movement vector</value>
        public virtual Vector3 Movement
        {
            get
            {
                return Vector3.zero;
            }
        }

        /// <summary>
        /// Calculates the position chance
        /// </summary>
        /// <value>The position difference from the initial position</value>
        public virtual Vector3 Position
        {
            get
            {
                return Vector3.zero;
            }
        }

        /// <summary>
        /// Gets current missile position
        /// </summary>
        /// <value>My position.</value>
        protected Vector3 MyPos
        {
            get
            {
                return Obj.transform.position;
            }
        }

        /// <summary>
        /// Gets the target position of the missile
        /// </summary>
        /// <value>The target.</value>
        protected Vector3 Target
        {
            get
            {
                return Obj.Target;
            }
        }

        /// <summary>
        /// Gets the initial position of the missile
        /// </summary>
        /// <value>The initial position.</value>
        protected Vector3 InitialPosition
        {
            get
            {
                return Obj.InitPosition;
            }
        }

        /// <summary>
        /// Gets the time in seconds since the missile was created
        /// </summary>
        /// <value>The alive.</value>
        protected float Alive
        {
            get
            {
                return Time.timeSinceLevelLoad - Obj.InitTime;
            }
        }

        /// <summary>
        /// Gets the distance from the beginning position to the current missile position 
        /// </summary>
        /// <value>The distance.</value>
        protected float Distance
        {
            get
            {
                return Vector3.Distance(Obj.transform.position, Obj.InitPosition);
            }
        }
    }

    public enum MovementType
    {
        ReturnsMovement,
        ReturnsPosition
    }
}