using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Settings")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] public AudioClip buttonClickSound;
    [Range(0, 1)] public float maxMusicVolume = 0.5f;
    [Range(0, 1)] public float maxSFXVolume = 0.7f;

    [Header("UI References")]
    [SerializeField] private Button musicToggleButton;
    [SerializeField] private Sprite musicOnIcon;
    [SerializeField] private Sprite musicOffIcon;
    [SerializeField] private Button sfxToggleButton;
    [SerializeField] private Sprite sfxOnIcon;
    [SerializeField] private Sprite sfxOffIcon;

    private AudioSource musicSource;
    private AudioSource sfxSource;
    private bool isMusicOn = true;
    private bool isSFXOn = true;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeAudioSources();
        LoadPlayerPrefs();
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SetupButtonListeners();
        UpdateButtonVisuals();
        PlayBackgroundMusic();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindButtonsInScene();
        SetupButtonListeners();
        UpdateButtonVisuals();
    }

    private void FindButtonsInScene()
    {
        var musicButton = GameObject.FindGameObjectWithTag("Музыка")?.GetComponent<Button>();
        var sfxButton = GameObject.FindGameObjectWithTag("Звуковые эффекты")?.GetComponent<Button>();

        if (musicButton != null) musicToggleButton = musicButton;
        if (sfxButton != null) sfxToggleButton = sfxButton;
    }

    private void InitializeAudioSources()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.playOnAwake = false;
    }

    private void LoadPlayerPrefs()
    {
        isMusicOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        isSFXOn = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;
    }

    private void SetupButtonListeners()
    {
        if (musicToggleButton != null)
        {
            musicToggleButton.onClick.RemoveAllListeners();
            musicToggleButton.onClick.AddListener(() => {
                ToggleMusic();
                PlayButtonClickSound();
            });
        }

        if (sfxToggleButton != null)
        {
            sfxToggleButton.onClick.RemoveAllListeners();
            sfxToggleButton.onClick.AddListener(() => {
                ToggleSFX();
                PlayButtonClickSound();
            });
        }
    }

    public void PlaySound(AudioClip clip, float volumeModifier = 1f)
    {
        if (!isSFXOn || clip == null) return;

        float volume = Mathf.Clamp(maxSFXVolume * volumeModifier, 0f, 1f);
        sfxSource.PlayOneShot(clip, volume);
    }

    private void PlayButtonClickSound()
    {
        if (isSFXOn && buttonClickSound != null)
        {
            sfxSource.PlayOneShot(buttonClickSound, maxSFXVolume * 0.8f);
        }
    }

    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        PlayerPrefs.SetInt("MusicEnabled", isMusicOn ? 1 : 0);
        UpdateMusicState();
        UpdateButtonVisuals();
    }

    public void ToggleSFX()
    {
        isSFXOn = !isSFXOn;
        PlayerPrefs.SetInt("SFXEnabled", isSFXOn ? 1 : 0);
        UpdateButtonVisuals();
    }

    private void UpdateMusicState()
    {
        musicSource.volume = isMusicOn ? maxMusicVolume : 0f;

        if (isMusicOn && !musicSource.isPlaying)
        {
            musicSource.Play();
        }
        else if (!isMusicOn)
        {
            musicSource.Pause();
        }
    }

    private void UpdateButtonVisuals()
    {
        if (musicToggleButton != null)
        {
            musicToggleButton.image.sprite = isMusicOn ? musicOnIcon : musicOffIcon;
            var musicText = musicToggleButton.GetComponentInChildren<TMP_Text>();
            if (musicText != null)
                musicText.text = isMusicOn ? "Музыка включена" : "Музыка выключена";
        }

        if (sfxToggleButton != null)
        {
            sfxToggleButton.image.sprite = isSFXOn ? sfxOnIcon : sfxOffIcon;
            var sfxText = sfxToggleButton.GetComponentInChildren<TMP_Text>();
            if (sfxText != null)
                sfxText.text = isSFXOn ? "Звуковые эффекты включены" : "Звуковые эффекты выключены";
        }
    }

    private void PlayBackgroundMusic()
    {
        if (backgroundMusic == null) return;

        musicSource.volume = isMusicOn ? maxMusicVolume : 0f;
        if (isMusicOn)
        {
            musicSource.Play();
        }
    }

    public void SetMusicButtonReference(Button button)
    {
        musicToggleButton = button;
        SetupButtonListeners();
        UpdateButtonVisuals();
    }

    public void SetSFXButtonReference(Button button)
    {
        sfxToggleButton = button;
        SetupButtonListeners();
        UpdateButtonVisuals();
    }
}