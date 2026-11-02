using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
public class FuelPickup : MonoBehaviour
{
    [FormerlySerializedAs("cantidad")]
    [Min(0.1f)] public float fuelSeconds = 30f;
    bool collected;

    void Awake()
    {
        GetComponent<SphereCollider>().isTrigger = true;
        Rigidbody body = GetComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;
    }

    public bool TryCollect(PlayerController player)
    {
        if (!collected && player != null && player.AddFuel(fuelSeconds))
        {
            collected = true;
            gameObject.SetActive(false);
            Destroy(gameObject);
            return true;
        }
        return false;
    }
}
