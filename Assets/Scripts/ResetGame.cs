using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetGame : MonoBehaviour
{
    // This function resets the current scene when called
    public void ResetGameScene()
    {
        // Reload the current scene by using the active scene index
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
