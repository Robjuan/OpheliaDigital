using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

using TMPro;

namespace Com.WhiteSwan.OpheliaDigital
{
    public class RoomListItem : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _roomText;
        [SerializeField]
        private TMP_Text _roomNumber;

        public RoomInfo RoomInfo { get; private set; }

        public void SetRoomInfo(RoomInfo roomInfo)
        {
            _roomText.text = roomInfo.Name;
            _roomNumber.text = roomInfo.PlayerCount.ToString();

            RoomInfo = roomInfo;
        }

    }
}
