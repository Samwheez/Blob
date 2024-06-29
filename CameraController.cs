using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlobBuddies.Runtime
{
    public class CameraController : MonoBehaviour
    {
        // Change edgePanThreshold to a percentage of the screen.  Set the values in SetEdgePanDelta in awake to make it cleaner;
        public static CameraController Instance;

        [SerializeField] private InputDirector inputDirector;
        [SerializeField] private Camera cam;

        [Header("Zoom")]
        [SerializeField] private float zoomSpeed = 6.0f;
        [SerializeField] private float maxZoom = 14f;
        [SerializeField] private float minZoom = 2f;
        [Header("Edge Panning")]
        [SerializeField] private float edgePanSpeed = 20f;
        [SerializeField] private float edgePanThreshold = 100f;

        private Transform target;
        private Vector3 targetZoomOffsets;
        private Vector3 edgepanDelta;
        private bool isFollowing = false;


        private void Awake()
        {
            Instance = this;

            targetZoomOffsets = cam.transform.localPosition;

            inputDirector.MouseMoved += SetEdgePanDelta;
            inputDirector.Zoomed += SetTargetZoomOffsets;

            inputDirector.ToggleCameraFollow += ToggleCameraFollow;

        }

        private void LateUpdate()
        {
            EdgePanCamera();
            ZoomCamera();
            FollowTarget();
        }

        private void EdgePanCamera()
        {
            if (isFollowing) return;

            transform.position += edgepanDelta * edgePanSpeed * Time.deltaTime;
        }

        private void ZoomCamera()
        {
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, targetZoomOffsets, zoomSpeed * Time.deltaTime);
        }


        private void FollowTarget()
        {
            if (!isFollowing || target == null) return;


            Vector3 targetPosition = target.transform.position;
            targetPosition.y = transform.position.y;

            transform.position = targetPosition;
        }


        private void SetEdgePanDelta(Vector2 mousePosition)
        {
            edgepanDelta = Vector3.zero;

            if (mousePosition.x > Screen.width - edgePanThreshold && mousePosition.x < Screen.width + edgePanThreshold / 2)
                edgepanDelta.x += 1;

            if (mousePosition.x < edgePanThreshold && mousePosition.x > -edgePanThreshold / 2)
                edgepanDelta.x -= 1;

            if (mousePosition.y > Screen.height - edgePanThreshold && mousePosition.y < Screen.height + edgePanThreshold / 2)
                edgepanDelta.z += 1;

            if (mousePosition.y < edgePanThreshold && mousePosition.y > -edgePanThreshold / 2)
                edgepanDelta.z -= 1;

            edgepanDelta = edgepanDelta.normalized;
        }

        private void SetTargetZoomOffsets(float zoomInput)
        {
            Vector3 zoomDir = targetZoomOffsets.normalized;

            if (zoomInput > 0)
                targetZoomOffsets -= zoomDir;
            else if (zoomInput < 0)
                targetZoomOffsets += zoomDir;

            if (targetZoomOffsets.magnitude > maxZoom)
                targetZoomOffsets = zoomDir * maxZoom;
            else if (targetZoomOffsets.magnitude < minZoom)
                targetZoomOffsets = zoomDir * minZoom;
        }

        private void ToggleCameraFollow()
        {
            isFollowing = !isFollowing;
        }

        public Camera GetCamera()
        {
            return cam;
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }
    }

}
