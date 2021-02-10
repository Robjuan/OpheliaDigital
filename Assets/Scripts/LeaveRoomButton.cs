using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Photon.Pun;

namespace Com.WhiteSwan.OpheliaDigital
{
    public class LeaveRoomButton : MonoBehaviourPunCallbacks
    {
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
            base.OnLeftRoom();
        }
    }
}
