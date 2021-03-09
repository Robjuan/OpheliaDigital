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
    public class GameRulesManager : MonoBehaviourPunCallbacks
    {

        void StartGame()
        {

        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
        }
    }
}
