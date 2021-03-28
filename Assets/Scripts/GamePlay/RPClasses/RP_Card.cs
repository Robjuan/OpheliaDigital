using System;
using System.Collections;
using UnityEngine;

namespace Com.WhiteSwan.OpheliaDigital
{
    [Serializable]
    public class RP_Card
    {
        public CardController.Faction faction;

        public (CardsZone.RP_ZoneType remoteZoneType, int actorNumber) zoneLocation;

        public int ownerActorID;

        public int instanceID;
        public string devName;

        // only character cards will have this
        public int power;
        public int initiative;
        public int armour;
        public int life;
    }
}