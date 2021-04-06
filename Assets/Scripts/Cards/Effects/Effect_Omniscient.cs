using System;
using UnityEngine;


namespace Com.WhiteSwan.OpheliaDigital
{
    [card_064_ophelia]
    public class Effect_Omniscient : CardEffectBase
    {

        private void Awake()
        {
            searchString = "monasd";
            effectType = GamePlayConstants.EffectType.Passive;
            fullText = "ad";
        }

        public override void Resolve()
        {
            Debug.LogWarning("firing swansong, not implemented");
        }

    }
}