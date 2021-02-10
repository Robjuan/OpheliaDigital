using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Photon.Pun;
using Photon.Realtime;

using System.Collections;
using System.Collections.Generic;

namespace Com.WhiteSwan.OpheliaDigital
{
    public class RoomLister : MonoBehaviourPunCallbacks
    {

        [SerializeField]
        private Transform _content;

        [SerializeField]
        private RoomListItem _roomListItemPrefab;

        [SerializeField]
        private JoinRoomButton joinRoomButton;

        private List<RoomListItem> _currentRoomList = new List<RoomListItem>();

        public void ResetSelection()
        {
            foreach(RoomListItem roomListItem in _currentRoomList)
            {
                roomListItem.ResetColor();
            }
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            foreach (RoomInfo info in roomList)
            {
                if (info.RemovedFromList)
                {
                    // room names are unique
                    int index = _currentRoomList.FindIndex(x => x.RoomInfo.Name == info.Name);
                    if (index != -1)
                    {
                        Destroy(_currentRoomList[index].gameObject);
                        _currentRoomList.RemoveAt(index);
                    }

                }   
                else 
                {
                    int index = _currentRoomList.FindIndex(x => x.RoomInfo.Name == info.Name);
                    if (index == -1)
                    {
                        RoomListItem roomListItem = Instantiate(_roomListItemPrefab, _content);
                        if (roomListItem != null)
                        {
                            roomListItem.SetRoomInfo(info);
                            _currentRoomList.Add(roomListItem);

                            roomListItem.joinRoomButton = joinRoomButton;
                            roomListItem.parentLister = this;
                        }
                    }
                    else
                    {
                        // modify list ? player val change etc
                    }
                }
                
            }
        }


    }
}

