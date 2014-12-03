﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quark
{
    public class CastData
    {
        Character caster;
        Vector3 beginPoint;
        float beginningTime;
        List<Character> targetChars;
        List<Vector3> targetPoints;
        Spell spell;

        LifeStep step = LifeStep.Null;

        public uint HitCount = 0;

        ~CastData()
        {
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
            data.step = LifeStep.Null;
            data.caster = caster;
            data.spell = spell;
            spell.SetData(ref data);
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
                return caster;
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
                return Time.timeSinceLevelLoad - beginningTime;
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
                return (int)(CastTime * 100 / spell.CastDuration);
            }
        }

        public Character[] TargetCharacters
        {
            get
            {
                return targetChars.ToArray();
            }
        }

        public Vector3[] TargetPoints
        {
            get
            {
                return targetPoints.ToArray();
            }
        }

        public Vector3 CastBeginPoint
        {
            get
            {
                return beginPoint;
            }
        }

        public Spell Spell
        {
            get
            {
                return spell;
            }
        }

        public bool CanAddTarget()
        {
            if (this.step != LifeStep.Targeting)
                throw new NotTargetingException();

            if (this.spell.TargetForm == TargetForm.Singular && (this.targetChars.Count > 0 || this.targetPoints.Count > 0))
                throw new SingularSpellException();

            return true;
        }

        public void AddTarget(Character character)
        {
            CanAddTarget();
            this.targetChars.Add(character);
        }

        public void AddTarget(Vector3 point)
        {
            CanAddTarget();
            this.targetPoints.Add(point);
        }

        /// <summary>
        /// Begin the casting logic.
        /// </summary>
        void Begin()
        {
            this.step = LifeStep.Begin;
            Logger.Debug("CastData.Begin");
            if (!caster.CanCast)
                return;
            caster.AddCast(this);
            spell.OnBegin();
            this.Targeting();
        }

        void Targeting()
        {
            this.step = LifeStep.Targeting;
            this.targetChars = new List<Character>();
            this.targetPoints = new List<Vector3>();
            TargetManager.GetTargets(this);
        }

        /// <summary>
        /// Targeting done logic.
        /// </summary>
        public void TargetingDone()
        {
            /*
         * Copy the time and the point at the instant the targeting was done and check for the spell type, if it is an instant spell, immediately cast it
         * otherwise, begin the casting logic
         */
            this.beginningTime = Time.timeSinceLevelLoad;
            this.beginPoint = caster.transform.position;
            spell.OnTargetingDone();
            if (spell.IsInstant)
                this.CastDone();
            else
                this.Cast();
        }

        void Cast()
        {
            this.step = LifeStep.Casting;
            Messenger.AddListener("Update", this.Update);
            spell.OnCastingBegan();
        }

        void controlCast()
        {
            this.step = LifeStep.Casting;
            if (CastPercentage >= 100)
                this.CastDone();
        }

        void CastDone()
        {
            this.step = LifeStep.Done;
            this.caster.ClearCast(this);
            if (!spell.IsInstant)
                Messenger.RemoveListener("Update", this.Update);
            spell.OnCastDone();
        }

        void Update()
        {
            controlCast();
        }

        public void Clear()
        {
            this.spell = null;
            this.caster = null;
        }
    }

    public enum LifeStep
    {
        Null,
        Begin,
        Targeting,
        Casting,
        Done
    }
}