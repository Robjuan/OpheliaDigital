﻿using System.Collections;
using UnityEngine;

namespace Com.WhiteSwan.OpheliaDigital
{
    public class PhaseEndRequest : IChainable 
    {
        public int ownerAN { get ; set; }

        // constructor
        public PhaseEndRequest(int ownerAN)
        {
            this.ownerAN = ownerAN;
        }

        public void Resolve()
        {
            Debug.LogWarning("resolve on PER not imp");
        }



    }
}