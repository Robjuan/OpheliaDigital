using System;
using System.Collections.Specialized;
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
        [HideInInspector]
        public BoardController boardController;

        private List<int> turnOrder;
        private int priorityPlayerAN = -1;
        private bool zoneMapsDone = false;

        [HideInInspector]
        public List<IChainable> chain = new List<IChainable>(); // objects must implement IChainable


        private void Awake()
        {
            current = this;

            if (!PhotonNetwork.IsMasterClient)
            {
                // signal that we're ready (mc will see this and fire SetupTurnOrder)
                Hashtable ht = new Hashtable();
                ht.Add(KeyStrings.SceneLoaded, true);
                PhotonNetwork.LocalPlayer.SetCustomProperties(ht);
            } else if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                // testing only
                SetupTurnOrder();
            }
            GameEvents.current.onPriorityChange += ReceivePriority;

        }
        
        private void SetupTurnOrder()
        {
            turnOrder = Enumerable.Range(0, PhotonNetwork.CurrentRoom.PlayerCount).ToList();
            turnOrder.Shuffle();
            foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                var newPC = SetupPlayerController(player);
                //playerControllers.Add(newPC); onPUNinstantiate on PC obj should add it to this;
                Debug.Log(newPC.ToString()); 
                newPC.turnOrder = turnOrder[0];
                turnOrder.RemoveAt(0);
                var tzm = ZoneResolver.GenerateZoneMap(newPC.turnOrder); 
                photonView.RPC(StoreZoneMap_string, player, ZoneResolver.ConvertZoneMapForTransport(tzm)); // when these are all done, setupgamestate will fire

            }

            PlayerController firstPlayer = playerControllers.Where(z => z.turnOrder == playerControllers.Min(x => x.turnOrder)).First();
            photonView.RPC(UpdatePrioPlayer_string, RpcTarget.AllViaServer, firstPlayer.punActorNumber);

            photonView.RPC(SetupPlayerDisplay_string, RpcTarget.AllViaServer);
        }

        private const string SetupPlayerDisplay_string = "SetupPlayerDisplay";
        [PunRPC]
        public void SetupPlayerDisplay()
        {
            LocalGameManager.current.SetupPlayerDisplay();
        }


        private const string UpdatePrioPlayer_string = "UpdatePrioPlayer";
        [PunRPC]
        public void UpdatePrioPlayer(int ppAN)
        {
            priorityPlayerAN = ppAN;
            GameEvents.current.PriorityChange(priorityPlayerAN);
        }

        private const string AddToChain_string = "AddToChain";
        [PunRPC]
        public void AddToChain(int chainObjInstanceID)
        {
            if(chainObjInstanceID == -1)
            {
                chain.Add(new PhaseEndRequest(priorityPlayerAN));
            }
            else
            {
                chain.Add(LocalGameManager.current.allEffects.Where(x => x.instanceID == chainObjInstanceID).First());
            }
            
        }


        // called when we GIVE priority to someone
        public void PassPriority()
        {
            if(chain.Count == 0)
            {
                // nothing on the chain and we're passing prio - that means Phase End
                photonView.RPC(AddToChain_string, RpcTarget.AllViaServer, -1);
            }

            // this will get the first player who doesn't currently have prio (won't scale to > 2 players) 
            // will also break when there is only one player 
            PlayerController newPP = playerControllers.Where(x => x.punActorNumber != PhotonNetwork.LocalPlayer.ActorNumber).First();
            photonView.RPC(UpdatePrioPlayer_string, RpcTarget.AllViaServer, newPP.punActorNumber);

        }

        public void ReceivePriority(int newPrioPlayer)
        {
            int myAN = PhotonNetwork.LocalPlayer.ActorNumber;
            if (newPrioPlayer != myAN)
            {
                // not my prio to receive
                return;
            }
            if(chain.Count > 0)
            {
                IChainable mostRecent = chain.Last();
                if (mostRecent.ownerAN == myAN)
                {
                    // they have given back to me without doing anything
                    ResolveChain();
                }
                else
                {
                    // they have added to the chain 
                }
            }
            else
            {
                Debug.Log("<color=teal>received prio with nothing on chain</color>");
            }
            
        }

        private void ResolveChain()
        {
            // work backwards through the list
            for (int i = chain.Count - 1; i >= 0; i--) // remove 1 to convert count to index
            {
                chain[i].Resolve();
                
                chain.RemoveAt(i);

                // todo: check for things that have triggered as a result of a chain item resolving
            }
        }


        private const string StoreZoneMap_string = "StoreZoneMap";
        [PunRPC]
        public void StoreZoneMap(Dictionary<int, byte> dictIn)
        {
            // cache locally
            LocalGameManager.current.localPlayerZoneMap = ZoneResolver.ConvertZoneMapForLocal(dictIn);
            LocalGameManager.current.InitAllZoneIds();

            // store in customproperties just in case -- also signals that load is done and we can progress
            Hashtable ht = new Hashtable();
            ht.Add(KeyStrings.ZoneMap, ZoneResolver.ConvertZoneMapForTransport(LocalGameManager.current.localPlayerZoneMap));
            PhotonNetwork.LocalPlayer.SetCustomProperties(ht);
        }

        private PlayerController SetupPlayerController(Player punPlayer)
        {
            object[] initData = new object[1];
            initData[0] = punPlayer.ActorNumber;

            var newPlayer = PhotonNetwork.InstantiateRoomObject("PlayerController", Vector3.zero, Quaternion.identity, 0, initData);
            var newPC = newPlayer.GetComponent<PlayerController>();

            return newPC;
        }

        private void SetupGameState()
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("only MC can setup gamestate");
                return;
            }

            int uniqueInstanceID = 0;

            foreach (PlayerController player in playerControllers)
            {
                Player punPlayer = PhotonNetwork.CurrentRoom.Players[player.punActorNumber];
                var cardArray = (string[])punPlayer.CustomProperties[KeyStrings.CardList];
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
                }

            }

            GameObject board = PhotonNetwork.InstantiateRoomObject("BoardController", Vector3.zero, Quaternion.identity);
            boardController.UpdatePhase(BoardController.Phase.PreGameSetupPhase);

        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (changedProps.ContainsKey(KeyStrings.SceneLoaded))
                {
                    SetupTurnOrder();
                }

                // don't do this ever again (will otherwise again and see ready on phasechange etc)
                if (changedProps.ContainsKey(KeyStrings.ZoneMap) && !zoneMapsDone)
                {
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
                        zoneMapsDone = true;
                        SetupGameState();
                    }
                }
                
            }
        }

    }
}
