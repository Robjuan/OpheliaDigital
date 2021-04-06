using UnityEngine;

namespace Com.WhiteSwan.OpheliaDigital
{
    public class CardEffectBase : MonoBehaviour, IChainable
    {
        public int ownerAN { get ; set ; }
        public int instanceID;

        public string searchString;
        public GamePlayConstants.EffectType effectType;
        public string fullText;

        public virtual void Resolve()
        {
            Debug.LogError("base class CardEffectBase Resolve called - not overridden?");
        }
        

    }
}