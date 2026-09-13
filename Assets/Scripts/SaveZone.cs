using UnityEngine;

public class SaveZone : MonoBehaviour
{
    [SerializeField] private Transform player;
    protected float zone = 5.0f;
    private PlayerController controller;
    private bool detectPlayer;

    protected virtual void Update()
    {
        if (controller == null)
        {
            controller = player != null ? player.GetComponentInParent<PlayerController>()
                : FindFirstObjectByType<PlayerController>();
            if (controller == null) return;
            player = controller.transform;
        }
        bool inside = controller.isActiveAndEnabled &&
            Vector3.Distance(transform.position, player.position) < zone;
        if (inside == detectPlayer) return;
        detectPlayer = inside;
        if (inside) controller.EnterSafeZone();
        else controller.ExitSafeZone();
    }

    protected virtual void OnDisable()
    {
        if (detectPlayer && controller != null) controller.ExitSafeZone();
        detectPlayer = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, zone);
    }
}
