using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


using Photon.Pun;
using Photon.Realtime;

using Hashtable = ExitGames.Client.Photon.Hashtable;


namespace Com.WhiteSwan.OpheliaDigital
{
    public class CardStringLoader : MonoBehaviour
    {

        // this class needs to load a specified deck from json into a list of strings into the playerProperties
        // later that list of strings will be used to instantiate actual cards

        // used in the DraftScene

        public void SetCardList(Player player)
        {
            // todo: this will need to get data quite differently when we build the full draft
            // eventually players should be able to save their drafted decks and play again with those

            //List<BaseCard> cardList = new List<BaseCard>();
            // it's not finding BaseCard
            string targetPath = "";
            switch ((string)player.CustomProperties[KeyStrings.ChosenDeck])
            {
                case KeyStrings.Yucatec:
                    targetPath = PathStrings.YucatecCards;
                    break;
                case KeyStrings.Mattervoid:
                    targetPath = PathStrings.MattervoidCards;
                    break;
                case KeyStrings.Mechanicus:
                    targetPath = PathStrings.MechanicusCards;
                    break;
                default:
                    Debug.LogErrorFormat("no valid deck set as chosendeck for player {0}", player);
                    return;
            }

            // get the json of all the cards
            var jsonString = Resources.Load<TextAsset>(targetPath).ToString();

            // save just the name as per CardStringHolder
            CardStringCollection cardStringCollection = JsonUtility.FromJson<CardStringCollection>(jsonString);

            //create dictionary
            Dictionary<string, int> cardListQuantity = new Dictionary<string, int>();
            foreach (CardStringHolder card in cardStringCollection.cards)
            {
                cardListQuantity[card.Name] = 1; // TODO: set quantities somehow
                //Debug.Log(card.Name);
            }


            // set the playerproperty
            Hashtable ht = new Hashtable();
            ht.Add(KeyStrings.CardList, cardListQuantity);
            player.SetCustomProperties(ht);

            

        }

    }

    [System.Serializable]

    public class CardStringHolder
    {
        public string Name;
    }

    [System.Serializable]
    public class CardStringCollection
    {
        public CardStringHolder[] cards;
    }
}
