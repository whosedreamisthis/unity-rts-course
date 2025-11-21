// using GameDevTV.Player;
using System;
using System.Collections.Generic;
using GameDevTV.EventBus;
using GameDevTV.Events;
using GameDevTV.Units;
using Unity.Android.Gradle.Manifest;
using Unity.Cinemachine;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameDevTV.Player
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody cameraTarget;

        [SerializeField]
        private CinemachineCamera cinemachineCamera;

        [SerializeField]
        private new Camera camera;
        private CinemachineFollow cinemachineFollow;

        [SerializeField]
        private CameraConfig cameraConfig;

        [SerializeField]
        private LayerMask selectableUnitsLayers;

        [SerializeField]
        private RectTransform selectionBox;

        private Vector2 startingMousePosition;

        [SerializeField]
        private LayerMask floorLayers;
        private float zoomStartTime;
        private float rotationStartTime;
        private float maxRotationAmount;
        private HashSet<AbstractUnit> aliveUnits = new(100);
        private List<ISelectable> selectedUnits = new(12);
        private HashSet<AbstractUnit> addedUnits = new(24);

        private Vector3 startingFollowOffset;

        private void Awake()
        {
            if (!cinemachineCamera.TryGetComponent(out cinemachineFollow))
            {
                Debug.LogError(
                    "Cinemachine camera did not have cinemachineFollow. zoom functionality will not work."
                );
            }
            startingFollowOffset = cinemachineFollow.FollowOffset;
            maxRotationAmount = Mathf.Abs(cinemachineFollow.FollowOffset.z);

            Bus<UnitSelectedEvent>.OnEvent += HandleUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent += HandleUnitDeselected;
            Bus<UnitSpawnEvent>.OnEvent += HandleUnitSpawned;
        }

        private void OnDestroy()
        {
            Bus<UnitSelectedEvent>.OnEvent -= HandleUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent -= HandleUnitDeselected;
            Bus<UnitSpawnEvent>.OnEvent -= HandleUnitSpawned;
        }

        private void HandleUnitSpawned(UnitSpawnEvent evt) => aliveUnits.Add(evt.Unit);

        private void HandleUnitSelected(UnitSelectedEvent evt) => selectedUnits.Add(evt.Unit);

        private void HandleUnitDeselected(UnitDeselectedEvent evt) =>
            selectedUnits.Remove(evt.Unit);

        // Update is called once per frame
        private void Update()
        {
            HandlePanning();
            HandleZooming();
            HandleRotation();
            HandleRightClick();
            HandleDragSelect();
        }

        private Bounds ResizeSelectionBox()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            float width = mousePosition.x - startingMousePosition.x;
            float height = mousePosition.y - startingMousePosition.y;
            selectionBox.anchoredPosition =
                startingMousePosition + new Vector2(width / 2, height / 2);
            selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
            return new Bounds(selectionBox.anchoredPosition, selectionBox.sizeDelta);
        }

        private void HandleDragSelect()
        {
            if (selectionBox == null)
                return;
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                HandleMouseDown();
            }
            else if (
                Mouse.current.leftButton.isPressed && !Mouse.current.leftButton.wasPressedThisFrame
            )
            {
                HandleMouseDrag();
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                HandleMouseUp();
            }
        }

        private void HandleMouseUp()
        {
            if (!Keyboard.current.shiftKey.isPressed)
            {
                DeselectAllUnits();
            }

            HandleLeftClick();
            foreach (AbstractUnit unit in addedUnits)
            {
                unit.Select();
            }
            selectionBox.gameObject.SetActive(false);
            selectionBox.sizeDelta = Vector2.zero;
        }

        private void HandleMouseDrag()
        {
            Bounds selectionBoxBounds = ResizeSelectionBox();
            foreach (AbstractUnit unit in aliveUnits)
            {
                Vector2 unitPosition = camera.WorldToScreenPoint(unit.transform.position);
                if (selectionBoxBounds.Contains(unitPosition))
                {
                    addedUnits.Add(unit);
                }
            }
        }

        private void HandleMouseDown()
        {
            selectionBox.gameObject.SetActive(true);
            startingMousePosition = Mouse.current.position.ReadValue();
            selectionBox.anchoredPosition = startingMousePosition;
            addedUnits.Clear();
        }

        private void DeselectAllUnits()
        {
            ISelectable[] currentlySelectedUnits = selectedUnits.ToArray();
            foreach (ISelectable selectable in currentlySelectedUnits)
            {
                selectable.Deselect();
            }
        }

        private void HandleRightClick()
        {
            if (selectedUnits.Count == 0)
                return;
            Ray cameraRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Mouse.current.rightButton.wasReleasedThisFrame)
            {
                if (Physics.Raycast(cameraRay, out RaycastHit hit, float.MaxValue, floorLayers))
                {
                    List<AbstractUnit> abstractUnits = new(selectedUnits.Count);

                    foreach (ISelectable selectable in selectedUnits)
                    {
                        if (selectable is AbstractUnit unit)
                        {
                            abstractUnits.Add(unit);
                        }
                    }

                    int unitsOnLayer = 0;
                    int maxUnitsOnLayer = 1;
                    float circleRadius = 0;
                    float radialOffset = 0;

                    foreach (AbstractUnit unit in abstractUnits)
                    {
                        Vector3 targetPosition = new(
                            hit.point.x + circleRadius * Mathf.Cos(radialOffset * unitsOnLayer),
                            hit.point.y,
                            hit.point.z + circleRadius * Mathf.Sin(radialOffset * unitsOnLayer)
                        );

                        unit.MoveTo(targetPosition);
                        unitsOnLayer++;

                        if (unitsOnLayer >= maxUnitsOnLayer)
                        {
                            unitsOnLayer = 0;
                            circleRadius += unit.AgentRadius * 3.5f;
                            maxUnitsOnLayer = Mathf.FloorToInt(
                                2 * Mathf.PI * circleRadius / (unit.AgentRadius * 2)
                            );

                            radialOffset = 2 * Mathf.PI / maxUnitsOnLayer;
                        }
                    }
                }
                // foreach (ISelectable selectable in selectedUnits)
                // {
                //     if (selectable is IMovable movable)
                //     {
                //         movable.MoveTo(hit.point);
                //     }
                // }
            }
        }

        private void HandleLeftClick()
        {
            if (camera == null)
                return;

            Ray cameraRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (
                Physics.Raycast(
                    cameraRay,
                    out RaycastHit hit,
                    float.MaxValue,
                    selectableUnitsLayers
                ) && hit.collider.TryGetComponent(out ISelectable selectable)
            )
            {
                // selectedUnit = selectable;
                selectable.Select();
            }
        }

        private void HandleRotation()
        {
            if (ShouldSetRotationStartTime())
            {
                rotationStartTime = Time.time;
            }
            float rotationTime = Mathf.Clamp01(
                (Time.time - rotationStartTime) * cameraConfig.RotationSpeed
            );
            Vector3 targetFollowOffset;

            if (Keyboard.current.pageDownKey.isPressed)
            {
                targetFollowOffset = new Vector3(
                    maxRotationAmount,
                    cinemachineFollow.FollowOffset.y,
                    0
                );
            }
            else if (Keyboard.current.pageUpKey.isPressed)
            {
                targetFollowOffset = new Vector3(
                    -maxRotationAmount,
                    cinemachineFollow.FollowOffset.y,
                    0
                );
            }
            else
            {
                targetFollowOffset = new Vector3(
                    startingFollowOffset.x,
                    cinemachineFollow.FollowOffset.y,
                    startingFollowOffset.z
                );
            }
            cinemachineFollow.FollowOffset = Vector3.Slerp(
                cinemachineFollow.FollowOffset,
                targetFollowOffset,
                rotationTime
            );
        }

        private void HandleZooming()
        {
            if (ShouldSetZoomStartTime())
            {
                zoomStartTime = Time.time;
            }
            Vector3 targetFollowOffset;
            float zoomTime = Mathf.Clamp01((Time.time - zoomStartTime) * cameraConfig.ZoomSpeed);
            // Debug.Log($"zoomTime {zoomTime} (Time.time - zoomStartTime) {(Time.time - zoomStartTime)} (Time.time - zoomStartTime) * zoomSpeed {(Time.time - zoomStartTime) * zoomSpeed}")
            if (Keyboard.current.endKey.isPressed)
            {
                targetFollowOffset = new Vector3(
                    cinemachineFollow.FollowOffset.x,
                    cameraConfig.MinZoomDistance,
                    cinemachineFollow.FollowOffset.z
                );
            }
            else
            {
                targetFollowOffset = new Vector3(
                    cinemachineFollow.FollowOffset.x,
                    startingFollowOffset.y,
                    cinemachineFollow.FollowOffset.z
                );
            }
            cinemachineFollow.FollowOffset = Vector3.Slerp(
                cinemachineFollow.FollowOffset,
                targetFollowOffset,
                zoomTime
            );
        }

        private void HandlePanning()
        {
            Vector2 moveAmount = GetKeyboardMoveAmount();
            moveAmount += GetMouseMoveAmount();

            cameraTarget.linearVelocity = new Vector3(moveAmount.x, 0, moveAmount.y);
        }

        private Vector2 GetMouseMoveAmount()
        {
            Vector2 moveAmount = Vector2.zero;

            if (!cameraConfig.EnableEdgePan)
                return moveAmount;

            Vector2 mousePosition = Mouse.current.position.ReadValue();
            int screenWidth = UnityEngine.Screen.width;
            int screenHeight = UnityEngine.Screen.height;

            if (mousePosition.x <= cameraConfig.EdgePanSize)
            {
                moveAmount.x -= cameraConfig.MousePanSpeed;
            }
            else if (mousePosition.x >= screenWidth - cameraConfig.EdgePanSize)
            {
                moveAmount.x += cameraConfig.MousePanSpeed;
            }

            if (mousePosition.y >= screenHeight - cameraConfig.EdgePanSize)
            {
                moveAmount.y += cameraConfig.MousePanSpeed;
            }
            else if (mousePosition.y <= cameraConfig.EdgePanSize)
            {
                moveAmount.y -= cameraConfig.MousePanSpeed;
            }

            return moveAmount;
        }

        private Vector2 GetKeyboardMoveAmount()
        {
            Vector2 moveAmount = Vector2.zero;

            if (Keyboard.current.upArrowKey.isPressed)
            {
                moveAmount.y += cameraConfig.KeyboardPanSpeed;
            }
            if (Keyboard.current.leftArrowKey.isPressed)
            {
                moveAmount.x -= cameraConfig.KeyboardPanSpeed;
            }
            if (Keyboard.current.downArrowKey.isPressed)
            {
                moveAmount.y -= cameraConfig.KeyboardPanSpeed;
            }
            if (Keyboard.current.rightArrowKey.isPressed)
            {
                moveAmount.x += cameraConfig.KeyboardPanSpeed;
            }
            return moveAmount;
        }

        private bool ShouldSetRotationStartTime()
        {
            return Keyboard.current.pageUpKey.wasPressedThisFrame
                || Keyboard.current.pageDownKey.wasPressedThisFrame
                || Keyboard.current.pageUpKey.wasReleasedThisFrame
                || Keyboard.current.pageDownKey.wasReleasedThisFrame;
        }

        private bool ShouldSetZoomStartTime()
        {
            return Keyboard.current.endKey.wasPressedThisFrame
                || Keyboard.current.endKey.wasReleasedThisFrame;
        }
    }
}
