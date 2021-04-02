using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.WhiteSwan.OpheliaDigital
{
    public class BoardController : MonoBehaviour
    {
        public int currentRound;

        public string currentPhase;

        // public List<Effect> activeEffects;

        public void UpdatePhase(string newPhase)
        {
            this.GetComponent<PhotonView>().RPC(UpdatePhase_RPC_string, RpcTarget.AllViaServer, newPhase);
        }

        public const string UpdatePhase_RPC_string = "UpdatePhase_RPC";
        [PunRPC]
        public void UpdatePhase_RPC(string newPhase)
        {
            GameEvents.current.PhaseChange(newPhase);
        }

    }
}