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

        [Header("Game Rule Constants")]
        public int initialCardCount = 9;
        public int roundCardDrawMax = 3;

        // filled by LocalGameManager in Start()
        [HideInInspector]
        public List<PlayerController> playerControllers = new List<PlayerController>();

        private List<int> turnOrder;
        private BoardController boardController;

        public Dictionary<int, CardsZone.LocalZoneType> localPlayerZoneMap;

        private void Awake()
        {
            current = this;

            if (PhotonNetwork.IsMasterClient)
            {
                SetupTurnOrder();
            }
        }
        
        private void SetupTurnOrder()
        {
            turnOrder = Enumerable.Range(0, PhotonNetwork.CurrentRoom.PlayerCount).ToList();
            turnOrder.Shuffle();
            foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                var newPC = SetupPlayerController(player);
                newPC.turnOrder = turnOrder[0];
                turnOrder.RemoveAt(0);
                var tzm = ZoneResolver.GenerateZoneMap(newPC.turnOrder); // when these are all done, setupgamestate will fire
                photonView.RPC(StoreZoneMap_RPC_string, player, ZoneResolver.ConvertZoneMapForTransport(tzm));

            }
            PhotonNetwork.SendAllOutgoingCommands(); // do it now
            LocalGameManager.current.SetupPlayerDisplay();
        }


        private const string StoreZoneMap_RPC_string = "StoreZoneMap_RPC";
        [PunRPC]
        public void StoreZoneMap_RPC(Dictionary<int, byte> dictIn)
        {
            // cache locally
            localPlayerZoneMap = ZoneResolver.ConvertZoneMapForLocal(dictIn);
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

            LocalGameManager.current.playerControllers.Add(newPC);

            return newPC;
        }

        private void SetupGameState()
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                Debug.LogWarning("only MC can setup gamestate RPs");
                return;
            }

            Hashtable ht = new Hashtable();

            int uniqueInstanceID = 0;

            foreach (PlayerController player in LocalGameManager.current.playerControllers)
            {
                var cardArray = (string[])player.punPlayer.CustomProperties[KeyStrings.CardList];
                List<string> cardList = cardArray.ToList();
              
                foreach(string card in cardList)
                {

                    GameObject newCard = PhotonNetwork.InstantiateRoomObject(card, Vector3.zero, Quaternion.identity); // will be immediately moved when we put it in the deck
                    CardController newCardCC = newCard.GetComponent<CardController>();
                    // put card in particular player's deck

                    // hardcoding these values here because the zonemap is not ready when we're here.
                    // values come from ZoneResolver.
                    int thisPlayerDeck;
                    if(player.turnOrder == 0)
                    {
                        thisPlayerDeck = 2;
                    }
                    else
                    {
                        thisPlayerDeck = 5;
                    }

                    newCardCC.SetCurrentZone(thisPlayerDeck);

                    newCardCC.instanceID = uniqueInstanceID;
                    uniqueInstanceID++;
                    newCardCC.devName = card;

                }

            }

            GameObject board = PhotonNetwork.InstantiateRoomObject("BoardController", Vector3.zero, Quaternion.identity);
            boardController = board.GetComponent<BoardController>();
            boardController.currentPhase = KeyStrings.LoadPhase;
            boardController.currentRound = 0;

            boardController.UpdatePhase(KeyStrings.PreGameSetupPhase);

        }


        private const string LGM_InitZones_RPCString = "LGM_InitZones";
        [PunRPC]
        private void LGM_InitZones()
        {
            LocalGameManager.current.InitAllZoneIds();
        }

        public void SetupGame()
        {
            //DealCards(initialCardCount);
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (PhotonNetwork.IsMasterClient)
            {
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
                if (changedProps.ContainsKey(KeyStrings.ZoneMap))
                {
                    photonView.RPC(LGM_InitZones_RPCString, targetPlayer);

                    bool everyoneReady = true;
                    foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
                    {
                        if (player.CustomProperties.ContainsKey(KeyStrings.ZoneMap) && player.CustomProperties[KeyStrings.PhaseReady] != null) // if any player is not ready
                        {
                            everyoneReady = false;
                            break;
                        }
                    }
                    if (everyoneReady)
                    {
                        SetupGameState();
                    }
                }
            }
        }

    }
}
