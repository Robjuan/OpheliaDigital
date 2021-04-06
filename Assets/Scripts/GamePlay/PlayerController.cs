using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Realtime;
using Photon.Pun;

namespace Com.WhiteSwan.OpheliaDigital
{
    // instantiated by LocalGameManager at runtime
    public class PlayerController : MonoBehaviour, IPunInstantiateMagicCallback
    {

        public GameObject playerDisplayPrefab;
        public int punActorNumber;

        // filled at instantiation
        public List<GameObject> ownedCards = new List<GameObject>();

        private PlayerDisplay selfDisplay;

        public int turnOrder;
        public int points;

        private bool hasPriority;


        public string GetName()
        {
            if(PhotonNetwork.CurrentRoom.Players.ContainsKey(punActorNumber))
            {
                return PhotonNetwork.CurrentRoom.Players[punActorNumber].NickName;
            }
            else
            {
                Debug.LogWarningFormat("no pun ActorNumber found on playerController");
                return "null PunPlayer name";
            }
            
        }

        public void SetupDisplay(RectTransform targetParent)
        {
            GameObject _pm = Instantiate(playerDisplayPrefab, targetParent);
            selfDisplay = _pm.GetComponent<PlayerDisplay>();
            selfDisplay.displayParent = targetParent;
            selfDisplay.playerController = this;
        }

        public override string ToString()
        {
            return "PC name: " + GetName() + ", AN: " + punActorNumber;
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            object[] initData = info.photonView.InstantiationData;
            punActorNumber = (int)initData[0];
            points = 0;

            GameStateManager.current.playerControllers.Add(this);
        }
    }
}
