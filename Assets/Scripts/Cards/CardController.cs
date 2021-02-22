using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace Com.WhiteSwan.OpheliaDigital
{

    public class CardController : MonoBehaviour
    {

        [SerializeField]
        private TMP_Text cardName;

        // this is a struct defined in cardcontainer
        // this struct is a number of properties that are set based on the card's current container
        // todo: this must support no container
        public CardContainer.ExternallySetCardProperties externallySetProperties;
    
        // will build these cards out by composition
        // each card type will be it's own component (CharacterCardController / TurningPointCardController)
        // each effect will be it's own component 
        
        // each unique card will be represented by a card id
        // each unique card will be a prefab variant of cardbase
            

        public void SetCardText(string text)
        {
            cardName.text = text;
        }


    }



}
