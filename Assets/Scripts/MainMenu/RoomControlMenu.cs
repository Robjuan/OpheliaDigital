using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Photon.Realtime;
using Photon.Pun;

namespace Com.WhiteSwan.OpheliaDigital
{
    public class RoomControlMenu : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private TMP_InputField _createRoomName;

        [SerializeField]
        public GameObject connectingLabel;
        public GameObject menuInput;

        private string gameVersion = "0.0.1";

        public void Awake()
        {

            // todo: separate out system setting stuff
            Screen.SetResolution(1024, 768, false);

            PhotonNetwork.AutomaticallySyncScene = true;
            //isConnecting = 
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;


            connectingLabel.SetActive(false);
            menuInput.SetActive(true);

        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby(); // does this lobby show me room lists
        }

        public void CreateRoom()
        {
            if (!PhotonNetwork.IsConnected) // need to check that join lobby worked
            {
                Debug.LogError("Not connected to Photon");
                return;
            }

            //connectingLabel.SetActive(true);
            // this is only good if we're going to move somwehere else after - presently only working in menu

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;

            PhotonNetwork.JoinOrCreateRoom(_createRoomName.text, roomOptions, TypedLobby.Default);
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("Created Room", this);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogError("Failed To Create Room", this);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Joined Room", this);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.LogError("Failed To Join Room", this);
        }
    }
}

