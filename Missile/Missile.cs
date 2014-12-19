using Quark.Spell;
using Quark.Targeting;
using Quark.Utilities;
using UnityEngine;

namespace Quark.Missile
{
    public class Missile : MonoBehaviour
    {
        const float NearEnough = 0.05F;
        float InitialTime;
        Vector3 InitialPosition;
        Vector3 TargetPosition;
        Character TargetCharacter;
        CastData Data;

        MissileController Controller;

        uint HitCount
        {
            get
            {
                return Data.HitCount;
            }
            set
            {
                Data.HitCount = value;
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
                return this.InitialPosition;
            }
        }

        public Vector3 Target
        {
            get
            {
                return ToPos ? this.TargetPosition : this.TargetCharacter.transform.position;
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
                return this.InitialTime;
            }
        }

        bool HasReached
        {
            get
            {
                return Vector3.Distance(this.transform.position, this.TargetPosition) <= NearEnough;
            }   
        }

        #region Initialization

        public static Missile Make(GameObject prefab, MissileController controller, CastData data)
        {
            GameObject obj = (GameObject)MonoBehaviour.Instantiate(prefab, data.CastBeginPoint, Quaternion.identity);
            Missile m = obj.AddComponent<Missile>();
            m.Data = data;
            m.Controller = controller;
            m.Controller.Set(m);
            return m;
        }

        bool ToPos = false;

        public void Set(Vector3 target)
        {
            this.CastRotation = target - Data.CastBeginPoint;
            this.ToPos = true;
            this.TargetPosition = target;
        }

        public void Set(Character target)
        {
            this.CastRotation = target.transform.position - Data.CastBeginPoint;
            this.TargetCharacter = target;
        }

        #endregion

        void Start()
        {
            this.InitialPosition = this.transform.position;
            this.InitialTime = Time.timeSinceLevelLoad;
        }

        void OnTriggerEnter(Collider c)
        {
            Character hit = c.gameObject.GetComponent<Character>();
            if (IsHitValid(hit))
            {
                Data.Spell.OnHit(hit);

                if ((!ToPos && hit.Equals(this.TargetCharacter)) || (this.Data.Spell.TargetForm == TargetForm.Singular))
                {
                    Data.Spell.CollectProjectile(this);
                    Destroy(this.gameObject);
                }
            }
            Logger.Debug("Hit: " + c.gameObject.name + "\nTarget Was" + (hit == null ? " Not" : "") + " A Character");
        }

        protected virtual bool IsHitValid(Character hit)
        {
            bool result = hit != null && !hit.Equals(Data.Caster);
            if (result)
                this.HitCount++;
            return result;
        }

        void Update()
        {
            if (!ToPos)
                this.TargetPosition = this.TargetCharacter.transform.position;
        
            Controller.Control();
            Data.Spell.OnTravel(this.transform.position);

            if (ToPos && this.HasReached)
            {
                Data.Spell.CollectProjectile(this);
                Destroy(this.gameObject);
                return;
            }
        }
    }
}