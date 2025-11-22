using System;
using System.Collections.Generic;
using GameDevTV.EventBus;
using GameDevTV.Events;
using GameDevTV.UI.Containers;
using GameDevTV.Units;
using UnityEngine;

namespace GameDevTV.UI
{
    public class RuntimeUI : MonoBehaviour
    {
        [SerializeField]
        private ActionsUI actionsUI;

        private HashSet<AbstractCommandable> selectedUnits = new(12);

        private void Awake()
        {
            Bus<UnitSelectedEvent>.OnEvent += HandleUnitSelectedEvent;
            Bus<UnitDeselectedEvent>.OnEvent += HandleUnitDeselectedEvent;
        }

        private void OnDestroy()
        {
            Bus<UnitSelectedEvent>.OnEvent -= HandleUnitSelectedEvent;
            Bus<UnitDeselectedEvent>.OnEvent -= HandleUnitDeselectedEvent;
        }

        private void HandleUnitDeselectedEvent(UnitDeselectedEvent evt)
        {
            if (evt.Unit is AbstractCommandable commandable)
            {
                selectedUnits.Remove(commandable);
                if (selectedUnits.Count > 0)
                {
                    actionsUI.EnableFor(selectedUnits);
                }
                else
                {
                    actionsUI.Disable();
                }
            }
        }

        private void HandleUnitSelectedEvent(UnitSelectedEvent evt)
        {
            if (evt.Unit is AbstractCommandable commandable)
            {
                selectedUnits.Add(commandable);
                actionsUI.EnableFor(selectedUnits);
            }
        }
    }
}
