using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Reference to the VideoPlayer component

    private void Start()
    {
        if (videoPlayer != null)
        {
            // Add a listener to the loopPointReached event
            videoPlayer.loopPointReached += OnVideoEnd;
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        // Load the PlayScene when the video ends
        SceneManager.LoadScene("HUWAT"); // Replace with your scene name
    }
}
