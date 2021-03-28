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

        public LocalGameManager localGameManager;

        [Header("Game Rule Constants")]
        public int initialCardCount = 9;
        public int roundCardDrawMax = 3;

        // filled by LocalGameManager in Start()
        [HideInInspector]
        public List<PlayerController> playerControllers = new List<PlayerController>();

        private void Awake()
        {
            bool regP = PhotonPeer.RegisterType(typeof(RP_Player), KeyStrings.RP_Player, UtilityExtensions.Serialize, UtilityExtensions.Deserialize);
            bool regC = PhotonPeer.RegisterType(typeof(RP_Card), KeyStrings.RP_Card, UtilityExtensions.Serialize, UtilityExtensions.Deserialize);
            bool regB = PhotonPeer.RegisterType(typeof(RP_Board), KeyStrings.RP_Board, UtilityExtensions.Serialize, UtilityExtensions.Deserialize);
            if (!(regC&&regP&&regB))
            {
                Debug.LogError("custom types could not be registered");
            }

            if (localGameManager == null)
            {
                Debug.LogError("manager setup no good");
            }

            if (PhotonNetwork.IsMasterClient)
            {
                SetupGameStateRoomProperties();
            }
        }
        
        private void SetupGameStateRoomProperties()
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                Debug.LogWarning("only MC can setup gamestate RPs");
                return;
            }

            Hashtable ht = new Hashtable();

            int uniqueInstanceID = 0;

            var turnSeq = Enumerable.Range(0, PhotonNetwork.CurrentRoom.PlayerCount).ToList();
            turnSeq.Shuffle();

            foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
            {

                RP_Player rp_player = new RP_Player();
                rp_player.points = 0;
                rp_player.punActorID = player.ActorNumber; // todo: do we need this?
                
                rp_player.turnOrder = turnSeq[0];
                turnSeq.RemoveAt(0);


                ht = new Hashtable();
                ht.Add(KeyStrings.ActorPrefix + player.ActorNumber.ToString(), rp_player);
                PhotonNetwork.CurrentRoom.SetCustomProperties(ht);

                var cardArray = (string[])player.CustomProperties[KeyStrings.CardList];
                List<string> cardList = cardArray.ToList();
                

                foreach(string card in cardList)
                {
                    RP_Card newCard = new RP_Card();
                    GameObject prefabRef = (GameObject)Resources.Load(card);

                    // need to put card in particular player's deck
                    newCard.zoneLocation = (CardsZone.RP_ZoneType.Deck, player.ActorNumber);

                    newCard.instanceID = uniqueInstanceID;
                    uniqueInstanceID++;
                    newCard.devName = card;

                    CardController prefabCC = prefabRef.GetComponent<CardController>();
                    newCard.faction = prefabCC.faction;

                    CharacterCardController prefabCCC = prefabRef.GetComponent<CharacterCardController>();
                    if (prefabCCC != null)
                    {
                        // character card
                        newCard.power = prefabCCC.basePower;
                        newCard.initiative = prefabCCC.baseInitiative;
                        newCard.armour = prefabCCC.baseArmour;
                        newCard.life = prefabCCC.baseLife;
                    }
                    else
                    {
                        // tp card
                        TurningPointCardController prefabTPC = prefabRef.GetComponent<TurningPointCardController>();                        
                        // tp cards will onyl require zone tracking
                    }

                    ht = new Hashtable();
                    ht.Add(KeyStrings.CardIdentPrefix + uniqueInstanceID.ToString(), newCard);
                    PhotonNetwork.CurrentRoom.SetCustomProperties(ht);

                }

            }


            RP_Board board = new RP_Board();
            board.currentPhase = KeyStrings.PreGameSetupPhase;
            board.currentRound = 0;
            ht = new Hashtable();
            ht.Add(KeyStrings.RP_Board, board);
            PhotonNetwork.CurrentRoom.SetCustomProperties(ht);


            ht = new Hashtable();
            ht.Add(KeyStrings.CardLoadComplete, true);
            PhotonNetwork.CurrentRoom.SetCustomProperties(ht);

        }


        public void SetupGame()
        {
            //DealCards(initialCardCount);
        }

        public bool UpdateCardLocation(CardController cardToMove, CardsZone source, CardsZone destination)
        {
            /*Hashtable ht = new Hashtable();
            ht.Add(KeyStrings.CardIdentPrefix + cardToMove.RP_instanceID);
            PhotonNetwork.CurrentRoom.SetCustomProperties(ht, expected_ht);
            */
            return true;

        }



        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.ContainsKey(KeyStrings.CardLoadComplete))
            {
                localGameManager.PrepareCards();
            }
        }

    }
}
