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

        //public PlayerController owner;
        public int ownerActorNumber;

        public int RP_instanceID; // set by GSM, allows resolution of card <-> RP_Card
        /// </summary>

        private CardsZone currentZone;

        // this is a struct defined in cardszone
        // this struct is a number of properties that are set based on the card's current zone
        // todo: this must support no zone
        public CardsZone.ExternallySetCardProperties externallySetProperties;
    
        // will build these cards out by composition
        // each card type will be it's own component (CharacterCardController / TurningPointCardController)
        // each effect will be it's own component 
        
            
        public void ResetCardNameText()
        {
            cardNameTMP.text = displayName;
        }

        public void SetCardText(string text)
        {
            cardNameTMP.text = text;
        }

        // this should only be called from CardsZone.AddCard
        public void SetCurrentZone(CardsZone newZone)
        {
            //Debug.Log(this + "setcurrentzone: " + newZone);
            this.currentZone = newZone;
        }
        public CardsZone GetCurrentZone()
        {
            return this.currentZone;
        }

        private void OnMouseDown()
        {
            Debug.Log("clicked");
        }

        public RP_Card GetRP_Card()
        {
            RP_Card rpCard = new RP_Card();
            rpCard.faction = faction;
            rpCard.zoneLocation = (currentZone.rpZoneType, ownerActorNumber);
            rpCard.ownerActorID = ownerActorNumber;
            rpCard.instanceID = RP_instanceID;
            rpCard.devName = gameObject.name; // dunno about this

            var ccc = gameObject.GetComponent<CharacterCardController>();
            if (ccc != null)
            {
                rpCard.power = ccc.power;
                rpCard.initiative = ccc.initiative;
                rpCard.armour = ccc.armour;
                rpCard.life = ccc.life;
            }

            return rpCard;
            
        }

        public void UpdateFromRP_Card(RP_Card rpCard, CardsZone localZone)
        {
            faction = rpCard.faction;
            currentZone = localZone;
            ownerActorNumber = rpCard.ownerActorID;

            RP_instanceID = rpCard.instanceID;
            gameObject.name = rpCard.devName; // dunno about this

            var ccc = gameObject.GetComponent<CharacterCardController>();
            if (ccc != null)
            {
                ccc.power = rpCard.power;
                ccc.initiative = rpCard.initiative;
                ccc.armour = rpCard.armour;
                ccc.life = rpCard.life;
            }
        }


    }



}
