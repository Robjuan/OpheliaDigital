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
        [HideInInspector]
        public Dictionary<int, CardsZone.LocalZoneType> localPlayerZoneMap;

        // these are just for convenience internally here
        private CardsZone myDeck;
        private CardsZone myHand;

        [Header("Prefab Refs")]
        public GameObject playerControllerPrefab;

        [Header("Display Controls")]
        public RectTransform playerDisplayPlace;
        public RectTransform opponentDisplayPlace;
        [SerializeField]
        private TMP_Text currentRoomDisplay;
        [SerializeField]
        private Button passPriorityButton; // eventually may need to build out a UI manager or something

        private string passButtonText_Phase = "End Phase";
        private string passButtonText_Priority = "Pass Priority";
        private string passButtonText_Waiting = "Waiting For Opponent...";

        private string lastSetPhase;

        public List<CardEffectBase> allEffects;

        private void Awake()
        {
            current = this;
            GameEvents.current.onPhaseChange += ChangePhase;
            GameEvents.current.onPriorityChange += PriorityChange;
            passPriorityButton.enabled = false;

            allEffects = FindObjectsOfType<CardEffectBase>().ToList();
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
            Dictionary<int, CardsZone.LocalZoneType> zoneMap = localPlayerZoneMap;
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
            Dictionary<int, CardsZone.LocalZoneType> zoneMap = localPlayerZoneMap;
            return zoneMap.FirstOrDefault(x => x.Value == localZone.localZoneType).Key;
        }
        
        public void PriorityChange(int newPrioAN)
        {
            if(newPrioAN == -1)
            {
                Debug.LogError("changing prio player to -1 (unset)");
                return;
            }
            if(newPrioAN == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                // priority is now mine

                // display "pass priority button"
                passPriorityButton.interactable = true;

                // is the chain empty?
                if(GameStateManager.current.chain.Count == 0)
                {
                    passPriorityButton.GetComponentInChildren<TMP_Text>().text = passButtonText_Phase;
                } else
                {
                    passPriorityButton.GetComponentInChildren<TMP_Text>().text = passButtonText_Priority;
                }
                

                // todo: highlight available actions
            }
            else // someone else's turn
            { 
                // todo: better visuals
                passPriorityButton.interactable = false;
                passPriorityButton.GetComponentInChildren<TMP_Text>().text = passButtonText_Waiting;
            }
        }

        public void PassPriority()
        {
            // im done
            GameStateManager.current.PassPriority();

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
                Debug.LogWarning("attemping to change to current phase: " + phaseKey);
            }
        }

        private void DoPreGameSetupPhase()
        {
            Debug.Log("doing pregame");
            // draw cards     
            DrawCards(GamePlayConstants.InitialDrawCount);
            passPriorityButton.enabled = true;
        }

        private bool DrawCards(int numCards)
        {
            for (int i = 0; i < numCards; i++)
            {
                var tryCard = myDeck.GetCardBelowTop(i);
                if(tryCard != null)
                {
                    tryCard.SetCurrentZone(ResolveLocalZoneToRemote(myHand));
                } else
                {
                    return false;
                }

            }
            return true;
        }
    }
}

