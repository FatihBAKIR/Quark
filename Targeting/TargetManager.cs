using System;
using Quark.Spell;
using Quark.Utilities;
using UnityEngine;

namespace Quark.Targeting
{
    public static class TargetManager
    {
        public static bool IsSelected
        {
            get { return SelecteCharacter != null; }
        }

        public static bool IsTargeting
        {
            get { return _currentData != null; }
        }

        public static Character SelecteCharacter { get; private set; }
        public static void Register()
        {
            Messenger.AddListener("Update", CheckTargeting);
            //Messenger<MouseArgs>.AddListener("*." + MouseEventType.Click + ".Mouse", OnCharacterClick);
        }

        private static void OnCharacterClick(MouseArgs mouseArgs)
        {
            SelecteCharacter = mouseArgs.Character;
        }

        private static void OnCharacterClick(Character character)
        {
            SelecteCharacter = character;
        }

        static void CheckTargeting()
        {
            if (Input.GetMouseButtonUp(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                    OnCharacterClick(hit.collider.GetComponent<Character>());
                else OnCharacterClick(null);
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

            /*
            if (Utils.Checkflag(data.Spell.Targetables, TargetType.Character))
                data.AddTarget(GameObject.Find("Cylinder").GetComponent<Character>());

            if (data.Spell.TargetForm == TargetForm.Plural && Utils.Checkflag(data.Spell.Targetables, TargetType.Point))
            {
                data.AddTarget(new Vector3(0, 0, 0));
                data.AddTarget(new Vector3(10, 0, 10));
                data.AddTarget(new Vector3(10, 0, 0));
                data.AddTarget(new Vector3(0, 0, 10));
            }
             */

            //Single character target and we already have a target, no need to wait input
            if (data.Spell.TargetForm == TargetForm.Singular &&
                Utils.Checkflag(data.Spell.Targetables, TargetType.Character))
            {
                if (IsSelected)
                {
                    data.AddTarget(SelecteCharacter);
                    DoneTargeting();
                }
                else
                    FailTargeting();
                return;
            }
        }
    }

    [Flags]
    public enum TargetType
    {
        Point = 1,
        Character = 2,
        Allied = 4,
        Neutral = 8,
        Enemy = 16,
        Self = 32,
        AoE = 64
    }

    public enum TargetForm
    {
        Singular,
        Plural
    }
}