using Quark.Exceptions;
using Quark.Targeting;
using Quark.Utilities;
using UnityEngine;

namespace Quark.Spells
{
    public class Cast : Identifiable
    {
        Character _caster;
        Vector3 _castBeginPoint;
        float _castBeginningTime;

        TargetCollection _targets;
        Spell _spell;
        TargetMacro _macro;
        Stages _step = Stages.Null;

        /// <summary>
        /// The total hit count in this context.
        /// </summary>
        public uint HitCount = 0;
        
#if DEBUG
        public Cast()
        {
            Logger.GC("Cast::ctor");
        }
#endif

        ~Cast()
        {
            _spell = null;
            _caster = null;
#if DEBUG
            Logger.GC("Cast::dtor");
#endif
        }

        /// <summary>
        /// Prepares a new instance of <see cref="Cast"/> with the specified caster and spell.
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
            Cast data = new Cast {_step = Stages.Null, _caster = caster, _spell = spell, NonSpell = false };
            Messenger<Cast>.Broadcast("Prepare", data);
            Messenger<Cast>.Broadcast(data.Spell.Name + ".Prepare", data);
            Messenger<Cast>.Broadcast(caster.Identifier + "." + data.Spell.Name + ".Prepare", data);
            data._spell.SetContext(data);
            Logger.Debug("Cast::PrepareCast");
            if (!data._spell.CanInvoke())
                return null;
            data.Identifier = spell.Identifier;
            data.Invoke();
            return data;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Quark.Spells.Cast"/> context is Item sourced.
        /// </summary>
        /// <value><c>true</c> if non Spell; otherwise, <c>false</c>.</value>
        public bool NonSpell { get; protected set; }

        public static Cast PrepareCast(Character caster, Item item)
        {
            Cast data = new Cast
            {
                _step = Stages.Null,
                _caster = caster,
                NonSpell = true,
                Identifier = item.Identifier
            };
            return data;
        }

        public string Identifier { get; private set; }

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
                return Time.timeSinceLevelLoad - _castBeginningTime;
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

        public Vector3 CastBeginPoint
        {
            get
            {
                return _castBeginPoint;
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
                if (_step != Stages.Null)
                    return;
                _spell = value;
            }
        }

        public bool CanAddTarget()
        {
            if (_step != Stages.Targeting)
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
            _step = Stages.Invoke;
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
            _step = Stages.Targeting;
            _targets = new TargetCollection();
            _macro = _spell.TargetMacro;
            _macro.SetData(this);
            _macro.Run();
        }

        public void TargetingFail(TargetingError error)
        {
            _macro = null;
            Clear(Stages.Failed);
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
            _macro = null; // Let GC collect the target macro we used
            _castBeginningTime = Time.timeSinceLevelLoad;
            _castBeginPoint = _caster.transform.position;
            _spell.OnTargetingDone();
            if (_spell.IsInstant)
                CastDone();
            else
                StartCast();
        }

        ConditionCollection _interruptConditions;
        void StartCast()
        {
            Logger.Debug("Cast::StartCast");

            _step = Stages.Casting;
            _interruptConditions = Caster.InterruptConditions.DeepCopy();
            Messenger.AddListener("Update", ControlCast);
            _spell.OnCastingBegan();
        }

        /// <summary>
        /// This flag determines whether the current Cast instance has been interrupted or not
        /// </summary>
        bool _interrupted;

        /// <summary>
        /// This field stores the last time the Casting logic of the spell being casted was executed
        /// </summary>
        private float _lastCast;

        void ControlCast()
        {
            _step = Stages.Casting;
            CheckInterrupt();
            if (_interrupted)
                CastFail();
            if (CastPercentage >= 100)
                CastDone();
            if (Time.timeSinceLevelLoad > _lastCast + Spell.CastingInterval)
            {
                Spell.OnCasting();
                _lastCast = Time.timeSinceLevelLoad;
            }
        }

        void CastDone()
        {
            Logger.Debug("Cast::CastDone");

            Clear(Stages.Done);
            _spell.OnCastDone();
        }

        void CastFail()
        {
            Logger.Debug("Cast::CastFail");

            Clear(Stages.Failed);
        }

        void CheckInterrupt()
        {
            _interruptConditions.SetContext(this);
            if (_interruptConditions.Check())
                Interrupt();
        }

        /// <summary>
        /// Interrupt this Cast instance.
        /// </summary>
        public void Interrupt()
        {
            _interrupted = true;
            _spell.OnInterrupt();
        }

        public void Clear(Stages step)
        {
            Logger.Debug("Cast::Clear");

            _interruptConditions = null;
            _step = step;
            _caster.ClearCast(this);
            if (!_spell.IsInstant)
                Messenger.RemoveListener("Update", ControlCast);
        }
    }

    public enum Stages
    {
        Null,
        Invoke,
        Targeting,
        Casting,
        Done,
        Failed
    }
}