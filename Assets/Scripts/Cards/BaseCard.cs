using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Com.WhiteSwan.OpheliaDigital
{
    [CreateAssetMenu(menuName = "Cards/BaseCard")]
    public class BaseCard : ScriptableObject
    { 
        public string cardName;

        public enum SlotType { TurningPoint, Historic, Unsung }; 

        public int cost;


        // todo: not sure if this works for scriptableobjects
        public static BaseCard CreateFromJSON(string jsonString)
        {
            // handle cost
            return JsonUtility.FromJson<BaseCard>(jsonString);
        }

    }
}
