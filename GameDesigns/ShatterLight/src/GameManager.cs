// GameManager.cs - Central controller for the game
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public enum GameState
{
    MainMenu,
    WakingWorld,
    DreamWorld,
    Transition,
    MemoryFlashback,
    Paused
}

public class GameManager : MonoBehaviour
{
    // Singleton pattern
    public static GameManager Instance { get; private set; }
    
    // Game state
    public GameState CurrentGameState { get; private set; }
    
    // References to other managers
    public MemoryManager MemoryManager { get; private set; }
    public PlayerManager PlayerManager { get; private set; }
    public UIManager UIManager { get; private set; }
    public WorldManager WorldManager { get; private set; }
    public SaveManager SaveManager { get; private set; }
    
    // Transition settings
    [SerializeField] private float transitionDuration = 2.0f;
    [SerializeField] private GameObject transitionEffectPrefab;
    
    // Game progression tracking
    public float GameProgress { get; private set; }
    
    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Initialize managers
        InitializeManagers();
        
        // Set initial state
        CurrentGameState = GameState.MainMenu;
    }
    
    private void InitializeManagers()
    {
        // Find or create manager components
        MemoryManager = GetOrCreateManager<MemoryManager>();
        PlayerManager = GetOrCreateManager<PlayerManager>();
        UIManager = GetOrCreateManager<UIManager>();
        WorldManager = GetOrCreateManager<WorldManager>();
        SaveManager = GetOrCreateManager<SaveManager>();
    }
    
    private T GetOrCreateManager<T>() where T : Component
    {
        T manager = GetComponent<T>();
        
        if (manager == null)
        {
            manager = gameObject.AddComponent<T>();
        }
        
        return manager;
    }
    
    // Methods for state transitions
    public void TransitionToWakingWorld()
    {
        if (CurrentGameState == GameState.DreamWorld || CurrentGameState == GameState.MainMenu)
        {
            StartCoroutine(TransitionCoroutine(GameState.WakingWorld));
        }
    }
    
    public void TransitionToDreamWorld()
    {
        if (CurrentGameState == GameState.WakingWorld)
        {
            StartCoroutine(TransitionCoroutine(GameState.DreamWorld));
        }
    }
    
    public void ShowMemoryFlashback(string memoryId)
    {
        GameState previousState = CurrentGameState;
        StartCoroutine(MemoryFlashbackCoroutine(memoryId, previousState));
    }
    
    public void PauseGame()
    {
        if (CurrentGameState != GameState.Paused && CurrentGameState != GameState.MainMenu)
        {
            Time.timeScale = 0f;
            CurrentGameState = GameState.Paused;
            UIManager.ShowPauseMenu();
        }
    }
    
    public void ResumeGame()
    {
        if (CurrentGameState == GameState.Paused)
        {
            Time.timeScale = 1f;
            CurrentGameState = GameState.WakingWorld; // Or restore previous state
            UIManager.HidePauseMenu();
        }
    }
    
    private IEnumerator TransitionCoroutine(GameState targetState)
    {
        // Set transition state
        CurrentGameState = GameState.Transition;
        
        // Show transition effect
        GameObject transitionEffect = Instantiate(transitionEffectPrefab);
        
        // Wait for transition animation
        yield return new WaitForSeconds(transitionDuration / 2);
        
        // Load appropriate scene
        if (targetState == GameState.WakingWorld)
        {
            WorldManager.LoadWakingWorldScene();
            PlayerManager.ExitDreamState();
        }
        else if (targetState == GameState.DreamWorld)
        {
            WorldManager.LoadDreamWorldScene();
            PlayerManager.EnterDreamState();
        }
        
        // Wait for scene to load
        yield return new WaitForSeconds(transitionDuration / 2);
        
        // Update game state
        CurrentGameState = targetState;
        
        // Clean up transition effect
        Destroy(transitionEffect);
        
        // Update UI
        UIManager.UpdateHUD();
    }
    
    private IEnumerator MemoryFlashbackCoroutine(string memoryId, GameState returnState)
    {
        // Set flashback state
        CurrentGameState = GameState.MemoryFlashback;
        
        // Show transition effect
        GameObject transitionEffect = Instantiate(transitionEffectPrefab);
        
        // Wait for transition animation
        yield return new WaitForSeconds(transitionDuration / 2);
        
        // Load memory flashback scene
        WorldManager.LoadMemoryFlashbackScene(memoryId);
        
        // Wait for scene to load
        yield return new WaitForSeconds(transitionDuration / 2);
        
        // Clean up transition effect
        Destroy(transitionEffect);
        
        // Display memory content
        UIManager.DisplayMemoryFlashback(memoryId);
        
        // Wait for player input to continue
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0));
        
        // Transition back to previous state
        StartCoroutine(TransitionCoroutine(returnState));
    }
    
    public void UpdateGameProgress()
    {
        // Calculate progress based on collected memories
        int totalCoreMemories = MemoryManager.GetTotalCoreMemoriesCount();
        int collectedCoreMemories = MemoryManager.GetCollectedCoreMemoriesCount();
        
        if (totalCoreMemories > 0)
        {
            GameProgress = (float)collectedCoreMemories / totalCoreMemories;
        }
    }
    
    public void StartNewGame()
    {
        // Reset all game data
        SaveManager.ResetGameData();
        
        // Initialize starting conditions
        MemoryManager.InitializeMemories();
        PlayerManager.InitializePlayer();
        
        // Start in waking world
        TransitionToWakingWorld();
    }
    
    public void QuitGame()
    {
        // Save game before quitting
        SaveManager.SaveGame(0);
        
        // Quit application
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
