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
        // both set by MultiPlayerManager on instantiation
        public RectTransform displayParent;
        public Player punPlayer;

        private void Start()
        {
            playerNameDisplay.text = punPlayer.NickName;
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



