using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    // UI elements
    [Header("UI Elements")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject creditsPanel;
    
    // Buttons
    [Header("Main Menu Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button exitButton;
    
    // Options
    [Header("Options")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Button backFromOptionsButton;
    
    // Credits
    [Header("Credits")]
    [SerializeField] private Button backFromCreditsButton;
    
    // Background
    [Header("Background")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private float backgroundPanSpeed = 0.1f;
    
    // Audio
    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip buttonHoverSound;
    [SerializeField] private AudioClip buttonClickSound;
    
    // Animation
    [Header("Animation")]
    [SerializeField] private Animator logoAnimator;
    [SerializeField] private float buttonFadeInDelay = 0.5f;
    [SerializeField] private float buttonFadeInDuration = 1.0f;
    
    private void Start()
    {
        // Initialize UI
        ShowMainPanel();
        
        // Set up button listeners
        SetupButtonListeners();
        
        // Animate menu appearance
        StartCoroutine(AnimateMenuAppearance());
        
        // Check for save data
        CheckForSaveData();
    }
    
    private void Update()
    {
        // Animate background
        if (backgroundImage != null)
        {
            Vector2 offset = backgroundImage.material.mainTextureOffset;
            offset.x += backgroundPanSpeed * Time.deltaTime;
            offset.y += backgroundPanSpeed * 0.5f * Time.deltaTime;
            backgroundImage.material.mainTextureOffset = offset;
        }
    }
    
    private void SetupButtonListeners()
    {
        // Main menu buttons
        if (newGameButton != null)
        {
            newGameButton.onClick.AddListener(OnNewGameClicked);
            AddButtonSounds(newGameButton);
        }
        
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueClicked);
            AddButtonSounds(continueButton);
        }
        
        if (optionsButton != null)
        {
            optionsButton.onClick.AddListener(OnOptionsClicked);
            AddButtonSounds(optionsButton);
        }
        
        if (creditsButton != null)
        {
            creditsButton.onClick.AddListener(OnCreditsClicked);
            AddButtonSounds(creditsButton);
        }
        
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitClicked);
            AddButtonSounds(exitButton);
        }
        
        // Options panel buttons
        if (backFromOptionsButton != null)
        {
            backFromOptionsButton.onClick.AddListener(OnBackFromOptionsClicked);
            AddButtonSounds(backFromOptionsButton);
        }
        
        // Credits panel buttons
        if (backFromCreditsButton != null)
        {
            backFromCreditsButton.onClick.AddListener(OnBackFromCreditsClicked);
            AddButtonSounds(backFromCreditsButton);
        }
        
        // Options sliders
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }
        
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
        
        // Options toggles
        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);
        }
        
        // Options dropdowns
        if (resolutionDropdown != null)
        {
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        }
    }
    
    private void AddButtonSounds(Button button)
    {
        // Add hover sound
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }
        
        EventTrigger.Entry enterEntry = new EventTrigger.Entry();
        enterEntry.eventID = EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener((data) => { PlayButtonHoverSound(); });
        trigger.triggers.Add(enterEntry);
        
        // Add click sound to button
        button.onClick.AddListener(PlayButtonClickSound);
    }
    
    private void PlayButtonHoverSound()
    {
        if (sfxSource != null && buttonHoverSound != null)
        {
            sfxSource.PlayOneShot(buttonHoverSound);
        }
    }
    
    private void PlayButtonClickSound()
    {
        if (sfxSource != null && buttonClickSound != null)
        {
            sfxSource.PlayOneShot(buttonClickSound);
        }
    }
    
    private IEnumerator AnimateMenuAppearance()
    {
        // Animate logo
        if (logoAnimator != null)
        {
            logoAnimator.SetTrigger("Appear");
        }
        
        // Fade in buttons
        if (newGameButton != null)
        {
            CanvasGroup buttonGroup = newGameButton.GetComponent<CanvasGroup>();
            if (buttonGroup != null)
            {
                buttonGroup.alpha = 0;
                
                yield return new WaitForSeconds(buttonFadeInDelay);
                
                float elapsed = 0;
                while (elapsed < buttonFadeInDuration)
                {
                    buttonGroup.alpha = elapsed / buttonFadeInDuration;
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                
                buttonGroup.alpha = 1;
            }
        }
    }
    
    private void CheckForSaveData()
    {
        // Check if save data exists
        bool saveExists = false;
        
        if (GameManager.Instance != null && GameManager.Instance.SaveManager != null)
        {
            saveExists = GameManager.Instance.SaveManager.SaveExists(0);
        }
        
        // Enable/disable continue button
        if (continueButton != null)
        {
            continueButton.interactable = saveExists;
        }
    }
    
    // Panel visibility
    private void ShowMainPanel()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }
    
    private void ShowOptionsPanel()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(true);
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }
    
    private void ShowCreditsPanel()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(true);
    }
    
    // Button handlers
    private void OnNewGameClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartNewGame();
        }
    }
    
    private void OnContinueClicked()
    {
        if (GameManager.Instance != null && GameManager.Instance.SaveManager != null)
        {
            GameManager.Instance.SaveManager.LoadGame(0);
        }
    }
    
    private void OnOptionsClicked()
    {
        ShowOptionsPanel();
    }
    
    private void OnCreditsClicked()
    {
        ShowCreditsPanel();
    }
    
    private void OnExitClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.QuitGame();
        }
    }
    
    private void OnBackFromOptionsClicked()
    {
        ShowMainPanel();
    }
    
    private void OnBackFromCreditsClicked()
    {
        ShowMainPanel();
    }
    
    // Options handlers
    private void OnMusicVolumeChanged(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
        
        // Save setting
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }
    
    private void OnSFXVolumeChanged(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = volume;
        }
        
        // Save setting
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }
    
    private void OnFullscreenToggled(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        
        // Save setting
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    private void OnResolutionChanged(int resolutionIndex)
    {
        // Get available resolutions
        Resolution[] resolutions = Screen.resolutions;
        
        if (resolutionIndex < resolutions.Length)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            
            // Save setting
            PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
            PlayerPrefs.Save();
        }
    }
}
