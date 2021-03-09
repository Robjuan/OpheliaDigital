using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.WhiteSwan.OpheliaDigital
{
    public class CharacterCardController : MonoBehaviour
    {
        public enum SlotType
        {
            Unsung
            ,Historic
            ,Fabled
        }

        // Game Stats (should these be public vars? better to expose via method?)

        public int baseLevel;
        public int basePower;
        public int baseInitiative;
        public int baseArmour;
        public int baseLife;

        public SlotType slotType;

        public int level;
        public int power;
        public int initiative;
        public int armour;
        public int life;

        public int currentDamage;

        // probably just for reference and to be loaded from the json
        public string claimText;
        public string specialText;
        public string passiveText;

        public string devName;

        private void Start()
        {
            level = baseLevel;
            power = basePower;
            initiative = baseInitiative;
            armour = baseArmour;
            life = baseLife;

            gameObject.GetComponent<CardController>().ResetCardNameText();

        }


    }
}