using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

// this class will take in a list of strings (cardnames), go and get the data about those cards 
// this means it will instantiate the required prefabs and attach the appropriate attributes and abilities

// used in the GameScene on entry


namespace Com.WhiteSwan.OpheliaDigital
{
    public class CardLoader : MonoBehaviour
    {

        public GameObject cardPrefab;

        [SerializeField]
        private GameObject selfDeckPlace;
        [SerializeField]
        private GameObject oppDeckPlace;


        private void Start()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                float yOffset = 0f;
                float xOffset = 0f;
                float zOffset = 0f;
                Dictionary<string, int> cardList = (Dictionary<string, int>)(PhotonNetwork.LocalPlayer.CustomProperties[KeyStrings.CardList]);
                foreach (string cardName in cardList.Keys)
                {
                    Vector3 spawnLoc = new Vector3(xOffset + selfDeckPlace.transform.position.x, yOffset + selfDeckPlace.transform.position.y, zOffset + selfDeckPlace.transform.position.z);
                    yOffset += 1.1f;
                    xOffset += 0f;
                    zOffset -= 5f;

                    CreateFullCardFromName(cardName, spawnLoc, Quaternion.identity);
                }
            }
            
        }

        private GameObject CreateFullCardFromName(string cardName, Vector3 position, Quaternion rotation)
        {
            // todo: go and get all the other values of the card from the json and load them into the CardController
            // 

            GameObject newCard = PhotonNetwork.Instantiate(cardPrefab.name, position, rotation);
            newCard.GetComponent<CardController>().SetCardText(cardName);


            return newCard;
        }

    }
}