// using GameDevTV.Player;
using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameDevTV.Player
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField]
        private Transform cameraTarget;

        [SerializeField]
        private CinemachineCamera cinemachineCamera;
        private CinemachineFollow cinemachineFollow;

        [SerializeField]
        private CameraConfig cameraConfig;
        private float zoomStartTime;
        private float rotationStartTime;
        private float maxRotationAmount;

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
        }

        // Update is called once per frame
        private void Update()
        {
            HandlePanning();
            HandleZooming();
            HandleRotation();
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

            moveAmount *= Time.deltaTime;
            cameraTarget.position += new Vector3(moveAmount.x, 0, moveAmount.y);
        }

        private Vector2 GetMouseMoveAmount()
        {
            Vector2 moveAmount = Vector2.zero;

            if (!cameraConfig.EnableEdgePan)
                return moveAmount;

            Vector2 mousePosition = Mouse.current.position.ReadValue();
            int screenWidth = Screen.width;
            int screenHeight = Screen.height;

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
