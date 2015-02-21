﻿using System.Collections.Generic;
using Quark.Exceptions;
using Quark.Targeting;
using Quark.Utilities;
using UnityEngine;

namespace Quark.Spell
{
    public class Cast
    {
        Character _caster;
        Vector3 _beginPoint;
        float _beginningTime;

        TargetCollection _targets;
        Spell _spell;
        TargetMacro _macro;
        LifeStep _step = LifeStep.Null;

        public uint HitCount = 0;

        public Cast()
        {
#if DEBUG
            Logger.GC("CastData::ctor");
#endif
        }

        ~Cast()
        {
            _spell = null;
            _caster = null;
#if DEBUG
            Logger.GC("CastData::dtor");
#endif
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
        public static Cast PrepareCast(Character caster, Spell spell)
        {
            Cast data = new Cast();
            data._step = LifeStep.Null;
            data._caster = caster;
            data._spell = spell;
            Messenger<Cast>.Broadcast("Prepare", data);
            Messenger<Cast>.Broadcast(data.Spell.Name + ".Prepare", data);
            Messenger<Cast>.Broadcast(caster.Identifier + "." + data.Spell.Name + ".Prepare", data);
            data._spell.SetContext(data);
            Logger.Debug("Cast::PrepareCast");
            if (!data._spell.CanInvoke())
                return null;
            data.Invoke();
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

        public TargetCollection Targets
        {
            get
            {
                return _targets;
            }
        }

        public Targetable[] Targetables
        {
            get
            {
                return _targets.Targetables;
            }
        }

        public Character[] TargetCharacters
        {
            get
            {
                return _targets.Characters;
            }
        }

        public Vector3[] TargetPoints
        {
            get
            {
                return _targets.Points;
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

            if (_spell.TargetForm == TargetForm.Singular && (_targets.Count > 0))
                throw new SingularSpellException();

            return true;
        }

        public void AddTarget(Targetable targetable)
        {
            CanAddTarget();
            _targets.Add(targetable);
        }

        public void AddTarget(Character character)
        {
            CanAddTarget();
            _targets.Add(character);
        }

        public void AddTarget(Vector3 point)
        {
            CanAddTarget();
            _targets.Add(point);
        }

        /// <summary>
        /// Begin the casting logic.
        /// </summary>
        void Invoke()
        {
            _step = LifeStep.Begin;
            Logger.Debug("Cast::Invoke");
            if (!_caster.CanCast(Spell))
            {
                Messenger<CastError>.Broadcast("CastError", new BusyError());
                return;
            }
            _caster.AddCast(this);
            _spell.OnInvoke();
            BeginTargeting();
        }

        /// <summary>
        /// Begin targeting logic
        /// </summary>
        void BeginTargeting()
        {
            _step = LifeStep.Targeting;
            _targets = new TargetCollection();
            _macro = _spell.TargetMacro;
            _macro.SetData(this);
            _macro.Run();
        }

        public void TargetingFail(TargetingError error)
        {
            _macro = null;
            Clear(LifeStep.Fail);
        }

        /// <summary>
        /// BeginTargeting done logic.
        /// </summary>
        public void TargetingDone(TargetCollection targets)
        {
            /*
             * Store the time and the point at the instant the targeting was done and check for the spell type, if it is an instant spell, immediately cast it
             * otherwise, begin the casting logic
             */
            _macro = null;
            _beginningTime = Time.timeSinceLevelLoad;
            _beginPoint = _caster.transform.position;
            _spell.OnTargetingDone();
            if (_spell.IsInstant)
                CastDone();
            else
                StartCast();
        }

        void StartCast()
        {
            Logger.Debug("Cast::StartCast");
            _step = LifeStep.Casting;
            Messenger.AddListener("Update", Update);
            _spell.OnCastingBegan();
        }

        void controlCast()
        {
            _step = LifeStep.Casting;
            if (CastPercentage >= 100)
                this.CastDone();
        }

        void CastDone()
        {
            Logger.Debug("Cast::CastDone");
            Clear(LifeStep.Done);
            _spell.OnCastDone();
        }

        void Update()
        {
            controlCast();
        }

        void Clear(LifeStep step)
        {
            _step = step;
            _caster.ClearCast(this);
            if (!_spell.IsInstant)
                Messenger.RemoveListener("Update", Update);
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