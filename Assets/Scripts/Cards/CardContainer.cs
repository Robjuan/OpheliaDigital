using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.WhiteSwan.OpheliaDigital
{

    public class CardContainer : MonoBehaviour
    {
        public string description;

        [Header("Display Properties")]
        public Transform startingLocation;
        public Vector3 stackingOffset;
        public bool moveOnContainerAdd;

        [Header("Properties Set On Contained Cards")]
        public ExternallySetCardProperties containedCardProperties;


        // if this is a class it is a reference-type
        // if this is a struct it is value-type
        [System.Serializable]
        public class ExternallySetCardProperties
        {

            public bool isPublicKnowledge;
            public bool isVisible;

        }



        // BindingList will raise events when lots of things happen
        public BindingList<GameObject> cards = new BindingList<GameObject>();

        private void Awake()
        {
            cards.RaiseListChangedEvents = true;
            cards.ListChanged += new ListChangedEventHandler(cards_ListChanged);
        }


        void cards_ListChanged(object sender, ListChangedEventArgs e)
        {

            GameObject thisCard = cards[e.NewIndex];
            thisCard.GetComponent<CardController>().externallySetProperties = containedCardProperties; // todo: reference or copy here?

            if (moveOnContainerAdd)
            {
                //todo: animate this somehow (call a function on the card?)
                thisCard.transform.position = startingLocation.position + (stackingOffset * cards.Count);
                thisCard.transform.rotation = startingLocation.rotation;
            }

        }
    }
}
