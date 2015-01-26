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
        Character _targetCharacter;
        CastData _data;

        MissileController _controller;

        uint HitCount
        {
            get
            {
                return _data.HitCount;
            }
            set
            {
                _data.HitCount = value;
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
                return ToPos ? this._targetPosition : this._targetCharacter.transform.position;
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

        public static Missile Make(GameObject prefab, MissileController controller, CastData data)
        {
            GameObject obj = (GameObject)MonoBehaviour.Instantiate(prefab, data.CastBeginPoint, Quaternion.identity);
            Missile m = obj.AddComponent<Missile>();
            m._data = data;
            m._controller = controller;
            m._controller.Set(m);
            return m;
        }

        bool ToPos = false;

        public void Set(Vector3 target)
        {
            this.CastRotation = target - _data.CastBeginPoint;
            this.ToPos = true;
            this._targetPosition = target;
        }

        public void Set(Character target)
        {
            this.CastRotation = target.transform.position - _data.CastBeginPoint;
            this._targetCharacter = target;
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
                _data.Spell.OnHit(hit);

                if ((!ToPos && hit.Equals(this._targetCharacter)) || (this._data.Spell.TargetForm == TargetForm.Singular))
                {
                    _data.Spell.CollectProjectile(this);
                    Destroy(this.gameObject);
                }
            }
            Logger.Debug("Hit: " + c.gameObject.name + "\nTarget Was" + (hit == null ? " Not" : "") + " A Character");
        }

        protected virtual bool IsHitValid(Character hit)
        {
            bool result = hit != null && !hit.Equals(_data.Caster) && hit.IsTargetable;
            if (result)
                HitCount++;
            return result;
        }

        void Update()
        {
            if (!ToPos)
                _targetPosition = _targetCharacter.transform.position;
        
            _controller.Control();
            _data.Spell.OnTravel(transform.position);

            if (ToPos && HasReached)
            {
                _data.Spell.OnHit(_targetPosition);
                _data.Spell.CollectProjectile(this);
                Destroy(gameObject);
            }
        }
    }
}