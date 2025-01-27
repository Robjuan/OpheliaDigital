﻿using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

using TMPro;

using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Linq;

namespace Com.WhiteSwan.OpheliaDigital
{
    public class DraftTurnManager : MonoBehaviourPunCallbacks
    {
        // RPCs
        public const string StartTurn_string = "StartTurn";
        public const string EndTurn_string = "EndCurrentTurn_MasterClient";
        public const string DeactivateReadyButton_string = "DeactivateReadyButton";

        [SerializeField]
        private GameObject deckSelectionPlace;
        [SerializeField]
        private GameObject deckSelectionButtonPrefab;

        [SerializeField]
        private Button readyButton; // text will initially be "Ready"

        private int activeTurnPlayer = 0;
        private List<Player> turnOrderPlayers;

        private bool amReady = false;
        private bool showingPreconUI = false;

        private List<GameObject> deckButtons = new List<GameObject>();

        private void Awake()
        {
            // TODO: have this selectable when you create room
            // should then show if it's precon or draft in the room list
            Hashtable ht = new Hashtable();
            ht.Add(KeyStrings.DraftType, KeyStrings.Precon);
            PhotonNetwork.CurrentRoom.SetCustomProperties(ht);

            ht = new Hashtable();
            ht.Add(KeyStrings.Ready, amReady);
            PhotonNetwork.LocalPlayer.SetCustomProperties(ht);


            // todo: get available decks and set in room properties
            // todo: where are we getting these from / how are we deciding this

            Dictionary<string, bool> availDecks = new Dictionary<string, bool>();
            availDecks.Add(KeyStrings.Yucatec, true);
            availDecks.Add(KeyStrings.Mechanicus, true);
            availDecks.Add(KeyStrings.Mattervoid, true);

            ht = new Hashtable();
            ht.Add(KeyStrings.AvailableDecks, availDecks);
            PhotonNetwork.CurrentRoom.SetCustomProperties(ht);


        }

        void Start()
        {
            readyButton.gameObject.SetActive(true);
            GameEvents.current.onDeckSelected += EndCurrentTurn;

        }

        public void ToggleReady()
        {
            amReady = !amReady;

            Hashtable ht = new Hashtable();
            ht.Add(KeyStrings.Ready, amReady);
            PhotonNetwork.LocalPlayer.SetCustomProperties(ht);

            if(amReady == true)
            {
                readyButton.GetComponentInChildren<TMP_Text>().text = "Unready";
            }
            else
            {
                readyButton.GetComponentInChildren<TMP_Text>().text = "Ready";
            }
        }

        private void StartDraft()
        {
            base.photonView.RPC(DeactivateReadyButton_string, RpcTarget.All);
            SetSelectionOrder();
        }

        [PunRPC]
        private void DeactivateReadyButton()
        {
            readyButton.GetComponentInChildren<TMP_Text>().text = "All Players Ready";
            readyButton.interactable = false;
            // todo: more visual feedback
        }

        private void SetSelectionOrder()
        {
            // only called by the masterclient

            // create a list of players
            turnOrderPlayers = new List<Player>(PhotonNetwork.CurrentRoom.Players.Values);

            // shuffle the list
            turnOrderPlayers.Shuffle();

            // set the active turn player
            SetActiveTurnPlayer(activeTurnPlayer); // init at 0

        }

        private void SetActiveTurnPlayer(int turnOrder)
        {
            base.photonView.RPC(StartTurn_string, turnOrderPlayers[turnOrder]);
        }

        public void EndCurrentTurn()
        {
            // called by onDeckSelected event
            DeactivatePreconSelection();
            base.photonView.RPC(EndTurn_string, RpcTarget.MasterClient);
        }

        [PunRPC]
        public void EndCurrentTurn_MasterClient()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("Master Client only function called on non-master");
                return;
            }

            // the last player finished their turn
            if (activeTurnPlayer + 1 > turnOrderPlayers.Count - 1) // zero based list
            {
                // set cardlist for each player
                foreach(Player player in turnOrderPlayers)
                {
                    string selectedDeck = (string)player.CustomProperties[KeyStrings.ChosenDeck];

                    // https://doc.photonengine.com/en/realtime/current/reference/serialization-in-photon
                    // can't use lists for customproperties
                    string[] cardList = GetPreconCardList(selectedDeck).ToArray();

                    Hashtable ht = new Hashtable();
                    ht.Add(KeyStrings.CardList, cardList);
                    player.SetCustomProperties(ht);

                }

            } else
            {
                activeTurnPlayer += 1;
                Debug.LogFormat("Progressing to next player: {0}", activeTurnPlayer);
                SetActiveTurnPlayer(activeTurnPlayer);
            }
        }

        public List<string> GetPreconCardList(string factionName)
        {
            var file = Resources.Load("Cards/" + factionName + "_generated_card_list") as TextAsset;
            List<string> cardList = new List<string>(file.text.Split(','));
            return cardList;
        }

        [PunRPC]
        public void StartTurn()
        {
            // only activate those that are available
            Dictionary<string, bool> decks = (Dictionary<string, bool>)PhotonNetwork.CurrentRoom.CustomProperties[KeyStrings.AvailableDecks];
            foreach (string deck in decks.Keys)
            {
                if(decks[deck] == false)
                {
                    // don't activate button if the deck isn't avail
                    continue;
                }
                foreach(GameObject button in deckButtons)
                {
                    if(button.GetComponent<DeckSelectionButton>().targetDeck == deck)
                    {
                        button.GetComponent<Button>().interactable = true;
                    }
                }
            }
        }

        private void ShowPreconSelectionUI()
        {
            // present all buttons
            Dictionary<string, bool> decks = (Dictionary<string, bool>)PhotonNetwork.CurrentRoom.CustomProperties[KeyStrings.AvailableDecks];
            foreach (string deck in decks.Keys)
            {
                GameObject deckButton = Instantiate(deckSelectionButtonPrefab, deckSelectionPlace.transform);
                deckButton.GetComponentInChildren<TMP_Text>().text = deck;
                deckButton.GetComponent<DeckSelectionButton>().targetDeck = deck;
                deckButton.GetComponent<Button>().interactable = false;
                deckButtons.Add(deckButton);
            }
        }

        private void DeactivatePreconSelection()
        {
            foreach(GameObject button in deckButtons)
            {
                button.GetComponent<Button>().interactable = false;
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            // if someone readies or unreadies, only MC needs to check
            if (PhotonNetwork.IsMasterClient)
            {
                if (changedProps.ContainsKey(KeyStrings.Ready))
                {
                    bool everyoneReady = true;
                    foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
                    {
                        if (player.CustomProperties.ContainsKey(KeyStrings.Ready) && (bool)player.CustomProperties[KeyStrings.Ready] == false) // if any player is not ready
                        {
                            everyoneReady = false;
                            break;
                        }
                    }

                    if (everyoneReady)
                    {
                        if (!(PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(KeyStrings.DraftType)))
                        {
                            Debug.LogError("Room Draft type not set");
                            return;
                        }
                        else
                        {
                            if ((string)PhotonNetwork.CurrentRoom.CustomProperties[KeyStrings.DraftType] == KeyStrings.Precon)
                            {
                                StartDraft();
                            }
                            else if ((string)PhotonNetwork.CurrentRoom.CustomProperties[KeyStrings.DraftType] == KeyStrings.FullDraft)
                            {
                                Debug.LogError("full draft is not yet supported");
                                return;
                            }
                        }

                    }
                }

                // if player cardlist set
                if (changedProps.ContainsKey(KeyStrings.CardList) && changedProps[KeyStrings.CardList] != null)
                {
                    bool everyoneCardListSet = true;
                    foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
                    {
                        // if any player does not have their cardlist set
                        if (!player.CustomProperties.ContainsKey(KeyStrings.CardList) || player.CustomProperties[KeyStrings.CardList] == null)
                        {
                            everyoneCardListSet = false;
                            break;
                        }
                    }

                    if (everyoneCardListSet)
                    {
                        PhotonNetwork.LoadLevel("GameScene");
                    }

                }
            }

            

            // if i've chosen my deck
            if (targetPlayer == PhotonNetwork.LocalPlayer && changedProps.ContainsKey(KeyStrings.ChosenDeck))
            {
                // this will end my turn also
                GameEvents.current.DeckSelected();
            }

        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if(propertiesThatChanged.ContainsKey(KeyStrings.AvailableDecks) && !showingPreconUI)
            {
                ShowPreconSelectionUI();
                showingPreconUI = true;
            }
        }

    }
}
