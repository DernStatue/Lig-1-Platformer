
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenuManager : MonoBehaviour
{
    public GameObject deathPanel;

    private bool isDead = false;

    void Start()
    {
        if (deathPanel != null)
            deathPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void ShowDeathMenu()
    {
        if (isDead) return;

        isDead = true;

        if (deathPanel != null)
            deathPanel.SetActive(true);

        // Pause gameplay.
        Time.timeScale = 0f;

        // Show the cursor for menu interaction.
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;

        // Change this to your main menu scene's name.
        SceneManager.LoadScene("MainMenu");
    }
}
