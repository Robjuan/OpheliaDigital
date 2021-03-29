using System;
using System.Collections;
using UnityEngine;

namespace Com.WhiteSwan.OpheliaDigital
{
    [Serializable]
    public class UpdateMessage
    {
        public object payload; // in gameplay, will be RP_x class
        public Guid guid;

    }
}