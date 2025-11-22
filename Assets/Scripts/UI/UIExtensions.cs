using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public static class UIExtensions
{
    public static bool IsPointerOverUIObject()
    {
        // 1. Get the current position from the active Pointer device (Mouse or Touch)
        Vector2 position = Pointer.current.position.ReadValue();

        // 2. Setup the pointer data
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = position,
        };

        // 3. Perform the Raycast
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        // --- DEBUGGING CODE START ---
        if (results.Count > 0)
        {
            Debug.LogWarning(
                $"UI Raycast detected a hit on a UI object! The click will be blocked."
            );
            foreach (RaycastResult result in results)
            {
                // CORRECTED LINE: Removed invalid 'sortingLayerName' property
                Debug.Log(
                    $"Blocking UI Element Hit: {result.gameObject.name} (Canvas Order: {result.sortingOrder})"
                );
            }
        }
        // --- DEBUGGING CODE END ---

        return results.Count > 0;
    }
}
