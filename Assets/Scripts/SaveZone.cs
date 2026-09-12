using Unity.VisualScripting;
using UnityEngine;

public class SaveZone : MonoBehaviour
{
    [SerializeField] private Transform player;
    private float zone = 5.0f;
    private bool detectPlayer=false;
    void Update()
    {
        DetectPlayer();
    }
    private void DetectPlayer()
    {
        float distancePlayer = Vector3.Distance(transform.position, player.position);
        if (distancePlayer <zone && !detectPlayer)
        {
            detectPlayer = true;
            player.GetComponent<PlayerController>().EnterSafeZone();
            Debug.Log("El jugador está en la fogata");
        }
        else if (distancePlayer > zone && detectPlayer)
        {
            detectPlayer = false;
            player.GetComponent<PlayerController>().ExitSafeZone();

            Debug.Log("El jugador salió de la fogata");
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, zone);
    }
}
