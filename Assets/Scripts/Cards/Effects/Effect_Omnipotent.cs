using System;
using UnityEngine;


namespace Com.WhiteSwan.OpheliaDigital
{
    [card_064_ophelia]
    public class Effect_Omnipotent : CardEffectBase
    {

        private void Awake()
        {
            searchString = "swansong";
            effectType = GamePlayConstants.EffectType.Claim;
            fullText = "do a swan song thing";
        }

        public override void Resolve()
        {
            Debug.LogWarning("firing swansong, not implemented");
        }

    }

    // this attribute determines which card it will be attached to on prefab generation, thus must include the card's devname
    // only needs to be defined once
    public class card_064_ophelia : Attribute { }

}