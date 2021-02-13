using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class CardController : MonoBehaviour
{

    [SerializeField]
    private TMP_Text cardName;

    // will build these cards out by composition
    // each card type will be it's own component (CharacterCardController / TurningPointCardController)
    // each (activated?) effect will be it's own component 

    // each unique card will be represented by a card id
    // each unique card will be a prefab variant of cardbase

    // the card controllers will know what 



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCardText(string text)
    {
        cardName.text = text;
    }

}
