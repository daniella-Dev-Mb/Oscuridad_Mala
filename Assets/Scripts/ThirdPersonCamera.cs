using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 6f;
    public float targetHeight = 0.6f;
    public float pitch = 40f;
    public float mouseSensitivity = 0.15f;
    public float minDistance = 2f;
    public float maxDistance = 12f;
    public float zoomStep = 0.75f;
    public float gamepadZoomSpeed = 4f;
    float yaw;

    void LateUpdate()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            return;
        }
        UpdateZoom();
        RotateCamera();
        FollowPlayer();
    }

    void UpdateZoom()
    {
        if (Mouse.current != null)
        {
            float scroll = Mouse.current.scroll.ReadValue().y;
            if (scroll > 0.01f)
            {
                distance -= zoomStep;
            }
            else if (scroll < -0.01f)
            {
                distance += zoomStep;
            }
        }
        if (Gamepad.current != null)
        {
            float dpad = Gamepad.current.dpad.ReadValue().y;
            distance -= dpad * gamepadZoomSpeed * Time.deltaTime;
        }
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }

    void RotateCamera()
    {
        Vector2 lookInput = Vector2.zero;
        if (Mouse.current != null && Mouse.current.rightButton.isPressed)
        {
            lookInput = Mouse.current.delta.ReadValue() * mouseSensitivity;
        }
        if (Gamepad.current != null)
        {
            lookInput += Gamepad.current.rightStick.ReadValue() * 100f * Time.deltaTime;
        }
        yaw += lookInput.x;
        pitch -= lookInput.y;
        pitch = Mathf.Clamp(pitch, 15f, 75f);
    }

    void FollowPlayer()
    {
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 pivot = target.position + Vector3.up * targetHeight;
        Vector3 backward = -transform.forward;
        float finalDistance = distance;
        RaycastHit[] hits = Physics.SphereCastAll(pivot, 0.2f, backward, distance,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform != target && !hit.transform.IsChildOf(target))
            {
                float wallDistance = Mathf.Max(0.05f, hit.distance - 0.1f);
                if (wallDistance < finalDistance)
                {
                    finalDistance = wallDistance;
                }
            }
        }
        transform.position = pivot + backward * finalDistance;
    }
}
