using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using UnityEngine.Video;
public class VideoInitBehaviour : MonoBehaviour
{
    [Header("Fade")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    [Header("Scene Roots")]
    [SerializeField] private GameObject lobbyRoot;
    [SerializeField] private GameObject videoRoot;

    [Header("Playback")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private AudioSource audioSourceVideo;

    [Header("External Video")]
    [SerializeField] private bool useExternalVideo = true;
    [SerializeField] private string externalVideoPath = "/storage/emulated/0/Downloads/";
    [SerializeField] private string initialVideo = "POV_PLAT.mp4";
    [SerializeField] private bool useFileProtocol = false;

    [Header("Options")]
    [SerializeField] private bool waitForVideoPrepare = true;
    [SerializeField] private float extraBlackDelay = 0.15f;

    private bool _isTransitioning;

    private void Awake()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
        }
        SetFadeAlpha(0f);

        if (videoRoot != null)
            videoRoot.SetActive(false);

        if (lobbyRoot != null)
            lobbyRoot.SetActive(true);

        if (videoPlayer != null)
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.waitForFirstFrame = true;


            videoPlayer.errorReceived += OnVideoError;

            if (useExternalVideo)
            {
                string finalUrl = externalVideoPath + initialVideo;

                videoPlayer.source = VideoSource.Url;
                videoPlayer.url = finalUrl;

                Debug.Log("Video URL set to: " + finalUrl);
                Debug.Log("Raw file exists: " + File.Exists(finalUrl));
            }





        }
    }
    private void Update()
    {
        if (videoPlayer != null && audioSourceVideo != null && videoPlayer.isPlaying)
        {
            float drift = Mathf.Abs(audioSourceVideo.time - (float)videoPlayer.time);

            if (drift > 0.05f) // 50 ms threshold
            {
                Debug.Log("Has drifted: " + drift);
                Debug.Log("Video current time: " + videoPlayer.time);
                Debug.Log("Audio current time: " + audioSourceVideo.time);
                //audioSourceVideo.time = (float)videoPlayer.time;
            }
        }
    }

    public void OnStartButtonClicked()
    {
        if (_isTransitioning) return;
        StartCoroutine(TransitionToVideo());
    }
    public void OnWorldButtonClicked(GameObject _button)
    {
        //gets the file name, send to video and then render
        if (_button.TryGetComponent<SetPath>(out SetPath setPath))
        {
            string filename = setPath.GetVideoPath();
            double currentTime = videoPlayer.time;
            videoPlayer.Pause();
            videoPlayer.url = externalVideoPath + filename;
            StartCoroutine(TransitionToVideo());
            videoPlayer.Prepare();
            
            videoPlayer.time = currentTime;
            videoPlayer.Play();
        }
        else
        {
            Debug.LogWarning("O botão clicado não possui o componente SetPath!");
        }
    }

    private IEnumerator TransitionToVideo()
    {
        _isTransitioning = true;

        yield return Fade(0f, 1f);

       
        if (lobbyRoot != null)
            lobbyRoot.SetActive(false);

        
        if (videoRoot != null)
            videoRoot.SetActive(true);

        if (videoPlayer != null && waitForVideoPrepare)
        {
            videoPlayer.Prepare();

            while (!videoPlayer.isPrepared)
                yield return null;
        }

        if (extraBlackDelay > 0f)
            yield return new WaitForSeconds(extraBlackDelay);

        if (videoPlayer != null && audioSourceVideo != null)
        {
            audioSourceVideo.time = (float)videoPlayer.time;
            videoPlayer.Play();
            audioSourceVideo.Play();
        }
          
        // Fade back from black
        yield return Fade(1f, 0f);

        _isTransitioning = false;
    }

    private IEnumerator Fade(float from, float to)
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / fadeDuration);
            SetFadeAlpha(Mathf.Lerp(from, to, t));
            yield return null;
        }

        SetFadeAlpha(to);
    }

    private void SetFadeAlpha(float alpha)
    {
        if (fadeImage == null) return;

        Color c = fadeImage.color;
        c.a = alpha;
        fadeImage.color = c;
    }

    private void OnVideoError(VideoPlayer source, string message)
    {
        Debug.LogError("VideoPlayer error: " + message);
    }
}
