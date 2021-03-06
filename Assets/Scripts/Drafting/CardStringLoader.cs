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
            /*
            // save data as per CardDataHolder
            CardDataCollection cardDataCollection = JsonUtility.FromJson<CardDataCollection>(jsonString);

            //create dictionary
            Dictionary<string, int> cardDict_FullPath_Quant = new Dictionary<string, int>();
            foreach (CardDataHolder card in cardDataCollection.cards)
            {
                cardDict_FullPath_Quant[card.Name] = 2; // TODO: set quantities somehow
                //Debug.Log(card.Name);
            }


            // set the playerproperty
            Hashtable ht = new Hashtable();
            ht.Add(KeyStrings.CardList, cardDict_FullPath_Quant);
            player.SetCustomProperties(ht);
            */
            

        }

    }

    /*

    // little classes to help unity's terrible json loader
    [System.Serializable]

    public class CardDataHolder
    {
        public string Name;
        public int Armour;
        public string Claim;
        public int Cost;
        public string Effect;
        public int Initiative;
        public int Life;
        public string Passive;
        public string SlotType;
        public string Special;
        public string devName; // in form "xxx_name" where xxx is unique number
    }

    [System.Serializable]
    public class CardDataCollection
    {
        public CardDataHolder[] cards;
    }
    */
}
