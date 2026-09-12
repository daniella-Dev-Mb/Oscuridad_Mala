using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
public class FuelPickup : MonoBehaviour
{
    [Min(0f)] public float fuelSeconds = 30f;
    bool collected;

    void Awake()
    {
        GetComponent<SphereCollider>().isTrigger = true;
        Rigidbody body = GetComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (!collected && player != null && player.AddFuel(fuelSeconds))
        {
            collected = true;
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
