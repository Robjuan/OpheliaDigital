using UnityEngine;
using UnityEngine.UI;

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

        private List<RoomListItem> _currentRoomList = new List<RoomListItem>();

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
                    RoomListItem roomListItem = Instantiate(_roomListItemPrefab, _content);
                    if (roomListItem != null)
                    {
                        roomListItem.SetRoomInfo(info);
                        _currentRoomList.Add(roomListItem);
                    }
                }
                
            }
        }


    }
}

