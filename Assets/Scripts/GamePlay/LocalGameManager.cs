using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
        public static LocalGameManager current
        {
            get
            {
                if (_current == null)
                    _current = FindObjectOfType(typeof(LocalGameManager)) as LocalGameManager;

                return _current;
            }
            set
            {
                _current = value;
            }
        }
        private static LocalGameManager _current;

        public List<CardsZone> zones = new List<CardsZone>();

        // these are just for convenience internally here
        private CardsZone myDeck;
        private CardsZone myHand;

        [Header("Prefab Refs")]
        public GameObject playerControllerPrefab;
        [SerializeField]
        private GameObject readyButton;

        [Header("Display Controls")]
        public RectTransform playerDisplayPlace;
        public RectTransform opponentDisplayPlace;
        [SerializeField]
        private TMP_Text currentRoomDisplay;


        private string lastSetPhase;

        private void Awake()
        {
            current = this;
            GameEvents.current.onPhaseChange += ChangePhase;
        }

        private void Start()
        {

            if (currentRoomDisplay != null)
            {
                currentRoomDisplay.text = PhotonNetwork.CurrentRoom.Name;
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

        public void SetupPlayerDisplay()
        {
            foreach (PlayerController player in GameStateManager.current.playerControllers)
            {
                if (player.punPlayer.IsLocal)
                {
                    player.SetupDisplay(playerDisplayPlace);
                }
                else
                {
                    player.SetupDisplay(opponentDisplayPlace);
                }
            }
        }

        // called by GSM as each player is loaded
        public void InitAllZoneIds()
        {
            foreach(CardsZone zone in zones)
            {
                zone.InitialiseRemoteID();
            }

        }


        public CardsZone ResolveRemoteZoneToLocal(int RemoteZone)
        {
            Dictionary<int, CardsZone.LocalZoneType> zoneMap = GameStateManager.current.localPlayerZoneMap;
            foreach (CardsZone zone in zones)
            {
                if (zoneMap[RemoteZone] == zone.localZoneType)
                {
                    return zone;
                }                
            }
            Debug.LogError("unable to resolve RemoteZone to local zone");
            return null;
        }

        public int ResolveLocalZoneToRemote(CardsZone localZone)
        {
            Dictionary<int, CardsZone.LocalZoneType> zoneMap = GameStateManager.current.localPlayerZoneMap;
            return zoneMap.FirstOrDefault(x => x.Value == localZone.localZoneType).Key;
        }
        
        public void ChangePhase(string phaseKey)
        {
            // don't restart the current phase
            if (phaseKey != lastSetPhase)
            {
                if (phaseKey == KeyStrings.PreGameSetupPhase)
                {
                    lastSetPhase = phaseKey;
                    DoPreGameSetupPhase();
                }
            }
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
                myDeck.GetTopCard().SetCurrentZone(ResolveLocalZoneToRemote(myHand));
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

