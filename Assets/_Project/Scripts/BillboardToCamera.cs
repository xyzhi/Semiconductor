using UnityEngine;

namespace SemiconductorTeaching
{
    public sealed class BillboardToCamera : MonoBehaviour
    {
        public Camera targetCamera;
        public bool keepUpright = true;
        public bool flipForward = true;

        private void LateUpdate()
        {
            Camera cameraToFace = targetCamera != null ? targetCamera : Camera.main;
            if (cameraToFace == null)
            {
                return;
            }

            Vector3 direction = transform.position - cameraToFace.transform.position;
            if (direction.sqrMagnitude < 0.0001f)
            {
                return;
            }

            if (keepUpright)
            {
                direction.y = 0f;
            }

            if (direction.sqrMagnitude < 0.0001f)
            {
                return;
            }

            if (!flipForward)
            {
                direction = -direction;
            }

            transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        }
    }
}
