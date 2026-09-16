using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryCondition : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene("WinScene");
        }
    }
}
