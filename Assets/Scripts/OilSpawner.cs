using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class OilSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> oilBottles = new List<GameObject>(); 

   
    void Start()
    {
        int random = Random.Range(0,oilBottles.Count);
        Instantiate(oilBottles[random], transform.position, transform.rotation);
    }

}
