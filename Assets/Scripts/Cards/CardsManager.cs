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
    [RequireComponent(typeof(CardsPrefabLibrary))]
    public class CardsManager : MonoBehaviour
    {

        private CardsPrefabLibrary prefabLibrary;

        [SerializeField]
        private GameObject selfDeckManager;
        [SerializeField]
        private GameObject oppDeckManager;

        private CardContainer selfDeckCardContainer;

        private void Awake()
        {
            selfDeckCardContainer = selfDeckManager.GetComponent<CardContainer>();
            prefabLibrary = GetComponent<CardsPrefabLibrary>();
        }

        private void Start()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                LoadDeck();
            }

        }

        private void LoadDeck()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("MasterClient only function called on non-MC player");
                return;
            }

            Dictionary<string, int> cardList = (Dictionary<string, int>)(PhotonNetwork.LocalPlayer.CustomProperties[KeyStrings.CardList]);
            if (cardList != null)
            {
                foreach (string cardName in cardList.Keys)
                {
                    selfDeckCardContainer.cards.Add(CreateFullCardFromName(cardName, Vector3.zero, Quaternion.identity));
                }
            }
            else
            {
                Debug.LogErrorFormat("cardlist was null on {0}", PhotonNetwork.LocalPlayer);
            }

        }

        private GameObject CreateFullCardFromName(string cardName, Vector3 position, Quaternion rotation)
        {
            // todo: decide if we should load all values from the json here
            // or if we should have them set in the prefabs

            // abilities will have to be set on the prefab because their functions will need to be coded in

            var prefab = prefabLibrary.prefabMapDict[cardName];
            GameObject newCard = PhotonNetwork.Instantiate(prefab.name, position, rotation);
            newCard.GetComponent<CardController>().SetCardText(cardName);


            return newCard;
        }

    }
}