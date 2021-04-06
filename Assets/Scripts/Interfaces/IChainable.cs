using System.Collections;
using UnityEngine;

namespace Com.WhiteSwan.OpheliaDigital
{
    public interface IChainable
    {
        int ownerAN { get; set; }

        void Resolve();
    }
}