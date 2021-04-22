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
        [SerializeField]
        private TMP_Text phaseDisplayText;

        private string passButtonText_EndPhase = "End Phase";
        private string passButtonText_Priority = "Pass Priority";
        private string passButtonText_Waiting = "Waiting For Opponent...";

        public List<CardEffectBase> allEffects;

        private bool selfCardPlayedThisTurn; // for passing and ending turn

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
                if (PhotonNetwork.LocalPlayer.ActorNumber == player.punActorNumber)
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
                foreach(var thing in GameStateManager.current.chain)
                {
                    Debug.Log(thing);
                }

                // is the chain empty?
                if(GameStateManager.current.chain.Count == 0)
                {
                    passPriorityButton.GetComponentInChildren<TMP_Text>().text = passButtonText_EndPhase;
                } else
                {
                    // change the button here based on the chain item
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

            // TODO: UI CONTROLLER
            // if i haven't played a card this round, and we're in the final 
            if (GameStateManager.current.boardController.currentPhase == BoardController.Phase.ActionTwo && !selfCardPlayedThisTurn)
            {
                Debug.LogWarning("this is your last opportunity to play a Character Card - otherwise you will pass at end of turn");
            }
            if (GameStateManager.current.boardController.currentPhase == BoardController.Phase.End && !selfCardPlayedThisTurn)
            {
                Debug.LogWarning("this is your last opportunity to play a Turning Point Card - otherwise you will pass at end of turn");
            }

            GameStateManager.current.PassPriority();

        }

        public void ChangePhase(BoardController.Phase newPhase)
        {
            switch (newPhase)
            {
                case BoardController.Phase.PreGameSetupPhase:
                    Debug.Log("changing to PGSP");
                    DoPreGameSetupPhase();
                    break;
            }
            phaseDisplayText.text = newPhase.ToString();
            lastSetPhase = newPhase;
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

