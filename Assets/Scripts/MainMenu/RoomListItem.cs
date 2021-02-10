using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

using TMPro;
using UnityEngine.EventSystems;

namespace Com.WhiteSwan.OpheliaDigital
{
    public class RoomListItem : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private TMP_Text _roomText;
        [SerializeField]
        private TMP_Text _roomNumber;

        // set by RoomLister on instantiate
        [HideInInspector]
        public JoinRoomButton joinRoomButton;
        public RoomLister parentLister;

        public RoomInfo RoomInfo { get; private set; }

        private RawImage image;
        private Color startingColor;
        private Color selectedColor = new Color32(115, 115, 154, 255);

        void Awake()
        {
            image = this.GetComponent<RawImage>();
            startingColor = image.color;
        }

        public void ResetColor()
        {
            image.color = startingColor;
        }

        public void SetRoomInfo(RoomInfo roomInfo)
        {
            _roomText.text = roomInfo.Name;
            _roomNumber.text = roomInfo.PlayerCount.ToString();

            RoomInfo = roomInfo;
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            joinRoomButton.interactable = true;
            joinRoomButton.targetRoomName = RoomInfo.Name;
            parentLister.ResetSelection();
            image.color = selectedColor;
        }

        // need to implement these to prevent the event going somewhere else (like the scrollrect)
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData){}
        void IPointerUpHandler.OnPointerUp(PointerEventData eventData){}
    }
}
