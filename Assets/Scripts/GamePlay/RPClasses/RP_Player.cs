using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Com.WhiteSwan.OpheliaDigital
{
    [Serializable]
    public class RP_Player
    {
        public int punActorID;

        public int points;

        public int turnOrder; // -1 means not yet assigned

    }
}