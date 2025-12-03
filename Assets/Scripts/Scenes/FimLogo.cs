using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
public class FimLogo : MonoBehaviour
{
public VideoPlayer videoPlayer;
    public string sceneName = "MenuInicial";

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene(sceneName);
    }
}
