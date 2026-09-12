using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TopDownCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 12f, -7f);

    private void LateUpdate()
    {
        if (target == null)
            return;

        transform.position = target.position + offset;
        transform.rotation = Quaternion.LookRotation(-offset, Vector3.up);
    }
}
