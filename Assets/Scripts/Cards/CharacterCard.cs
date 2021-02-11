using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Com.WhiteSwan.OpheliaDigital
{
    [CreateAssetMenu(menuName = "Cards/Character")]

    public class CharacterCard : BaseCard
    {
        public int armour;
        public int initiative;
        public int power;
        public int life;

        public string passiveText;
        public string claimTeXt;
        public string specialText;


    }
}

