using System;
using Quark.Spell;
using Quark.Utilities;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace Quark.Targeting
{
    public static class TargetManager
    {
        public static LayerMask TerrainLayer = LayerMask.NameToLayer("Terrain");

        public static bool IsSelected
        {
            get { return SelectedCharacter != null; }
        }

        public static bool IsTargeting
        {
            get { return _currentData != null; }
        }

        public static bool IsBusy
        {
            get { return _runningMacro != null; }
        }

        public static Character SelectedCharacter { get; private set; }

        public static void Register()
        {
            Messenger.AddListener("Update", CheckTargeting);
            //Messenger<MouseArgs>.AddListener("*." + MouseEventType.Click + ".Mouse", OnCharacterClick);
        }

        private static void OnCharacterClick(MouseArgs mouseArgs)
        {
            OnCharacterClick(mouseArgs.Character);
        }

        private static void OnCharacterClick(Character character)
        {
            Messenger.Broadcast("SelectedCharacterChanged");
            SelectedCharacter = character;
        }

        private static void OnPointClick(Vector3 point)
        {
            if (!IsTargeting)
                return;

            if (!IsPointValid)
            {
                Messenger<CastError>.Broadcast("CastError", new RangeError());
                FailTargeting();
                return;
            }

            if (_pointCallback != null)
                _pointCallback(point);
            
            return;

            _currentData.AddTarget(point);
            DoneTargeting();
        }

        static void CheckTargeting()
        {
            RunRaycast();
            if (Input.GetMouseButtonUp(0))
            {
                if (IsCharacterHovering)
                    OnCharacterClick(HoveringCharacter);
                else if (IsPointHovering)
                    OnPointClick(HoveringPoint);
                else
                    OnCharacterClick(null);
            }

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Messenger<CastError>.Broadcast("CastError", new CancelError());
                FailTargeting();
            }
        }

        static void DoneTargeting()
        {
            _currentData.TargetingDone();
            _currentData = null;
        }

        static void FailTargeting()
        {
            _currentData.TargetingFail();
            _currentData = null;
        }

        private static CastData _currentData = null;

        public static void GetTargets(CastData data)
        {
            if (IsTargeting)
            {
                Messenger<CastError>.Broadcast("CastError", new TargetingError());
                data.TargetingFail();
                return;
            }

            _currentData = data;

            if (data.Spell.Targetables == TargetType.Character)
            {
                _currentData.AddTarget(_currentData.Caster);
                DoneTargeting();
            }

            //Single character target and we already have a target, no need to wait input
            if (_currentData.Spell.TargetForm == TargetForm.Singular &&
                Utils.Checkflag(data.Spell.Targetables, TargetType.Character))
            {
                if (IsSelected)
                {
                    _currentData.AddTarget(SelectedCharacter);
                    DoneTargeting();
                }
                else
                    FailTargeting();
                return;
            }

            //Otherwise, it is an asynchronous targeting, we simply broadcast the event and wait for targeting
            Messenger<TargetType>.Broadcast("Targeting", data.Spell.Targetables);
            Messenger<TargetType>.Broadcast(data.Spell.Targetables + ".Targeting", data.Spell.Targetables);
        }

        static void RunRaycast()
        {
            HoveringCharacter = null;
            _hoveringPoint = null;

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (IsTargeting)
            {
                if (Utils.Checkflag(_currentData.Spell.Targetables, TargetType.Point))
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << TerrainLayer.value))
                        HoveringPoint = hit.point;
            }
            else if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.GetComponent<Character>() != null)
                    HoveringCharacter = hit.collider.GetComponent<Character>();
            }
        }

        public static Character HoveringCharacter
        {
            get;
            private set;
        }

        static Vector3? _hoveringPoint;

        public static Vector3 HoveringPoint
        {
            get
            {
                return (Vector3)_hoveringPoint;
            }
            private set
            {
                _hoveringPoint = value;
            }
        }

        public static bool IsCharacterHovering
        {
            get
            {
                return HoveringCharacter != null;
            }
        }

        public static bool IsPointHovering
        {
            get
            {
                return _hoveringPoint != null;
            }
        }

        public static float PointDistance
        {
            get
            {
                return Vector3.Distance(_currentData.Caster.transform.position, HoveringPoint);
            }
        }

        public static bool IsPointValid
        {
            get
            {
                if (!IsTargeting)
                    return false;

                if (PointDistance > _currentData.Spell.CastRange)
                    return false;

                return true;
            }
        }

        public static bool ReserveTargeter(TargetMacro macro)
        {
            if (IsBusy)
                return false;

            _runningMacro = macro;
            return true;
        }

        public static void FreeTargeter()
        {
            _currentData = null;
        }

        private static TargetMacro _runningMacro;
        private static Callback<Vector3> _pointCallback;

        public static void RequestPoint(Callback<Vector3> handler)
        {
            _pointCallback = handler;
        }

        public static Character RequestCharacter()
        {
            return SelectedCharacter;
        }
    }

    public enum TargetType
    {
        Point,
        Character
    }

    public enum TargetForm
    {
        /// <summary>
        /// May hit multiple characters
        /// </summary>
        Singular,
        /// <summary>
        /// May not hit multiple characters
        /// </summary>
        Plural
    }

    [Flags]
    public enum TargetConstraints
    {
        Allied = 1,
        Neutral = 2,
        Enemy = 4,
        Self = 8
    }
}