using System;
using System.Collections;
using UnityEngine;

namespace Com.WhiteSwan.OpheliaDigital
{
    [Serializable]
    public class RP_Card
    {
        public CardController.Faction faction;

        public int zone;

        public int ownerActorID;

        public int instanceID;
        public string devName;

        // only character cards will have this
        public int power;
        public int initiative;
        public int armour;
        public int life;

        public override string ToString()
        {
            return devName + "_" + instanceID + " oid:" + ownerActorID + " rz:" + zone; 
        }
    }
}