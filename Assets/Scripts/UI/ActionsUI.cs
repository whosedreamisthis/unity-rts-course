using System;
using System.Collections.Generic;
using System.Linq;
using GameDevTV.Commands;
using GameDevTV.EventBus;
using GameDevTV.Events;
using GameDevTV.Units;
using Unity.AppUI.UI;
using UnityEngine;

namespace GameDevTV.UI
{
    public class ActionsUI : MonoBehaviour
    {
        [SerializeField]
        private UIActionButton[] actionButtons;
        private HashSet<AbstractCommandable> selectedUnits = new(12);

        private void Awake()
        {
            Bus<UnitSelectedEvent>.OnEvent += HandleUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent += HandleUnitDeselected;
            Debug.Log("selected");
            foreach (UIActionButton button in actionButtons)
            {
                button.SetIcon(null);
            }
        }

        private void OnDestroy()
        {
            // Debug.Log("destroy selected");
            Bus<UnitSelectedEvent>.OnEvent -= HandleUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent -= HandleUnitDeselected;
        }

        private void HandleUnitDeselected(UnitDeselectedEvent args)
        {
            if (args.Unit is AbstractCommandable commandable)
            {
                selectedUnits.Remove(commandable);
                RefreshButtons();
            }
        }

        private void HandleUnitSelected(UnitSelectedEvent args)
        {
            if (args.Unit is AbstractCommandable commandable)
            {
                selectedUnits.Add(commandable);

                RefreshButtons();
            }
        }

        private void RefreshButtons()
        {
            // Debug.Log("regresh");
            HashSet<ActionBase> availableCommands = new(9);

            foreach (AbstractCommandable commandable in selectedUnits)
            {
                availableCommands.UnionWith(commandable.AvailableCommands);
                // Debug.Log($"availableCommands.Length {availableCommands.Count}");
            }

            for (int i = 0; i < actionButtons.Length; i++)
            {
                // Debug.Log("action for slot wherew");
                ActionBase actionForSlot = availableCommands
                    .Where(action => action.Slot == i)
                    .FirstOrDefault();

                if (actionForSlot != null)
                {
                    // Debug.Log("actionForSlot != null");

                    actionButtons[i].SetIcon(actionForSlot.Icon);
                }
                else
                {
                    // Debug.Log("actionForSlot == null");
                    actionButtons[i].SetIcon(null);
                }
            }
        }
    }
}
