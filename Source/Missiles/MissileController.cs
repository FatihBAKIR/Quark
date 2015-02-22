using Quark.Utilities;
using UnityEngine;

namespace Quark.Missiles
{
    public class MissileController
    {
        /// <summary>
        /// Gets the position update type for the missile
        /// </summary>
        /// <value>Update type.</value>
        public virtual MovementType Type { get { return MovementType.ReturnsMovement; } }

        protected Missile obj { get; set; }

        /// <summary>
        /// Controls the missile object's position
        /// </summary>
        public virtual void Control()
        {
            switch (this.Type)
            {
                case MovementType.ReturnsMovement:
                    obj.transform.Translate(Movement);
                    break;
                case MovementType.ReturnsPosition:
                    obj.transform.position = Utils.RotateVector(Position, obj.CastRotation) + InitialPosition;
                    break;
            }
            obj.transform.LookAt(Target);
        }

        public void Set(Missile obj)
        {
            this.obj = obj;
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
                return this.obj.transform.position;
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
                return this.obj.Target;
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
                return this.obj.InitPosition;
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
                return Time.timeSinceLevelLoad - this.obj.InitTime;
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
                return Vector3.Distance(this.obj.transform.position, this.obj.InitPosition);
            }
        }
    }

    public enum MovementType
    {
        ReturnsMovement,
        ReturnsPosition
    }
}