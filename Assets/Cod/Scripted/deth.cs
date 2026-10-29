
using UnityEngine;

public class DeathHazard : MonoBehaviour
{
    private DeathMenuManager deathManager;

    private void Start()
    {
        deathManager = FindFirstObjectByType<DeathMenuManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (deathManager != null)
            deathManager.ShowDeathMenu();
        else
            Debug.LogWarning("DeathMenuManager was not found!");
    }
}
