using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Com.WhiteSwan.OpheliaDigital
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Private Serialisable Fields
        [Tooltip("Above this a new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;

        [Tooltip("UI Panel")]
        [SerializeField]
        private GameObject controlPanel;

        [Tooltip("Connection progress label")]
        [SerializeField]
        private GameObject progressLabel;

        #endregion

        #region Private Fields

        /// <summary>
        /// version of this client
        /// </summary>
        string gameVersion = "1";
        bool isConnecting;

        #endregion

        #region MonoBehaviour Callbacks

        void Awake()
        {
            // #critical
            // ensures PhotonNetwork.LoadLevel() causes all clients to sync when run on master client[
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        void Start()
        {
            // Connect();

            // setup visibility
            //progressLabel.SetActive(false);
            controlPanel.SetActive(true);


            Screen.SetResolution(1024, 768, false);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Start connection
        /// - if connected, attempt to join random room
        /// - if not, connect to photon cloud network
        /// </summary>
        public void Connect()
        {
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);

            if (PhotonNetwork.IsConnected)
            {
                // #critical: if fails, we will create
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }

        #endregion

        #region MonoBehaviourPunCallbacks Callbacks
        
        public override void OnConnectedToMaster()
        {
            Debug.Log("OnConnectedToMaster() was called by PUN");
            if (isConnecting)
            {
                PhotonNetwork.JoinRandomRoom(); // calls onjoinrandomfailed when fails
            }
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("join random failed");
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
            
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("joined room successfully");
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("load room for 1");
                
                // #critical
                PhotonNetwork.LoadLevel("Room for 1");
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            isConnecting = false;
            Debug.LogWarningFormat("OnDisconnected() was called by PUN with reason {0}", cause);
        }

        #endregion
    }

}