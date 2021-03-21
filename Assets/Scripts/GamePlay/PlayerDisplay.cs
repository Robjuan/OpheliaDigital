using System;
using System.Collections;

using Photon.Realtime;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace Com.WhiteSwan.OpheliaDigital
{
    public class PlayerDisplay : MonoBehaviour
    {
        public TMP_Text playerNameDisplay;
        public TMP_Text currentPointsDisplay;
        public Image playerIcon;

        [HideInInspector]
        // both set by LocalGameManager on instantiation
        public RectTransform displayParent;
        public PlayerController playerController;

        private void Start()
        {
            playerNameDisplay.text = playerController.GetName();
            currentPointsDisplay.text = "0";

            this.GetComponent<RectTransform>().SetParent(displayParent);
            ExpandToFillParent(this.GetComponent<RectTransform>());
        }
        private void ExpandToFillParent(RectTransform childRect)
        {
            childRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, childRect.parent.GetComponent<RectTransform>().sizeDelta.x);
            childRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, childRect.parent.GetComponent<RectTransform>().sizeDelta.y);
        }
    }
}



