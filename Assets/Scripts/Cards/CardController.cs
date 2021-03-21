using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using TMPro;

namespace Com.WhiteSwan.OpheliaDigital
{

    public class CardController : MonoBehaviour
    {

        [Flags]
        public enum Faction
        {
            None = 0
            ,Yucatec = 1
            ,Mattervoid = 2
            ,Mechanicus = 4
            ,All = ~0
        }

        [SerializeField]
        private TMP_Text cardNameTMP;

        public string displayName;

        public Faction faction;

        public PlayerController owner;

        public CardsZone currentZone;

        // this is a struct defined in cardszone
        // this struct is a number of properties that are set based on the card's current zone
        // todo: this must support no zone
        public CardsZone.ExternallySetCardProperties externallySetProperties;
    
        // will build these cards out by composition
        // each card type will be it's own component (CharacterCardController / TurningPointCardController)
        // each effect will be it's own component 
        
        // each unique card will be represented by a card id
        // each unique card will be a prefab variant of cardbase
            
        public void ResetCardNameText()
        {
            cardNameTMP.text = displayName;
        }

        public void SetCardText(string text)
        {
            cardNameTMP.text = text;
        }

        private void OnMouseDown()
        {
            Debug.Log("clicked");
        }

        public bool MoveTo(CardsZone targetZone)
        {
            if (true) // TODO: rules check if this is allowed
            {
                currentZone.RemoveCard(this);

                currentZone = targetZone;
                targetZone.AddCard(this);

                return true;
            }
            else
            {
                return false;
            }
        }

    }



}
