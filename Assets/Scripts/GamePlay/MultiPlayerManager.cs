using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

using TMPro;

namespace Com.WhiteSwan.OpheliaDigital
{
    public class MultiPlayerManager : MonoBehaviourPunCallbacks
    {

        public static MultiPlayerManager Instance;
        public GameObject playerPrefab;

        public RectTransform playerDisplayPlace;
        public RectTransform opponentDisplayPlace;

        private PlayerManager localPlayer;
        private PlayerManager oppPlayer;


        [SerializeField]
        private TMP_Text currentRoomDisplay;

        [SerializeField]
        private GameObject readyButton;

        private void Start()
        {
            Instance = this;
            if (currentRoomDisplay != null)
            {
                currentRoomDisplay.text = PhotonNetwork.CurrentRoom.Name;
            }

            // if the only one here, it'll do me, otherwise will do everyone
            foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                SetupPlayerManager(player);
            }

        }


        private void SetupPlayerManager(Player player)
        {

            Debug.Log(player.CustomProperties[KeyStrings.CardList]);

            GameObject _pm = Instantiate(playerPrefab);
            if (player == PhotonNetwork.LocalPlayer)
            {
                localPlayer = _pm.GetComponent<PlayerManager>();
                localPlayer.displayParent = playerDisplayPlace;
                localPlayer.punPlayer = player;
            } else
            {
                oppPlayer = _pm.GetComponent<PlayerManager>();
                oppPlayer.displayParent = opponentDisplayPlace;
                oppPlayer.punPlayer = player;
            }
        }

        public void SetLocalPlayerReady()
        {
            localPlayer.SetReady();
            readyButton.SetActive(false);
        }


        #region photon callbacks

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0); // send us back to the lobby - using scenemanager here because we are not syncing with other players
        }

        /* this won't fire - players won't enter the room straight into MPM area, they'll go to draft area
        public override void OnPlayerEnteredRoom(Player other)
        {
            SetupPlayerManager(other);
        }
        */
        #endregion

        #region public methods

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        #endregion

    }
}

