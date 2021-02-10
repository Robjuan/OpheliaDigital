using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

using Hashtable = ExitGames.Client.Photon.Hashtable;


namespace Com.WhiteSwan.OpheliaDigital
{
    public class PlayerManager : MonoBehaviourPunCallbacks
    {

        [HideInInspector]
        // both set by MultiPlayerManager on instantiation
        public RectTransform displayParent;
        public Player punPlayer;

        [SerializeField]
        private GameObject playerDisplayPrefab;
        private PlayerDisplay playerDisplay;

        void Awake()
        {
            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (playerDisplayPrefab == null)
            {
                Debug.LogError("player display prefab not set");
                return;
            }


            GameObject pd = Instantiate(playerDisplayPrefab, displayParent);
            playerDisplay = pd.GetComponent<PlayerDisplay>();
            playerDisplay.playerNameDisplay.text = punPlayer.NickName;
            playerDisplay.currentPointsDisplay.text = "0";
            ExpandToFillParent(pd.GetComponent<RectTransform>());

            if (!punPlayer.CustomProperties.ContainsKey(KeyStrings.Ready))
            {
                punPlayer.CustomProperties.Add(KeyStrings.Ready, 0);
            }

        }
        public void SetReady(bool unready = false)
        {
            Hashtable ht = new Hashtable();
            ht.Add(KeyStrings.Ready, unready ? 0 : 1);
            punPlayer.SetCustomProperties(ht);
        }

        private void ExpandToFillParent(RectTransform childRect)
        {
            childRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, childRect.parent.GetComponent<RectTransform>().sizeDelta.x);
            childRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, childRect.parent.GetComponent<RectTransform>().sizeDelta.y);
        }

        [PunRPC]
        public void StartTurn()
        {
            Debug.Log("starting turn");
        }


    }
}

