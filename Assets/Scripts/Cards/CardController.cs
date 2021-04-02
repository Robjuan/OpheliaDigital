using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Photon.Pun;
using TMPro;

namespace Com.WhiteSwan.OpheliaDigital
{

    public class CardController : MonoBehaviour
    {

        PhotonView photonView;

        [Header("Technical Refs")]
        [SerializeField]
        private TMP_Text cardNameTMP;
        public int ownerActorNumber;
        public string devName;

        public enum SlotType
        {
            Unsung
            ,Historic
            ,Fabled
            ,TurningPoint
            ,Unique
        }
        
        [Flags]
        public enum Faction
        {
            None = 0
            ,Yucatec = 1
            ,Mattervoid = 2
            ,Mechanicus = 4
            ,All = ~0
        }

        [Header("Gameplay Properties")]
        public SlotType slotType;
        public Faction faction;

        // for setting in prefab generator (todo: getters if needed)
        private int baseLevel, basePower, baseInitiative, baseArmour, baseLife;
        public void SetBaseStats(int level, int power, int initiative, int armor, int life)
        {
            baseLevel = level;
            basePower = power;
            baseInitiative = initiative;
            baseArmour = armor;
            baseLife = life;
        }


        public int level; // stars for characters, cost for TPs
        public int power;
        public int initiative;
        public int armour;
        public int life;

        public int currentDamage;

        // probably just for reference and to be loaded from the json
        public string claimText;
        public string specialText;
        public string passiveText;
        public string effectText; // only TPs

        private int currentZone = -1; // server based int, use with ZoneResolver to get local ref
        public string displayName;
        

        public int instanceID; // set by GSM

        
        // this is a struct defined in cardszone
        // this struct is a number of properties that are set based on the card's current zone
        // todo: this must support no zone
        public CardsZone.ExternallySetCardProperties externallySetProperties;

        // will build these cards out by composition
        // each effect will be it's own component 

        private void Awake()
        {
            level = baseLevel;
            power = basePower;
            initiative = baseInitiative;
            armour = baseArmour;
            life = baseLife;

            gameObject.GetComponent<CardController>().ResetCardNameText();
            photonView = gameObject.GetComponent<PhotonView>();
        }

        public void ResetCardNameText()
        {
            cardNameTMP.text = displayName;
        }

        public void SetCardText(string text)
        {
            cardNameTMP.text = text;
        }


        public void SetCurrentZone(int newZone)
        {
            photonView.RPC(SetCurrentZone_RPC_string, RpcTarget.AllViaServer, newZone);
        }

        public const string SetCurrentZone_RPC_string = "SetCurrentZone_RPC";
        [PunRPC]
        public void SetCurrentZone_RPC(int newZone)
        {
            GameEvents.current.CardAdded(this, newZone, currentZone);
            GameEvents.current.CardRemoved(this, currentZone);
            currentZone = newZone;           
        }

        public int GetCurrentZone()
        {
            return currentZone;
        }

        private void OnMouseDown()
        {
            Debug.Log(displayName + ":" + instanceID + " clicked");
        }

    }



}
