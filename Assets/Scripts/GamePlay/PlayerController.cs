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
        public Player punPlayer;

        // filled at instantiation
        public List<GameObject> ownedCards = new List<GameObject>();

        private PlayerDisplay selfDisplay;

        public int turnOrder;
        public int points;

        private bool hasPriority;


        public string GetName()
        {
            if(punPlayer != null)
            {
                return punPlayer.NickName;
            }
            else
            {
                Debug.LogWarningFormat("no punplayer found on {0}", this);
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
            return "PC name: " + GetName() + ", AN: " + punPlayer.ActorNumber;
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            GameStateManager.current.playerControllers.Add(this);
        }
    }
}
