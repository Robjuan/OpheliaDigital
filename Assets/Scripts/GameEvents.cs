﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.WhiteSwan.OpheliaDigital
{
    public class GameEvents : MonoBehaviour
    {
        public static GameEvents current
        {
            get
            {
                if (_current == null)
                    _current = FindObjectOfType(typeof(GameEvents)) as GameEvents;

                return _current;
            }
            set
            {
                _current = value;
            }
        }
        private static GameEvents _current;
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

        public event Action<BoardController.Phase> onPhaseChange;
        public void PhaseChange(BoardController.Phase newPhase)
        {
            if (onPhaseChange != null)
            {
                onPhaseChange(newPhase);
            }
        }

        public event Action<int> onPriorityChange;
        public void PriorityChange(int newPrioActorNumber)
        {
            if (onPriorityChange != null)
            {
                onPriorityChange(newPrioActorNumber);
            }
        }

        public event Action<CardController, int, int> onCardAdded;
        public void CardAdded(CardController card, int newZone, int previousZone)
        {
            if (onCardAdded != null)
            {
                onCardAdded(card, newZone, previousZone);
            }
        }
        public event Action<CardController, int> onCardRemoved;
        public void CardRemoved(CardController card, int previousZone)
        {
            if (onCardRemoved != null)
            {
                onCardRemoved(card, previousZone);
            }
        }



    }
}