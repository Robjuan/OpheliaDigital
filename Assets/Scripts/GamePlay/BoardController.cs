using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.WhiteSwan.OpheliaDigital
{
    public class BoardController : MonoBehaviour, IPunInstantiateMagicCallback
    {
        public enum Phase : int
        {
            LoadPhase = -1
            , PreGameSetupPhase = 0
            , RoundStart = 1
            , TurnStart = 2
            , ActionOne = 3
            , Contest = 4
            , ActionTwo = 5
            , End = 6
            , RoundEnd = 7
        }

        public int currentRound;

        public Phase currentPhase;

        // public List<Effect> activeEffects;

        public void GoToNextPhase()
        {
            UpdatePhase(GetNextValidPhase());
        }

        public Phase GetNextValidPhase()
        {
            switch((int)currentPhase)
            {
                case int p when p < 6: // if we're in the main part of the turn
                    return currentPhase++;
                case int p when p == 6:
                    currentPhase = Phase.TurnStart; // start a new turn    
                    return currentPhase; 
                default:
                    Debug.LogWarning("end round not yet implemented");
                    return currentPhase;
            }

        }

        public void UpdatePhase(Phase newPhase)
        {
            if(newPhase != currentPhase)
            {
                this.GetComponent<PhotonView>().RPC(UpdatePhase_RPC_string, RpcTarget.AllViaServer, (int)newPhase);
            }
            else
            {
                Debug.LogWarning("attempint got change to current phase: " + currentPhase);
            }
            
        }

        public const string UpdatePhase_RPC_string = "UpdatePhase_RPC";
        [PunRPC]
        public void UpdatePhase_RPC(int newPhase)
        {
            Debug.Log("Progressing to Phase: <color=teal>" + (Phase)newPhase + "</color>");
            GameEvents.current.PhaseChange((Phase)newPhase);
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            GameStateManager.current.boardController = this;
            currentPhase = Phase.LoadPhase;
            currentRound = 0;
        }

        


    }
}