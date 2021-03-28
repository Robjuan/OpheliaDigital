using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

using TMPro;

namespace Com.WhiteSwan.OpheliaDigital
{
    public class LocalGameManager : MonoBehaviourPunCallbacks
    {

        public static LocalGameManager Instance;

        public GameStateManager gameStateManager;
        public List<CardsZone> zones = new List<CardsZone>();

        // these are just for convenience internally here
        private CardsZone myDeck;
        private CardsZone myHand;

        [Header("Prefab Refs")]
        public GameObject playerControllerPrefab;
        [SerializeField]
        private GameObject readyButton;

        [HideInInspector]
        public List<PlayerController> playerControllers = new List<PlayerController>();

        private List<GameObject> allCards = new List<GameObject>();

        [Header("Display Controls")]
        public RectTransform playerDisplayPlace;
        public RectTransform opponentDisplayPlace;
        [SerializeField]
        private TMP_Text currentRoomDisplay;


        private string lastSetPhase;
        private bool loaded = false;

        private void Start()
        {
            Instance = this;

            if(gameStateManager == null)
            {
                Debug.LogError("manager setup no good");
            }


            if (currentRoomDisplay != null)
            {
                currentRoomDisplay.text = PhotonNetwork.CurrentRoom.Name;
            }

            // if the only one here, it'll do me, otherwise will do everyone
            foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                var newPlayer = SetupPlayerController(player);
                if (player.IsLocal)
                {
                    newPlayer.SetupDisplay(playerDisplayPlace);
                }
                else
                {
                    newPlayer.SetupDisplay(opponentDisplayPlace);
                }
                gameStateManager.playerControllers.Add(newPlayer);
            }

           
            // initialise convenience methods
            foreach (CardsZone zone in zones)
            {
                if (zone.localZoneType == CardsZone.LocalZoneType.MyDeck)
                {
                    myDeck = zone;
                }
                if (zone.localZoneType == CardsZone.LocalZoneType.MyHand)
                {
                    myHand = zone;
                }
            }

        }

        // this will catch any updates to the gamestate from the GameStateManager
        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            // if my player thingo changed
            if (propertiesThatChanged.ContainsKey(KeyStrings.ActorPrefix + PhotonNetwork.LocalPlayer.ActorNumber))
            {
                Debug.Log("my rp_player changed");
            }

            if (propertiesThatChanged.ContainsKey(KeyStrings.RP_BoardString))
            {
                RP_Board board = (RP_Board)propertiesThatChanged[KeyStrings.RP_BoardString];
                var retval = ChangePhase(board.currentPhase);
                if (!retval)
                {
                    Debug.LogWarning("unable to local change phase to :" + board.currentPhase);
                }
            }

            // todo: do this loop better?
            // this will update any card from any card update
            // need to ensure cards are ready to be updated tho
            if(loaded)
            {
                foreach (string key in propertiesThatChanged.Keys)
                {
                    if (key.Contains(KeyStrings.CardIdentPrefix))
                    {
                        RP_Card rpCard = (RP_Card)PhotonNetwork.CurrentRoom.CustomProperties[key];
                        //Debug.Log(rpCard.ToString());
                        foreach (GameObject card in allCards)
                        {
                            Debug.Log(card.GetComponent<CardController>().RP_instanceID);
                        }
                        GameObject localCardCont = allCards.Find(x => x.GetComponent<CardController>().RP_instanceID == rpCard.instanceID);
                        Debug.Log(localCardCont);
                        var localCardCC = localCardCont.GetComponent<CardController>();
                        if (localCardCC != null)
                        {
                            localCardCC.UpdateFromRP_Card(rpCard, ResolveRPZoneToLocal(rpCard.zoneLocation));
                        }

                    }
                }
            }

        }

        private CardsZone ResolveRPZoneToLocal((CardsZone.RP_ZoneType remoteZoneType, int actorNumber) RPZone)
        {
            foreach(CardsZone zone in zones)
            {
                // one of my zones
                if(RPZone.actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    //if (RPZone.label == KeyStrings.Zone_Deck && zone.localZoneType == CardsZone.LocalZoneType.MyDeck)
                    if (RPZone.remoteZoneType == CardsZone.RP_ZoneType.Deck && zone.localZoneType == CardsZone.LocalZoneType.MyDeck)
                    {
                        return zone;
                    }
                    else if (RPZone.remoteZoneType == CardsZone.RP_ZoneType.Hand && zone.localZoneType == CardsZone.LocalZoneType.MyHand)
                    {
                        return zone;
                    }
                } 
                else // opp or neutral zone
                {
                    if (RPZone.remoteZoneType == CardsZone.RP_ZoneType.Deck && zone.localZoneType == CardsZone.LocalZoneType.OppDeck)
                    {
                        return zone;
                    }
                    else if (RPZone.remoteZoneType == CardsZone.RP_ZoneType.Hand && zone.localZoneType == CardsZone.LocalZoneType.OppHand)
                    {
                        return zone;
                    }
                }

                
            }
            Debug.LogError("unable to resolve RPZone to local zone");
            return null;
        }

        private PlayerController SetupPlayerController(Player punPlayer)
        {
            var newPlayer = Instantiate(playerControllerPrefab).GetComponent<PlayerController>();
            newPlayer.punPlayer = punPlayer;

            // make the link go both ways
            Hashtable ht = new Hashtable();
            ht.Add(KeyStrings.UnityInstanceID, newPlayer.GetInstanceID());
            punPlayer.SetCustomProperties(ht);

            return newPlayer;
        }

        // called by GSM OnRoomPropertiesUpdate
        public void PrepareCards()
        {
            foreach (string key in PhotonNetwork.CurrentRoom.CustomProperties.Keys)
            {
                if(key.Contains(KeyStrings.CardIdentPrefix))
                {
                    RP_Card rpCard = (RP_Card)PhotonNetwork.CurrentRoom.CustomProperties[key];
                    GameObject localCard = (GameObject)Instantiate(Resources.Load(rpCard.devName));
                    var localCardCC = localCard.GetComponent<CardController>();

                    localCardCC.ownerActorNumber = rpCard.ownerActorID;
                    localCardCC.RP_instanceID = rpCard.instanceID;
                    allCards.Add(localCard);

                    var localZone = ResolveRPZoneToLocal(rpCard.zoneLocation);

                    localZone.AddCard(localCardCC);
                    
                }
            }

            Hashtable ht = new Hashtable();
            ht.Add(KeyStrings.PhaseReady, true);
            PhotonNetwork.LocalPlayer.SetCustomProperties(ht);

        }
        
        public bool ChangePhase(string phaseKey)
        {
            if (phaseKey == lastSetPhase)
            {
                // don't restart the current phase
                return false;
            }
            if(phaseKey == KeyStrings.PreGameSetupPhase)
            {
                lastSetPhase = phaseKey;
                DoPreGameSetupPhase();
                return true;
            }
            return false;
        }

        private void DoPreGameSetupPhase()
        {
            Debug.Log("doing pregame");
            // draw cards 
            // aka request GSM to move 6 cards
            for (int i = 0; i < GamePlayConstants.InitialDrawCount; i++)
            {
                DrawCard();
            }

        }

        private bool DrawCard()
        {
            if(myDeck.GetTopCard() != null)
            {
                gameStateManager.UpdateCardLocation(myDeck.GetTopCard(), myDeck, myHand);
                return true;
            }
            else
            {
                Debug.LogWarning("tried to draw with no cards in deck");
                return false;
            }

        }
    }
}

