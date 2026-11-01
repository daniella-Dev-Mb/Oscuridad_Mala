using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
public class PilasPickUp : MonoBehaviour
{
    bool collected;
    [Min(1)] public int pilas = 1;
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
            player.pilas += Mathf.Max(1, pilas);
            collected = true;
            gameObject.SetActive(false);
            Destroy(gameObject);
            return true;
        }
    }
}
