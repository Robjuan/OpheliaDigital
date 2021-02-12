using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

using Hashtable = ExitGames.Client.Photon.Hashtable;


namespace Com.WhiteSwan.OpheliaDigital
{
    public class DeckSelectionButton : MonoBehaviourPunCallbacks
    {

        // set by DraftTurnManager on instantiation
        [HideInInspector]
        public string targetDeck;

        public void Start()
        {
            // go and get image based on targetDeck string
            // load from resources folder
            
            
        }

        public void SelectDeck()
        {
            // set the deck unavailable in roomproperties
            Dictionary<string, bool> availDecks = (Dictionary<string,bool>)PhotonNetwork.CurrentRoom.CustomProperties[KeyStrings.AvailableDecks];
            availDecks[targetDeck] = false;
            Hashtable ht = new Hashtable();
            ht.Add(KeyStrings.AvailableDecks,availDecks);
            PhotonNetwork.CurrentRoom.SetCustomProperties(ht);

            // set the localplayer property "chosen deck" as the deck
            ht = new Hashtable();
            ht.Add(KeyStrings.ChosenDeck, targetDeck);
            PhotonNetwork.LocalPlayer.SetCustomProperties(ht);

            base.GetComponent<Button>().interactable = false;

        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            // don't fire the deck selected event until the player property is set (and only care about local)
            if(targetPlayer == PhotonNetwork.LocalPlayer && changedProps.ContainsKey(KeyStrings.ChosenDeck))
            {
                // fire the onDeckSelected event for my local draftTurnManager
                // this will end my turn also
                GameEvents.current.DeckSelected();
            }

        }

    }

}
