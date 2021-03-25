using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.WhiteSwan.OpheliaDigital
{

    public class CardsZone : MonoBehaviour
    {
        [System.Serializable]
        public enum LocalZoneType
        {
            MyHand
            ,MyDeck
            ,OppHand
            ,OppDeck
        }
        public LocalZoneType localZoneType;

        [HideInInspector]
        public int owner; // will match PUN ActorNumber, or -1 if not owned

        [Header("Display Properties")]
        public Transform startingLocation;
        public Vector3 stackingOffset;
        public bool moveOnZoneAdd;

        [Header("Properties Set On Contained Cards")]
        public ExternallySetCardProperties containedCardProperties;


        // if this is a class it is a reference-type
        // if this is a struct it is value-type
        [System.Serializable]
        public class ExternallySetCardProperties
        {

            public bool isPublicKnowledge;
            public bool isVisible;
            public bool isClickable;
        }

        private List<CardController> cards = new List<CardController>();


        public void AddCard(CardController thisCard)
        {

            if(thisCard.currentZone != this)
            {
                Debug.LogWarning("currentzone not set before adding card, bad data potential");
            }

            thisCard.externallySetProperties = containedCardProperties; // todo: reference or copy here?
            thisCard.currentZone = this;

            if (moveOnZoneAdd)
            {
                //todo: animate this somehow (call a function on the card?)
                thisCard.transform.position = startingLocation.position + (stackingOffset * cards.Count);
                thisCard.transform.rotation = startingLocation.rotation;
            }
        }

        public void RemoveCard(CardController thisCard)
        {
            if(cards.Contains(thisCard))
            {
                cards.Remove(thisCard);
            }    
            else
            {
                Debug.LogError(thisCard + " does not exist in " + this);
            }
        }

        public CardController GetTopCard()
        {
            if(cards.Count > 0)
            {
                return cards[cards.Count-1];
            }
            else
            {
                return null;
            }
        }

    }


}
