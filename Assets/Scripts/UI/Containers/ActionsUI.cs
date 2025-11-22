using System;
using System.Collections.Generic;
using System.Linq;
using GameDevTV.Commands;
using GameDevTV.EventBus;
using GameDevTV.Events;
using GameDevTV.UI.Components;
using GameDevTV.Units;
using UnityEngine;
using UnityEngine.Events;

namespace GameDevTV.UI.Containers
{
    public class ActionsUI : MonoBehaviour, IUIElement<HashSet<AbstractCommandable>>
    {
        [SerializeField]
        private UIActionButton[] actionButtons;

        private void RefreshButtons(HashSet<AbstractCommandable> selectedUnits)
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

                    actionButtons[i].EnableFor(actionForSlot, HandleClick(actionForSlot));
                }
                else
                {
                    // Debug.Log("actionForSlot == null");
                    actionButtons[i].Disable();
                }
            }
        }

        private UnityAction HandleClick(ActionBase action)
        {
            return () => Bus<ActionSelectedEvent>.Raise(new ActionSelectedEvent(action));
        }

        public void EnableFor(HashSet<AbstractCommandable> selectedUnits)
        {
            RefreshButtons(selectedUnits);
        }

        public void Disable()
        {
            foreach (UIActionButton button in actionButtons)
            {
                button.Disable();
            }
        }
    }
}
