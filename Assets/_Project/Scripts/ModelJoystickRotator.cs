using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

namespace SemiconductorTeaching
{
    public sealed class ModelJoystickRotator : MonoBehaviour
    {
        public float horizontalSpeed = 90f;
        public float verticalSpeed = 70f;
        public float maximumTilt = 60f;
        public float zoomSpeed = 1f;
        public float minimumScale = 0.5f;
        public float maximumScale = 2f;
        public float deadZone = 0.15f;

        UnityEngine.XR.InputDevice rightController;
        UnityEngine.XR.InputDevice leftController;
        InputAction rightJoystick;
        InputAction leftJoystick;
        Quaternion initialRotation;
        Vector3 initialScale;
        float yaw;
        float pitch;
        float zoom = 1f;

        void Awake()
        {
            initialRotation = transform.localRotation;
            initialScale = transform.localScale;
            rightJoystick = new InputAction(
                "Right Model Zoom",
                InputActionType.Value,
                "<XRController>{RightHand}/primary2DAxis");
            leftJoystick = new InputAction(
                "Left Model Rotation",
                InputActionType.Value,
                "<XRController>{LeftHand}/primary2DAxis");
            RefreshControllers();
        }

        void OnEnable()
        {
            rightJoystick?.Enable();
            leftJoystick?.Enable();
        }

        void OnDisable()
        {
            rightJoystick?.Disable();
            leftJoystick?.Disable();
        }

        void OnDestroy()
        {
            rightJoystick?.Dispose();
            leftJoystick?.Dispose();
        }

        void Update()
        {
            var rotationAxis = ReadLeftJoystick();
            if (rotationAxis.sqrMagnitude >= deadZone * deadZone)
            {
                yaw += rotationAxis.x * horizontalSpeed * Time.deltaTime;
                pitch = Mathf.Clamp(
                    pitch - rotationAxis.y * verticalSpeed * Time.deltaTime,
                    -maximumTilt,
                    maximumTilt);

                transform.localRotation = initialRotation * Quaternion.Euler(pitch, yaw, 0f);
            }

            var zoomAxis = ReadRightJoystick();
            if (Mathf.Abs(zoomAxis.y) >= deadZone)
            {
                zoom = Mathf.Clamp(
                    zoom + zoomAxis.y * zoomSpeed * Time.deltaTime,
                    minimumScale,
                    maximumScale);
                transform.localScale = initialScale * zoom;
            }
        }

        Vector2 ReadLeftJoystick()
        {
            var leftInputSystemAxis = leftJoystick.ReadValue<Vector2>();
            if (leftInputSystemAxis.sqrMagnitude >= deadZone * deadZone)
                return leftInputSystemAxis;

            if (!leftController.isValid)
                RefreshControllers();

            if (leftController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out var leftAxis))
                return leftAxis;

            return Vector2.zero;
        }

        Vector2 ReadRightJoystick()
        {
            var rightInputSystemAxis = rightJoystick.ReadValue<Vector2>();
            if (rightInputSystemAxis.sqrMagnitude >= deadZone * deadZone)
                return rightInputSystemAxis;

            if (!rightController.isValid)
                RefreshControllers();

            if (rightController.TryGetFeatureValue(
                    UnityEngine.XR.CommonUsages.primary2DAxis,
                    out var rightAxis))
                return rightAxis;

            return Vector2.zero;
        }

        void RefreshControllers()
        {
            rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        }
    }
}
