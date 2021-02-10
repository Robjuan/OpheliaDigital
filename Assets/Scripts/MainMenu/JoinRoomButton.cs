using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

namespace Com.WhiteSwan.OpheliaDigital
{
    public class JoinRoomButton : Button
    {
        [HideInInspector]
        public string targetRoomName;

        private Color startingColor;

        public void Awake()
        {
            // will be set interactible when a room is selected
            this.interactable = false;
            startingColor = this.image.color;
        }

        public void JoinTargetRoom()
        {
            if (!string.IsNullOrEmpty(targetRoomName))
            {
                Debug.Log("Joining target room: " +targetRoomName);
                PhotonNetwork.JoinRoom(targetRoomName);
            }
        }
    }
}


