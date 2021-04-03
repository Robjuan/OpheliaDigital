using System;
using System.Collections.Generic;
using System.Linq;


using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

using TMPro;

using Hashtable = ExitGames.Client.Photon.Hashtable;


namespace Com.WhiteSwan.OpheliaDigital
{

    public class GameStateManager : MonoBehaviourPunCallbacks
    {

        public static GameStateManager current
        {
            get
            {
                if (_current == null)
                    _current = FindObjectOfType(typeof(GameStateManager)) as GameStateManager;

                return _current;
            }
            set
            {
                _current = value;
            }
        }
        private static GameStateManager _current;

        [HideInInspector]
        public List<PlayerController> playerControllers = new List<PlayerController>();

        private List<int> turnOrder;
        private BoardController boardController;

        public Dictionary<int, CardsZone.LocalZoneType> localPlayerZoneMap;
        private bool zoneMapsDone = false;

        private void Awake()
        {
            current = this;

            if (!PhotonNetwork.IsMasterClient)
            {
                // signal that we're ready (mc will see this and fire SetupTurnOrder)
                Hashtable ht = new Hashtable();
                ht.Add(KeyStrings.SceneLoaded, true);
                PhotonNetwork.LocalPlayer.SetCustomProperties(ht);
            }
        }
        
        private void SetupTurnOrder()
        {
            turnOrder = Enumerable.Range(0, PhotonNetwork.CurrentRoom.PlayerCount).ToList();
            turnOrder.Shuffle();
            foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                var newPC = SetupPlayerController(player);
                playerControllers.Add(newPC);
                Debug.Log(newPC.ToString()); 
                newPC.turnOrder = turnOrder[0];
                turnOrder.RemoveAt(0);
                var tzm = ZoneResolver.GenerateZoneMap(newPC.turnOrder); 
                photonView.RPC(StoreZoneMap_RPC_string, player, ZoneResolver.ConvertZoneMapForTransport(tzm)); // when these are all done, setupgamestate will fire

            }
            PhotonNetwork.SendAllOutgoingCommands(); // do it now (necessary???)
            LocalGameManager.current.SetupPlayerDisplay();
        }


        private const string StoreZoneMap_RPC_string = "StoreZoneMap_RPC";
        [PunRPC]
        public void StoreZoneMap_RPC(Dictionary<int, byte> dictIn)
        {
            // cache locally
            localPlayerZoneMap = ZoneResolver.ConvertZoneMapForLocal(dictIn);
            LocalGameManager.current.InitAllZoneIds();

            // store in customproperties just in case -- also signals that load is done and we can progress
            Hashtable ht = new Hashtable();
            ht.Add(KeyStrings.ZoneMap, ZoneResolver.ConvertZoneMapForTransport(localPlayerZoneMap));
            PhotonNetwork.LocalPlayer.SetCustomProperties(ht);

            
        }

        private PlayerController SetupPlayerController(Player punPlayer)
        {
            var newPlayer = PhotonNetwork.InstantiateRoomObject("PlayerController", Vector3.zero, Quaternion.identity);
            var newPC = newPlayer.GetComponent<PlayerController>();
            newPC.punPlayer = punPlayer;
            newPC.points = 0;

            return newPC;
        }

        private void SetupGameState()
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("only MC can setup gamestate");
                return;
            }

            Hashtable ht = new Hashtable();

            int uniqueInstanceID = 0;

            foreach (PlayerController player in playerControllers)
            {

                var cardArray = (string[])player.punPlayer.CustomProperties[KeyStrings.CardList];
                List<string> cardList = cardArray.ToList();

                // hardcoding these values here because the zonemap is not ready when we're here.
                // values come from ZoneResolver.
                int thisPlayerDeck;
                if (player.turnOrder == 0)
                {
                    thisPlayerDeck = 2;
                }
                else
                {
                    thisPlayerDeck = 5;
                }

                foreach (string card in cardList)
                {
                    var justAboveBoard = new Vector3(0, 0, -5);
                    uniqueInstanceID++;

                    object[] initData = new object[3];
                    initData[0] = thisPlayerDeck;
                    initData[1] = uniqueInstanceID;
                    initData[2] = card;

                    GameObject newCard = PhotonNetwork.InstantiateRoomObject(card, justAboveBoard, Quaternion.identity, 0, initData); // will be immediately moved when we put it in the deck
                    CardController newCardCC = newCard.GetComponent<CardController>();
                }

            }

            GameObject board = PhotonNetwork.InstantiateRoomObject("BoardController", Vector3.zero, Quaternion.identity);
            boardController = board.GetComponent<BoardController>();
            boardController.currentPhase = KeyStrings.LoadPhase;
            boardController.currentRound = 0;

            boardController.UpdatePhase(KeyStrings.PreGameSetupPhase);

        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (changedProps.ContainsKey(KeyStrings.SceneLoaded))
                {
                    SetupTurnOrder();
                }

                if (changedProps.ContainsKey(KeyStrings.PhaseReady))
                {
                    bool everyoneReady = true;
                    foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
                    {
                        if (player.CustomProperties.ContainsKey(KeyStrings.PhaseReady) && (bool)player.CustomProperties[KeyStrings.PhaseReady] == false) // if any player is not ready
                        {
                            everyoneReady = false;
                            break;
                        }
                    }
                    if(everyoneReady)
                    {
                        // go to the next phase
                        boardController.UpdatePhase(KeyStrings.PreGameSetupPhase); // todo: dynamic next-phasing
                    }

                }
                // don't do this ever again (will otherwise again and see ready on phasechange etc)
                if (changedProps.ContainsKey(KeyStrings.ZoneMap) && !zoneMapsDone)
                {
                    Debug.Log("checking zonemaps");
                    bool everyoneReady = true;
                    foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
                    {
                        
                        if (player.CustomProperties[KeyStrings.ZoneMap] == null) // if any player is not ready
                        {
                            everyoneReady = false;
                            break;
                        }
                    }
                    if (everyoneReady)
                    {
                        Debug.Log("firing setupgamestate");
                        zoneMapsDone = true;
                        SetupGameState();
                    }
                }
            }
        }

    }
}
