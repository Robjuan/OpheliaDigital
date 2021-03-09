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
        public GameObject playerDisplayPrefab;

        public RectTransform playerDisplayPlace;
        public RectTransform opponentDisplayPlace;

        private PlayerDisplay localPlayerDisplay;
        private PlayerDisplay oppPlayerDisplay;

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
                SetupPlayerDisplay(player);
            }

        }


        private void SetupPlayerDisplay(Player player)
        {
            
            if (player == PhotonNetwork.LocalPlayer)
            {
                GameObject _pm = Instantiate(playerDisplayPrefab, playerDisplayPlace);
                localPlayerDisplay = _pm.GetComponent<PlayerDisplay>();
                localPlayerDisplay.displayParent = playerDisplayPlace;
                localPlayerDisplay.punPlayer = player;
            } else
            {
                GameObject _pm = Instantiate(playerDisplayPrefab, opponentDisplayPlace);
                oppPlayerDisplay = _pm.GetComponent<PlayerDisplay>();
                oppPlayerDisplay.displayParent = opponentDisplayPlace;
                oppPlayerDisplay.punPlayer = player;
            }
        }

    }
}

