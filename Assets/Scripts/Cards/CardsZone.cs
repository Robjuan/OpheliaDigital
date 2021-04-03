using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

namespace Com.WhiteSwan.OpheliaDigital
{

    public class CardsZone : MonoBehaviour
    {
        [System.Serializable]
        public enum LocalZoneType : byte // byte for network
        {
            MyHand = 0
            ,MyDeck
            ,MyFZ
            ,OppHand
            ,OppDeck
            ,OppFZ
            ,NeutralBoardLeft
            ,NeutralBoardRight
            ,MyBoardLeft
            ,MyBoardCentre
            ,MyBoardRight
            ,OppBoardLeft
            ,OppBoardCentre
            ,OppBoardRight
            ,RemovedFromGame
        }
        public LocalZoneType localZoneType;

        [SerializeField]
        private int remoteZoneID;
        public bool initDone = false;

        [HideInInspector]
        public int ownerActorNumber; // will match PUN ActorNumber, or -1 if not owned

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

        private void Awake()
        {
            GameEvents.current.onCardAdded += AddCard;
            GameEvents.current.onCardRemoved += RemoveCard;
        }

        public void InitialiseRemoteID()
        {
            remoteZoneID = LocalGameManager.current.ResolveLocalZoneToRemote(this);
            initDone = true;
            //Debug.Log("init lzt: " + localZoneType + ", remoteID: " + remoteZoneID + "uid: " + GetInstanceID()) ;
        }

        public void AddCard(CardController thisCard, int newZone, int previousZone)
        {
            if(!initDone)
            {
                Debug.LogError("attempting to add to non init zone, rid:" + remoteZoneID + "uid: " + GetInstanceID());
                return;
            }
            if(newZone != remoteZoneID)
            {
                // it's not being added here
                return;
            }

            //Debug.Log("adding card to rid: " + remoteZoneID);
            thisCard.externallySetProperties = containedCardProperties; // todo: reference or copy here?
            cards.Add(thisCard);

            if (moveOnZoneAdd)
            {
                //todo: animate this somehow (call a function on the card?)
                // todo: make stacking offset work more dynamically for hands
                // define card size in code?
                thisCard.transform.position = startingLocation.position + (stackingOffset * cards.Count);
                thisCard.transform.rotation = startingLocation.rotation;
            }

            
        }

        public void RemoveCard(CardController thisCard, int previousZone)
        {
            if(previousZone != remoteZoneID)
            {
                // not being removed from here
                return;
            }
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
