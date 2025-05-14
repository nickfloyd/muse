using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseMenuUI : MonoBehaviour
{
    // UI panels
    [Header("UI Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject memoryJournalPanel;
    
    // Pause menu buttons
    [Header("Pause Menu Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button memoryJournalButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;
    
    // Options
    [Header("Options")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Button backFromOptionsButton;
    
    // Memory Journal
    [Header("Memory Journal")]
    [SerializeField] private RectTransform journalContainer;
    [SerializeField] private Button backFromJournalButton;
    
    // Audio
    [Header("Audio")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip buttonHoverSound;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip pauseSound;
    [SerializeField] private AudioClip unpauseSound;
    
    // Animation
    [Header("Animation")]
    [SerializeField] private Animator pauseAnimator;
    
    // Private variables
    private bool isPaused = false;
    
    private void Start()
    {
        // Hide all panels initially
        if (pausePanel != null) pausePanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (memoryJournalPanel != null) memoryJournalPanel.SetActive(false);
        
        // Set up button listeners
        SetupButtonListeners();
    }
    
    private void Update()
    {
        // Check for pause input
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    private void SetupButtonListeners()
    {
        // Pause menu buttons
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(OnResumeClicked);
            AddButtonSounds(resumeButton);
        }
        
        if (memoryJournalButton != null)
        {
            memoryJournalButton.onClick.AddListener(OnMemoryJournalClicked);
            AddButtonSounds(memoryJournalButton);
        }
        
        if (optionsButton != null)
        {
            optionsButton.onClick.AddListener(OnOptionsClicked);
            AddButtonSounds(optionsButton);
        }
        
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            AddButtonSounds(mainMenuButton);
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitClicked);
            AddButtonSounds(quitButton);
        }
        
        // Options panel buttons
        if (backFromOptionsButton != null)
        {
            backFromOptionsButton.onClick.AddListener(OnBackFromOptionsClicked);
            AddButtonSounds(backFromOptionsButton);
        }
        
        // Memory journal panel buttons
        if (backFromJournalButton != null)
        {
            backFromJournalButton.onClick.AddListener(OnBackFromJournalClicked);
            AddButtonSounds(backFromJournalButton);
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
    
    // Toggle pause state
    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    
    // Pause the game
    public void PauseGame()
    {
        isPaused = true;
        
        // Pause time
        Time.timeScale = 0f;
        
        // Show pause menu
        ShowPausePanel();
        
        // Play pause sound
        if (sfxSource != null && pauseSound != null)
        {
            sfxSource.PlayOneShot(pauseSound);
        }
        
        // Notify game manager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGamePaused();
        }
    }
    
    // Resume the game
    public void ResumeGame()
    {
        isPaused = false;
        
        // Resume time
        Time.timeScale = 1f;
        
        // Hide all panels
        if (pausePanel != null) pausePanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (memoryJournalPanel != null) memoryJournalPanel.SetActive(false);
        
        // Play unpause sound
        if (sfxSource != null && unpauseSound != null)
        {
            sfxSource.PlayOneShot(unpauseSound);
        }
        
        // Notify game manager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameResumed();
        }
    }
    
    // Panel visibility
    private void ShowPausePanel()
    {
        if (pausePanel != null) pausePanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (memoryJournalPanel != null) memoryJournalPanel.SetActive(false);
        
        // Animate panel
        if (pauseAnimator != null)
        {
            pauseAnimator.SetTrigger("Show");
        }
    }
    
    private void ShowOptionsPanel()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(true);
        if (memoryJournalPanel != null) memoryJournalPanel.SetActive(false);
    }
    
    private void ShowMemoryJournalPanel()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (memoryJournalPanel != null) memoryJournalPanel.SetActive(true);
        
        // Update memory journal visualization
        if (journalContainer != null && GameManager.Instance != null && 
            GameManager.Instance.MemoryManager != null && 
            GameManager.Instance.MemoryManager.Journal != null)
        {
            GameManager.Instance.MemoryManager.Journal.VisualizeJournal(journalContainer);
        }
    }
    
    // Button handlers
    private void OnResumeClicked()
    {
        ResumeGame();
    }
    
    private void OnMemoryJournalClicked()
    {
        ShowMemoryJournalPanel();
    }
    
    private void OnOptionsClicked()
    {
        ShowOptionsPanel();
    }
    
    private void OnMainMenuClicked()
    {
        // Confirm dialog would be good here
        
        // Resume time scale
        Time.timeScale = 1f;
        
        // Return to main menu
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToMainMenu();
        }
    }
    
    private void OnQuitClicked()
    {
        // Confirm dialog would be good here
        
        // Quit game
        if (GameManager.Instance != null)
        {
            GameManager.Instance.QuitGame();
        }
    }
    
    private void OnBackFromOptionsClicked()
    {
        ShowPausePanel();
    }
    
    private void OnBackFromJournalClicked()
    {
        ShowPausePanel();
    }
    
    // Options handlers
    private void OnMusicVolumeChanged(float volume)
    {
        if (GameManager.Instance != null && GameManager.Instance.AudioManager != null)
        {
            GameManager.Instance.AudioManager.SetMusicVolume(volume);
        }
        
        // Save setting
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }
    
    private void OnSFXVolumeChanged(float volume)
    {
        if (GameManager.Instance != null && GameManager.Instance.AudioManager != null)
        {
            GameManager.Instance.AudioManager.SetSFXVolume(volume);
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
