using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
public class PilasPickUp : MonoBehaviour
{
    [Min(0f)] public float fuelSeconds = 30f;
    bool collected;
    public int pilas = 0;
    void Awake()
    {
        GetComponent<SphereCollider>().isTrigger = true;
        Rigidbody body = GetComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;
    }

    public bool TryCollect(PlayerController player)
    {
        if (collected || player == null) return false;
        {
            player.pilas += pilas;
            collected = true;
            gameObject.SetActive(false);
            Destroy(gameObject);
            return true;
        }
        return false;
    }
}
