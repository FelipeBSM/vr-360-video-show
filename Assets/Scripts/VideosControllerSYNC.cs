using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using UnityEngine.Video;

public class VideosControllerSYNC : MonoBehaviour
{
    [Header("Fade")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    [Header("Scene Roots")]
    [SerializeField] private GameObject lobbyRoot;
    [SerializeField] private GameObject videoRoot;

    [Header("Playback")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private AudioSource audioSourceVideo; // kept from your original
    [SerializeField] private AudioSource[] videoAudioSources; // use 2 sources here
    private int activeAudioSourceIndex = 0;

    [Header("External Video")]
    [SerializeField] private string externalVideoPath = "/storage/emulated/0/Download/";
    [SerializeField] private string initialVideo = "POV_PLAT.mp4";
    [SerializeField] private AudioClip initialAudio;

    [Header("Options")]
    [SerializeField] private bool waitForVideoPrepare = true;
    [SerializeField] private float extraBlackDelay = 0.15f;

    [Header("UI Manager")]
    [SerializeField] private UIPerspectiveManager uiManager;

    private bool _isTransitioning;

    [SerializeField] private bool isTesting = true;
    private bool hasStartedAudio = false;
    private void Awake()
    {
        if (!isTesting)
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageRead);
            }

            SetFadeAlpha(0f);
            if (videoRoot != null) videoRoot.SetActive(false);
            if (lobbyRoot != null) lobbyRoot.SetActive(true);

            if (videoPlayer != null)
            {
                videoPlayer.playOnAwake = false;
                videoPlayer.errorReceived += OnVideoError;

                // Configura o vídeo inicial, mas não dá play ainda
                videoPlayer.source = VideoSource.Url;
                videoPlayer.url = Path.Combine(externalVideoPath, initialVideo);
            }

            // AUDIO PART ADDED
            if (videoAudioSources != null)
            {
                for (int i = 0; i < videoAudioSources.Length; i++)
                {
                    if (videoAudioSources[i] == null) continue;
                    videoAudioSources[i].playOnAwake = false;
                    videoAudioSources[i].Stop();
                    videoAudioSources[i].volume = 0f;
                    videoAudioSources[i].clip = null;
                }
            }
        }
    }

    // Chamado pelos botões de mundo
    public void OnWorldButtonClicked(GameObject _button)
    {
        if (_isTransitioning) return;

        if (_button.TryGetComponent<SetPath>(out SetPath setPath))
        {
            string filename = setPath.GetVideoPath();
            Perspective targetPerspective = setPath.GetPerspective();

            string fullPath = Path.Combine(externalVideoPath, filename);
            AudioClip newClip = setPath.GetVideoAudioClip();

            StartCoroutine(TransitionToVideo(fullPath, -1, newClip, targetPerspective));
        }
        else
        {
            Debug.LogWarning("O botão clicado não possui o componente SetPath!");
        }
    }

    // Chamado pelo botão de Start simples (usa o vídeo inicial)
    public void OnStartButtonClicked()
    {
        if (_isTransitioning) return;
        string fullPath = Path.Combine(externalVideoPath, initialVideo);
        StartCoroutine(TransitionToVideo(fullPath, 0, initialAudio, Perspective.Plateia));
    }

    private IEnumerator TransitionToVideo(string videoUrl, double seekTime, AudioClip clip, Perspective perspective)
    {
        _isTransitioning = true;

        AudioSource currentAudio = null;
        AudioSource nextAudio = null;

        if (videoAudioSources != null && videoAudioSources.Length >= 2)
        {
            currentAudio = videoAudioSources[activeAudioSourceIndex];
            int nextIndex = (activeAudioSourceIndex + 1) % videoAudioSources.Length;
            nextAudio = videoAudioSources[nextIndex];
        }

        if (seekTime < 0)
        {
            seekTime = videoPlayer.time;
        }

        // Prepara vídeo
        videoPlayer.Stop();
        if (audioSourceVideo != null)
            audioSourceVideo.Stop();

        videoPlayer.url = videoUrl;

        if (waitForVideoPrepare)
        {
            videoPlayer.Prepare();
            while (!videoPlayer.isPrepared)
                yield return null;
        }

        // FIRST PLAY: populate currentAudio itself
        if (!hasStartedAudio)
        {
            if (currentAudio != null)
            {
                currentAudio.Stop();
                currentAudio.clip = null;
                currentAudio.time = 0f;
                currentAudio.volume = 0f;

                currentAudio.clip = clip;

                if (clip != null)
                {
                    float safeTime = Mathf.Clamp((float)seekTime, 0f, Mathf.Max(0f, clip.length - 0.05f));
                    currentAudio.time = safeTime;
                    currentAudio.Play();
                }
            }

            // Fade to black while audio fades in from 0 -> 0.5
            yield return FadeWithAudio(
                0f, 1f,
                null, currentAudio,
                0f, 0f,
                0f, 0.5f
            );

            if (lobbyRoot != null) lobbyRoot.SetActive(false);
            if (videoRoot != null) videoRoot.SetActive(true);

            if (extraBlackDelay > 0f)
                yield return new WaitForSeconds(extraBlackDelay);

            if (uiManager != null)
                uiManager.ApplyPerspective(perspective);

            double hiddenElapsed = fadeDuration + extraBlackDelay;
            videoPlayer.time = seekTime + hiddenElapsed;
            videoPlayer.Play();

            // Fade back while audio goes 0.5 -> 1
            yield return FadeWithAudio(
                1f, 0f,
                null, currentAudio,
                0f, 0f,
                0.5f, 1f
            );

            if (currentAudio != null)
                currentAudio.volume = 1f;

            hasStartedAudio = true;
            _isTransitioning = false;
            yield break;
        }

        // NORMAL TRANSITIONS AFTER FIRST ONE
        if (nextAudio != null)
        {
            nextAudio.Stop();
            nextAudio.clip = null;
            nextAudio.time = 0f;
            nextAudio.volume = 0f;

            nextAudio.clip = clip;

            if (clip != null)
            {
                float safeTime = Mathf.Clamp((float)seekTime, 0f, Mathf.Max(0f, clip.length - 0.05f));
                nextAudio.time = safeTime;
                nextAudio.Play();
            }
        }

        yield return FadeWithAudio(
            0f, 1f,
            currentAudio, nextAudio,
            1f, 0.5f,
            0f, 0.5f
        );

        if (lobbyRoot != null) lobbyRoot.SetActive(false);
        if (videoRoot != null) videoRoot.SetActive(true);

        if (extraBlackDelay > 0f)
            yield return new WaitForSeconds(extraBlackDelay);

        if (uiManager != null)
            uiManager.ApplyPerspective(perspective);

        double hiddenElapsedNormal = fadeDuration + extraBlackDelay;
        videoPlayer.time = seekTime + hiddenElapsedNormal;
        videoPlayer.Play();

        yield return FadeWithAudio(
            1f, 0f,
            currentAudio, nextAudio,
            0.5f, 0f,
            0.5f, 1f
        );

        if (currentAudio != null)
        {
            currentAudio.Stop();
            currentAudio.clip = null;
            currentAudio.volume = 0f;
        }

        if (nextAudio != null)
        {
            nextAudio.volume = 1f;
            activeAudioSourceIndex = (activeAudioSourceIndex + 1) % videoAudioSources.Length;
        }

        _isTransitioning = false;
    }

    // AUDIO PART ADDED
    private IEnumerator FadeWithAudio(
        float fadeFrom, float fadeTo,
        AudioSource currentAudio, AudioSource nextAudio,
        float currentFrom, float currentTo,
        float nextFrom, float nextTo)
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / fadeDuration);

            SetFadeAlpha(Mathf.Lerp(fadeFrom, fadeTo, t));

            if (currentAudio != null)
                currentAudio.volume = Mathf.Lerp(currentFrom, currentTo, t);

            if (nextAudio != null)
                nextAudio.volume = Mathf.Lerp(nextFrom, nextTo, t);

            yield return null;
        }

        SetFadeAlpha(fadeTo);

        if (currentAudio != null)
            currentAudio.volume = currentTo;

        if (nextAudio != null)
            nextAudio.volume = nextTo;
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
        _isTransitioning = false; // Destrava se der erro
    }
}