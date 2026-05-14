using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using UnityEngine.Video;
public class VideosController : MonoBehaviour
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

    [SerializeField] private VideoPlayer videoPlayerB;

    private VideoPlayer currentVideoPlayer;
    private VideoPlayer nextVideoPlayer;

    [Header("External Video")]
    [SerializeField] private string externalVideoPath = "/storage/emulated/0/Download/";
    [SerializeField] private string initialVideo = "POV_PLAT.mp4";
    [SerializeField] private AudioClip initialAudio;

    [Header("Options")]
    [SerializeField] private bool waitForVideoPrepare = true;
    [SerializeField] private float extraBlackDelay = 0.15f;

    [Header("UI Manager")]
    [SerializeField] private UIPerspectiveManager uiManager;


    [SerializeField] private VRViewRecenter viewRecenter;

    [SerializeField] private GameObject menuScreen;
    [SerializeField] private GameObject creditsScreen;
    [SerializeField] private GameObject fichaScreen;

    [Header("Rendering")]
    [SerializeField] private Renderer videoSphereRenderer;

    private bool _isTransitioning;

    [SerializeField] private bool isTesting = true;
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

            if (videoPlayer != null && videoPlayerB!=null)
            {
                videoPlayer.playOnAwake = false;
                videoPlayer.errorReceived += OnVideoError;

                // Configura o vídeo inicial, mas não dá play ainda
                videoPlayer.source = VideoSource.Url;
                videoPlayer.url = Path.Combine(externalVideoPath, initialVideo);
                videoPlayer.loopPointReached += OnVideoFinished;

                videoPlayerB.source = VideoSource.Url;

                videoPlayerB.playOnAwake = false;
                videoPlayerB.errorReceived += OnVideoError;
                videoPlayerB.loopPointReached += OnVideoFinished;

                currentVideoPlayer = videoPlayer;
                nextVideoPlayer = videoPlayerB;
            }
        }
        else
        {
            uiManager.ApplyPerspective(Perspective.Plateia);
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

            Vector3 audioRotation = setPath.GetAudioRot();
           

            StartCoroutine(TransitionToVideo(fullPath,-1,newClip,targetPerspective,audioRotation));
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
        StartCoroutine(TransitionToVideo(fullPath,0,initialAudio, Perspective.Plateia, audioSourceVideo.transform.localEulerAngles));
    }
    public void OnCreditsClicked() 
    {
        menuScreen.SetActive(false);
        creditsScreen.SetActive(true);
        fichaScreen.SetActive(false);
    }
    public void OnFichaClicked()
    {
        menuScreen.SetActive(false);
        creditsScreen.SetActive(false);
        fichaScreen.SetActive(true);
    }
    public void OnBackClicked()
    {
        menuScreen.SetActive(true);
        creditsScreen.SetActive(false);
        fichaScreen.SetActive(false);
    }

    public void OnExitShowClicked()
    {
        StartCoroutine(ReturnToLobby());
    }
    private IEnumerator TransitionToVideo(string videoUrl, double seekTime, AudioClip clip, Perspective perspective, Vector3 audioRotation)
    {
        _isTransitioning = true;

        // 1. Escurece a tela

        nextVideoPlayer.Stop();
        nextVideoPlayer.url = videoUrl;
        nextVideoPlayer.Prepare();
        
        yield return Fade(0f, 1f);

        // Hide the sphere while everything changes
        if (videoSphereRenderer != null)
            videoSphereRenderer.enabled = false;

        viewRecenter.FaceDefaultTarget();

        while (!nextVideoPlayer.isPrepared)
        {
            yield return null;
        }

        if (seekTime < 0)
        {
            seekTime = currentVideoPlayer.time;
        }

        // 2. Troca as interfaces
        if (lobbyRoot != null) lobbyRoot.SetActive(false);
        if (videoRoot != null) videoRoot.SetActive(true);

        // 3. Prepara o novo vídeo
        currentVideoPlayer.Stop();
        //audioSourceVideo.Stop(); // mudei aqui
        nextVideoPlayer.time = seekTime;
        
        audioSourceVideo.clip = clip;

        /*if (waitForVideoPrepare)
        {
            videoPlayer.Prepare();
            while (!videoPlayer.isPrepared)
                yield return null;
        }*/

       
        audioSourceVideo.time = (float)seekTime;


        if (extraBlackDelay > 0f)
            yield return new WaitForSeconds(extraBlackDelay);

        if (uiManager != null)
        {
            uiManager.ApplyPerspective(perspective);
        }
        


        if (nextVideoPlayer != null && audioSourceVideo != null)
        {
            nextVideoPlayer.Play();
            audioSourceVideo.transform.localEulerAngles = audioRotation;
            audioSourceVideo.Play();
        }

        yield return WaitUntilFirstFrame(nextVideoPlayer);

        if (videoSphereRenderer != null)
            videoSphereRenderer.enabled = true;

        // Swap references
        VideoPlayer temp = currentVideoPlayer;
        currentVideoPlayer = nextVideoPlayer;
        nextVideoPlayer = temp;

       

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
        _isTransitioning = false; // Destrava se der erro
    }

    private void OnVideoFinished(VideoPlayer source)
    {
        if (_isTransitioning) return;

        // Só reage ao player atualmente ativo
        if (source != currentVideoPlayer) return;

        StartCoroutine(ReturnToLobby());
    }

    private IEnumerator ReturnToLobby()
    {
        _isTransitioning = true;

        yield return Fade(0f, 1f);

        ResetPlaybackState();

        if (videoRoot != null) videoRoot.SetActive(false);
        if (lobbyRoot != null) lobbyRoot.SetActive(true);

        if (uiManager != null)
        {
            uiManager.ApplyPerspective(Perspective.Plateia);
        }

        yield return Fade(1f, 0f);

        _isTransitioning = false;
    }
    private IEnumerator WaitUntilFirstFrame(VideoPlayer player)
    {
        long startingFrame = player.frame;

        float timeout = 1.5f;
        float timer = 0f;

        while (player.frame <= startingFrame && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        yield return null;
    }

    private void ResetPlaybackState()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            videoPlayer.time = 0;
        }

        if (videoPlayerB != null)
        {
            videoPlayerB.Stop();
            videoPlayerB.time = 0;
        }

        if (audioSourceVideo != null)
        {
            audioSourceVideo.Stop();
            audioSourceVideo.time = 0f;
            audioSourceVideo.clip = null;
        }

        // Volta pro estado inicial do sistema
        currentVideoPlayer = videoPlayer;
        nextVideoPlayer = videoPlayerB;

        if (videoPlayer != null)
        {
            videoPlayer.url = Path.Combine(externalVideoPath, initialVideo);
        }
    }
}
