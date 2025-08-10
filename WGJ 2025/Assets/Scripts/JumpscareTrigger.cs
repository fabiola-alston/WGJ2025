using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class JumpscareTrigger : MonoBehaviour
{
    [Header("Trigger")]
    [Tooltip("Player must have tag 'Player'")]
    public string playerTag = "Player";
    public bool oneShot = true;

    [Header("Visuals")]
    [Tooltip("Red vignette sprite with transparent center (optional). If empty, a solid red fade is used.")]
    public Sprite redVignetteSprite;
    [Tooltip("The jumpscare image to show in the middle of the screen.")]
    public Sprite jumpscareSprite;

    [Header("Audio")]
    public AudioClip scareSound;
    [Range(0f, 1f)] public float scareVolume = 1f;

    [Header("Timing")]
    [Tooltip("Time to fade the red overlay in.")]
    public float redFadeIn = 0.35f;
    [Tooltip("Opacity of the red overlay once faded in (0-1).")]
    public float redMaxAlpha = 0.85f;
    [Tooltip("Time to pop-in the jumpscare image.")]
    public float imagePopIn = 0.12f;
    [Tooltip("How long to hold before reloading the scene.")]
    public float holdBeforeReload = 1.0f;

    [Header("Image Effects")]
    [Tooltip("Final scale of the jumpscare image (1 = original).")]
    public float imageFinalScale = 1.15f;
    [Tooltip("Start scale multiplier for a quick pop effect.")]
    public float imageStartScale = 0.75f;

    // Runtime UI
    private Canvas overlayCanvas;
    private Image redOverlay;
    private Image scareImage;
    private AudioSource audioSource;
    private bool triggered;

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true; 
    }

    void Awake()
    {
       
        overlayCanvas = new GameObject("JumpscareCanvas").AddComponent<Canvas>();
        overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        overlayCanvas.sortingOrder = short.MaxValue; // on top of everything

        var scaler = overlayCanvas.gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        overlayCanvas.gameObject.AddComponent<GraphicRaycaster>();

        
        var redGO = new GameObject("RedOverlay");
        redGO.transform.SetParent(overlayCanvas.transform, false);
        redOverlay = redGO.AddComponent<Image>();
        redOverlay.sprite = redVignetteSprite; 
        redOverlay.color = new Color(1f, 0f, 0f, 0f);
        redOverlay.type = redVignetteSprite ? Image.Type.Sliced : Image.Type.Simple;

        var rtf = redOverlay.rectTransform;
        rtf.anchorMin = Vector2.zero;
        rtf.anchorMax = Vector2.one;
        rtf.offsetMin = Vector2.zero;
        rtf.offsetMax = Vector2.zero;

        
        var imgGO = new GameObject("ScareImage");
        imgGO.transform.SetParent(overlayCanvas.transform, false);
        scareImage = imgGO.AddComponent<Image>();
        scareImage.sprite = jumpscareSprite;
        scareImage.color = new Color(1f, 1f, 1f, 0f); 
        var imgRT = scareImage.rectTransform;
        imgRT.anchorMin = new Vector2(0.5f, 0.5f);
        imgRT.anchorMax = new Vector2(0.5f, 0.5f);
        imgRT.pivot = new Vector2(0.5f, 0.5f);
        imgRT.sizeDelta = new Vector2(800, 800);
        imgRT.localScale = Vector3.one * imageStartScale;

        
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D
        audioSource.loop = false;
        audioSource.volume = scareVolume;

        
        overlayCanvas.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered && oneShot) return;
        if (!other.CompareTag(playerTag)) return;

        triggered = true;
        StartCoroutine(JumpscareSequence());
    }

    private IEnumerator JumpscareSequence()
    {
        
        overlayCanvas.gameObject.SetActive(true);

       
        if (scareSound)
        {
            audioSource.clip = scareSound;
            audioSource.volume = scareVolume;
            audioSource.Play();
        }

       
        float t = 0f;
        Color rc = redOverlay.color;
        while (t < redFadeIn)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(0f, redMaxAlpha, t / redFadeIn);
            redOverlay.color = new Color(rc.r, rc.g, rc.b, a);
            yield return null;
        }
        redOverlay.color = new Color(rc.r, rc.g, rc.b, redMaxAlpha);

        
        t = 0f;
        var imgRT = scareImage.rectTransform;
        while (t < imagePopIn)
        {
            t += Time.deltaTime;
            float k = t / imagePopIn;
            scareImage.color = new Color(1f, 1f, 1f, Mathf.SmoothStep(0f, 1f, k));
            float s = Mathf.SmoothStep(imageStartScale, imageFinalScale, k);
            imgRT.localScale = Vector3.one * s;
            yield return null;
        }
        scareImage.color = new Color(1f, 1f, 1f, 1f);
        imgRT.localScale = Vector3.one * imageFinalScale;

       
        yield return new WaitForSeconds(holdBeforeReload);

       
        if (audioSource.isPlaying) audioSource.Stop();

        
        Scene active = SceneManager.GetActiveScene();
        SceneManager.LoadScene(active.buildIndex);
    }
}