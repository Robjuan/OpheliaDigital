using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

using TMPro;

using Hashtable = ExitGames.Client.Photon.Hashtable;


namespace Com.WhiteSwan.OpheliaDigital
{
    public class GameFlowManager : MonoBehaviourPunCallbacks
    {

        private Room room;
        private Player firstReadyPlayer = null;

        


        void Start()
        {
            room = PhotonNetwork.CurrentRoom;
        }

        void StartGame()
        {

        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            // fire when players say they're ready (this will need updating for unreadying)
            if (changedProps.ContainsKey(KeyStrings.Ready))
            {
                if (firstReadyPlayer == null)
                {
                    firstReadyPlayer = targetPlayer;
                }
                else
                {
                    // a second player has readied, start game
                    StartGame();
                }
            }
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {


        }
    }
}
