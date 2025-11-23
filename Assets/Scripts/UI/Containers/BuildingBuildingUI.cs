using System;
using System.Collections;
using GameDevTV.UI.Components;
using GameDevTV.Units;
using Unity.VisualScripting;
using UnityEngine;

namespace GameDevTV.UI.Containers
{
    public class BuildingBuildingUI : MonoBehaviour, IUIElement<BaseBuilding>
    {
        [SerializeField]
        private UIBuildQueueButton[] unitButtons;

        [SerializeField]
        private ProgressBar progressBar;

        private BaseBuilding building;
        private Coroutine buildCoroutine;

        public void Disable()
        {
            gameObject.SetActive(false);
            if (building != null)
            {
                building.OnQueueUpdated -= HandleQueueUpdated;
            }
            buildCoroutine = null;
            building = null;
        }

        private void HandleQueueUpdated(UnitSO[] unitsInQueue)
        {
            if (unitsInQueue.Length == 1 && buildCoroutine == null)
            {
                buildCoroutine = StartCoroutine(UpdateUnitProgress());
            }
            SetupUnitButtons();
        }

        public void EnableFor(BaseBuilding item)
        {
            progressBar.SetProgress(0);
            gameObject.SetActive(true);
            building = item;
            building.OnQueueUpdated += HandleQueueUpdated;
            SetupUnitButtons();
            buildCoroutine = StartCoroutine(UpdateUnitProgress());
        }

        private void SetupUnitButtons()
        {
            int i = 0;
            for (; i < building.QueueSize; i++)
            {
                int index = i;
                unitButtons[i]
                    .EnableFor(building.Queue[i], () => building.CancelBuildingUnit(index));
            }
            for (; i < unitButtons.Length; i++)
            {
                unitButtons[i].Disable();
            }
        }

        private IEnumerator UpdateUnitProgress()
        {
            while (building != null && building.QueueSize > 0)
            {
                float startTime = building.CurrentQueueStartTime;
                float endTime = startTime + building.BuildingUnit.BuildTime;
                float progress = Mathf.Clamp01((Time.time - startTime) / (endTime - startTime));
                progressBar.SetProgress(progress);
                yield return null;
            }
            progressBar.SetProgress(0);
            buildCoroutine = null;
        }
    }
}
