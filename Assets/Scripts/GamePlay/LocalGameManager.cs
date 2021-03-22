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

        public GameObject playerControllerPrefab;

        [HideInInspector]
        public List<PlayerController> playerControllers = new List<PlayerController>();

        private List<GameObject> allCards = new List<GameObject>();

        [Header("Display Controls")]
        public RectTransform playerDisplayPlace;
        public RectTransform opponentDisplayPlace;
        [SerializeField]
        private TMP_Text currentRoomDisplay;

        [SerializeField]
        private GameObject readyButton;

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

            gameStateManager.SetupGame();

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

        public void PrepareCards()
        {
            string cardIdentPrefix = "card_uid_";
            foreach (string key in PhotonNetwork.CurrentRoom.CustomProperties.Keys)
            {
                if(key.Contains(cardIdentPrefix))
                {
                    RP_Card rpCard = (RP_Card)PhotonNetwork.CurrentRoom.CustomProperties[key];
                    allCards.Add((GameObject)Instantiate(Resources.Load(rpCard.devName)));

                }
            }

        }
        
        // this will catch any updates to the gamestate from the GameStateManager
        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            // if my player thingo changed
            if(propertiesThatChanged.ContainsKey(PhotonNetwork.LocalPlayer.ActorNumber))
            {
                Debug.Log("local player thing changed");
            }
        }

    }
}

