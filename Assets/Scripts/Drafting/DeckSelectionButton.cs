using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

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
            // todo: visual improvements
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

    }

}
