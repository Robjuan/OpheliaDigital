using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

namespace Com.WhiteSwan.OpheliaDigital
{
    public class MultiPlayerManager : MonoBehaviourPunCallbacks
    {

        public static MultiPlayerManager Instance;
        public GameObject playerPrefab;


        private void Start()
        {
            Instance = this;

            if (playerPrefab == null)
            {
                Debug.LogError("no player prefab set");
            }
            else
            {
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    Debug.LogFormat("instantiating LocalPlayer {0}", Application.loadedLevelName);
                    PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0,0,0), Quaternion.identity);
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
                
            }
        }


        #region photon callbacks
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0); // send us back to the lobby - using scenemanager here because we are not syncing with other players
        }

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName);
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom() IsMasterClient {0}", PhotonNetwork.IsMasterClient);

                LoadArena();
            }
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects


            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


                LoadArena();
            }
        }
        #endregion

        #region public methods

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        #endregion

        #region private methods

        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("Must be master client to load a level"); // caller's responsibility to check
            }
            Debug.LogFormat("PUN: Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
        }

        #endregion

    }

}
