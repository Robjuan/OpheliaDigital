using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.WhiteSwan.OpheliaDigital
{
    public class GameEvents : MonoBehaviour
    {
        public static GameEvents current;
        void Awake()
        {
            current = this;
        }

        /*
        public event Action<WeaponController> onWeaponChange;
        public void WeaponChange(WeaponController newActiveWeapon)
        {
            if (onWeaponChange != null)
            {
                onWeaponChange(newActiveWeapon);
            }
        }
        */

        public event Action onDeckSelected;
        public void DeckSelected()
        {
            if (onDeckSelected != null)
            {
                onDeckSelected();
            }
        }


    }
}