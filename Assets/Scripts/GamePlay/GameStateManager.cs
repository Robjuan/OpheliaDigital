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

        [Serializable]
        public struct LabelledZone
        {
            public string name;
            public CardsZone zone;
        }

        public List<LabelledZone> labelledZones = new List<LabelledZone>();

        // filled by LocalGameManager in Start()
        [HideInInspector]
        public List<PlayerController> playerControllers = new List<PlayerController>();

        private void Awake()
        {
            bool regP = PhotonPeer.RegisterType(typeof(RP_Player), KeyStrings.RP_Player, UtilityExtensions.Serialize, UtilityExtensions.Deserialize);
            bool regC = PhotonPeer.RegisterType(typeof(RP_Card), KeyStrings.RP_Card, UtilityExtensions.Serialize, UtilityExtensions.Deserialize);
            if(!(regC&&regP))
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
        
        [PunRPC]
        public void SetupLocalCards()
        {
            localGameManager.PrepareCards();
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

            // for duplicate checking
            //List<string> allCards = new List<string>();

            foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                RP_Player rp_player = new RP_Player();
                rp_player.points = 0;
                rp_player.punActorID = player.ActorNumber; // todo: do we need this?
                
                ht = new Hashtable();
                ht.Add(player.ActorNumber.ToString(), rp_player);
                PhotonNetwork.CurrentRoom.SetCustomProperties(ht);

                var cardArray = (string[])player.CustomProperties[KeyStrings.CardList];
                List<string> cardList = cardArray.ToList();

                foreach(string card in cardList)
                {
                    RP_Card newCard = new RP_Card();
                    GameObject prefabRef = (GameObject)Resources.Load(card);

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
                    ht.Add("card_uid_" + uniqueInstanceID.ToString(), newCard);
                    PhotonNetwork.CurrentRoom.SetCustomProperties(ht);

                }

            }

            ht = new Hashtable();
            ht.Add(KeyStrings.CardLoadComplete, true);
            PhotonNetwork.CurrentRoom.SetCustomProperties(ht);

        }


        public void SetupGame()
        {
            //DealCards(initialCardCount);
        }

        private void DealCards(int numCards)
        {
            foreach(PlayerController player in playerControllers)
            {
                player.DrawCards(numCards);
            }
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.ContainsKey(KeyStrings.CardLoadComplete))
            {
                base.photonView.RPC(RPCStrings.SetupLocalCards, RpcTarget.All);
            }
        }

    }
}
