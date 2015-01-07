using System.Collections.Generic;
using Quark.Exceptions;
using Quark.Targeting;
using Quark.Utilities;
using UnityEngine;

namespace Quark.Spell
{
    public class CastData
    {
        Character _caster;
        Vector3 _beginPoint;
        float _beginningTime;
        List<Targetable> _targetables;
        List<Character> _targetChars;
        List<Vector3> _targetPoints;
        Spell _spell;
        TargetMacro _macro;
        LifeStep _step = LifeStep.Null;

        public uint HitCount = 0;

        public CastData()
        {
            Logger.GC("CastData::ctor");
        }

        ~CastData()
        {
            _spell = null;
            _caster = null;
            Logger.GC("CastData::dtor");
        }

        /// <summary>
        /// Prepares a new instance of <see cref="CastData"/> with the specified caster and spell.
        /// </summary>
        /// <returns>
        /// A began CastData object.
        /// </returns>
        /// <param name='caster'>
        /// Caster Character.
        /// </param>
        /// <param name='spell'>
        /// The instance of the spell to be cast.
        /// </param>
        public static CastData PrepareCast(Character caster, Spell spell)
        {
            CastData data = new CastData();
            data._step = LifeStep.Null;
            data._caster = caster;
            data._spell = spell;
            Messenger<CastData>.Broadcast("Prepare", data);
            Messenger<CastData>.Broadcast(data.Spell.Name + ".Prepare", data);
            Messenger<CastData>.Broadcast(caster.Identifier() + "." + data.Spell.Name + ".Prepare", data);
            data._spell.SetData(ref data);
            Logger.Debug("CastData::PrepareCast");
            data.Begin();
            return data;
        }

        /// <summary>
        /// Gets the caster.
        /// </summary>
        /// <value>
        /// The caster of the spell.
        /// </value>
        public Character Caster
        {
            get
            {
                return _caster;
            }
        }

        /// <summary>
        /// Gets the cast time.
        /// </summary>
        /// <value>
        /// The time past since the beginning of the cast.
        /// </value>
        public float CastTime
        {
            get
            {
                return Time.timeSinceLevelLoad - _beginningTime;
            }
        }

        /// <summary>
        /// Gets the cast percentage.
        /// </summary>
        /// <value>
        /// The cast done percentage.
        /// </value>
        public int CastPercentage
        {
            get
            {
                return (int)(CastTime * 100 / _spell.CastDuration);
            }
        }

        public Targetable[] Targetables
        {
            get
            {
                return _targetables.ToArray();
            }
        }

        public Character[] TargetCharacters
        {
            get
            {
                return _targetChars.ToArray();
            }
        }

        public Vector3[] TargetPoints
        {
            get
            {
                return _targetPoints.ToArray();
            }
        }

        public Vector3 CastBeginPoint
        {
            get
            {
                return _beginPoint;
            }
        }

        public Spell Spell
        {
            get
            {
                return _spell;
            }
            set
            {
                if (_step != LifeStep.Null)
                    return;
                _spell = value;
            }
        }

        public bool CanAddTarget()
        {
            if (_step != LifeStep.Targeting)
                throw new NotTargetingException();

            if (_spell.TargetForm == TargetForm.Singular && (_targetChars.Count > 0 || _targetPoints.Count > 0))
                throw new SingularSpellException();

            return true;
        }

        public void AddTarget(Targetable targetable)
        {
            CanAddTarget();
            _targetables.Add(targetable);
        }

        public void AddTarget(Character character)
        {
            CanAddTarget();
            _targetChars.Add(character);
        }

        public void AddTarget(Vector3 point)
        {
            CanAddTarget();
            _targetPoints.Add(point);
        }

        /// <summary>
        /// Begin the casting logic.
        /// </summary>
        void Begin()
        {
            _step = LifeStep.Begin;
            Logger.Debug("CastData.Begin");
            if (!_caster.CanCast)
            {
                Messenger<CastError>.Broadcast("CastError", new BusyError());
                return;
            }
            _caster.AddCast(this);
            _spell.OnBegin();
            BeginTargeting();
        }

        /// <summary>
        /// Begin targeting logic
        /// </summary>
        void BeginTargeting()
        {
            _step = LifeStep.Targeting;
            _targetables = new List<Targetable>();
            _targetChars = new List<Character>();
            _targetPoints = new List<Vector3>();
            _macro = _spell.TargetMacro;
            _macro.SetData(this);
            _macro.Run();
        }

        public void TargetingFail()
        {
            _macro = null;
            Clear(LifeStep.Fail);
        }

        /// <summary>
        /// BeginTargeting done logic.
        /// </summary>
        public void TargetingDone()
        {
            /*
             * Copy the time and the point at the instant the targeting was done and check for the spell type, if it is an instant spell, immediately cast it
             * otherwise, begin the casting logic
             */
            _macro = null;
            this._beginningTime = Time.timeSinceLevelLoad;
            this._beginPoint = _caster.transform.position;
            _spell.OnTargetingDone();
            if (_spell.IsInstant)
                this.CastDone();
            else
                this.Cast();
        }

        void Cast()
        {
            this._step = LifeStep.Casting;
            Messenger.AddListener("Update", this.Update);
            _spell.OnCastingBegan();
        }

        void controlCast()
        {
            this._step = LifeStep.Casting;
            if (CastPercentage >= 100)
                this.CastDone();
        }

        void CastDone()
        {
            Clear(LifeStep.Done);
            _spell.OnCastDone();
        }

        void Update()
        {
            controlCast();
        }

        void Clear(LifeStep step)
        {
            this._step = step;
            this._caster.ClearCast(this);
            if (!_spell.IsInstant)
                Messenger.RemoveListener("Update", this.Update);
        }
    }

    public enum LifeStep
    {
        Null,
        Begin,
        Targeting,
        Casting,
        Done,
        Fail
    }
}