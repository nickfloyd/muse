# Shatter Light Chronicles - Code Documentation

## Overview

This document provides comprehensive documentation for the codebase of Shatter Light Chronicles, a narrative-driven action-adventure game built in Unity. The game features a dual-world system where the protagonist navigates between the waking world and a dream world to recover lost memories.

## Core Systems Architecture

The game is built using a modular architecture with several manager classes that control different aspects of gameplay. These managers communicate through the central GameManager class, which serves as the primary coordinator.

### Manager Classes Hierarchy

```
GameManager
├── PlayerManager
│   ├── PlayerMovement
│   ├── PlayerCombat
│   └── PlayerMemory
├── MemoryManager
│   └── MemoryJournal
├── WorldManager
│   ├── WakingWorld
│   └── DreamWorld
├── UIManager
│   ├── HUD
│   ├── MemoryJournalUI
│   └── DialogueSystem
├── SaveManager
└── StoryManager
```

## Key Classes and Components

### GameManager

**Purpose**: Central controller for the game that manages state transitions and coordinates other manager classes.

**Key Methods**:
- `TransitionToWakingWorld()`: Handles transition to the waking world state
- `TransitionToDreamWorld()`: Handles transition to the dream world state
- `ShowMemoryFlashback(string memoryId)`: Triggers memory flashback sequence
- `PauseGame()` / `ResumeGame()`: Controls game pause state
- `UpdateGameProgress()`: Calculates and updates overall game progression
- `StartNewGame()`: Initializes a new game session

**Usage Example**:
```csharp
// Transition to dream world when player falls asleep
public void PlayerSleep()
{
    GameManager.Instance.TransitionToDreamWorld();
}

// Show memory flashback when player interacts with memory trigger
public void TriggerMemoryFlashback(string memoryId)
{
    GameManager.Instance.ShowMemoryFlashback(memoryId);
}
```

### PlayerManager

**Purpose**: Manages the player character, including movement, combat, and state changes between worlds.

**Key Methods**:
- `UpdatePlayerAppearance(bool dreamWorld)`: Changes player appearance based on world state
- `ApplyMemoryEffects(List<MemoryFragment> activeMemories)`: Applies effects from collected memories
- `EnterDreamState()` / `ExitDreamState()`: Handles player state changes between worlds
- `EnterCombat()` / `ExitCombat()`: Manages combat state transitions

**Usage Example**:
```csharp
// Apply effects from newly collected memory
public void OnMemoryCollected(MemoryFragment fragment)
{
    List<MemoryFragment> allMemories = GameManager.Instance.MemoryManager.GetAllCollectedMemories();
    GameManager.Instance.PlayerManager.ApplyMemoryEffects(allMemories);
}
```

### MemoryManager

**Purpose**: Handles memory fragment collection, storage, and effects on gameplay.

**Key Methods**:
- `CollectMemoryFragment(string fragmentId)`: Adds a memory fragment to the collected list
- `ViewMemoryFragment(string fragmentId)`: Triggers viewing of a collected memory
- `GetCollectedMemoriesByType(MemoryType type)`: Retrieves memories of a specific type
- `ApplyMemoryEffects()`: Applies gameplay effects from collected memories

**Usage Example**:
```csharp
// When player finds a memory fragment in the environment
public void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
        GameManager.Instance.MemoryManager.CollectMemoryFragment(memoryFragmentId);
        Destroy(gameObject);
    }
}
```

### Memory Fragment Classes

**Base Class**: `MemoryFragment`

**Derived Classes**:
- `CoreMemoryFragment`: Essential story memories
- `SkillMemoryFragment`: Memories that unlock player abilities
- `PeripheralMemoryFragment`: Optional backstory memories

**Key Methods**:
- `OnCollect()`: Called when a memory is collected
- `OnView()`: Called when a memory is viewed in the journal
- `ApplyEffects()`: Applies gameplay effects specific to the memory type

**Usage Example**:
```csharp
// Creating a new skill memory fragment
SkillMemoryFragmentData skillData = ScriptableObject.CreateInstance<SkillMemoryFragmentData>();
skillData.Id = "skill_memory_001";
skillData.Title = "Memory Projection";
skillData.Description = "The ability to project memory light as an attack.";
skillData.Type = MemoryType.Skill;
// ... set other properties

SkillMemoryFragment fragment = new SkillMemoryFragment(skillData);
```

### PlayerMovement

**Purpose**: Handles player character movement and controls.

**Key Methods**:
- `Update()`: Processes input and moves the character
- `ForceMove(Vector3 direction, float speed, bool rotateToDirection)`: Forces movement for cutscenes
- `Teleport(Vector3 position)`: Instantly moves player to a new position

**Usage Example**:
```csharp
// Teleport player to a new location after a memory flashback
public void AfterMemoryFlashback(Transform targetPosition)
{
    GameManager.Instance.PlayerManager.Movement.Teleport(targetPosition.position);
}
```

### PlayerCombat

**Purpose**: Handles player combat abilities and interactions.

**Key Methods**:
- `BasicAttack()`: Performs basic attack
- `UseAbility(int index)`: Uses a memory-based ability
- `UpdateAbilities()`: Updates available abilities based on collected memories
- `AddMemoryEnergy(float amount)`: Adds energy for using abilities

**Usage Example**:
```csharp
// When player collects an energy pickup
public void OnEnergyPickup(float energyAmount)
{
    GameManager.Instance.PlayerManager.Combat.AddMemoryEnergy(energyAmount);
}
```

### MemoryJournal

**Purpose**: Visualizes collected memories and their connections.

**Key Methods**:
- `AddMemory(MemoryFragment fragment)`: Adds a memory to the journal
- `CreateConnection(string sourceId, string targetId)`: Creates a connection between memories
- `GetConnectedMemories(string fragmentId)`: Gets memories connected to a specific fragment
- `VisualizeJournal(RectTransform journalContainer)`: Creates visual representation of the journal

**Usage Example**:
```csharp
// Visualize the journal when player opens it
public void OpenJournal()
{
    RectTransform journalContainer = journalPanel.GetComponent<RectTransform>();
    GameManager.Instance.MemoryManager.Journal.VisualizeJournal(journalContainer);
}
```

## Data Structures

### ScriptableObjects

The game uses ScriptableObjects to store data that doesn't change during runtime:

- `MemoryFragmentData`: Base class for memory fragment data
- `CoreMemoryFragmentData`: Data for core story memories
- `SkillMemoryFragmentData`: Data for skill-unlocking memories
- `PeripheralMemoryFragmentData`: Data for optional backstory memories

**Usage Example**:
```csharp
// Loading memory data from resources
void LoadMemoryData()
{
    MemoryFragmentData[] allMemoryData = Resources.LoadAll<MemoryFragmentData>("MemoryData");
    foreach (MemoryFragmentData data in allMemoryData)
    {
        // Process each memory data
    }
}
```

### Save System

Game saves use JSON serialization to store player progress:

```csharp
[System.Serializable]
public class GameSaveData
{
    public string SaveName;
    public System.DateTime SaveDate;
    public float PlayTime;
    
    public PlayerSaveData PlayerData;
    public List<string> CollectedMemoryIds;
    public WorldSaveData WorldData;
    public QuestSaveData QuestData;
    
    public string LastCheckpointId;
    public string CurrentSceneId;
}
```

**Usage Example**:
```csharp
// Saving game data
public void SaveGame(int saveSlot)
{
    GameSaveData saveData = new GameSaveData();
    saveData.SaveName = "Save " + saveSlot;
    saveData.SaveDate = System.DateTime.Now;
    saveData.PlayTime = GameManager.Instance.PlayTime;
    
    // Populate other data
    
    string json = JsonUtility.ToJson(saveData);
    string savePath = Application.persistentDataPath + "/save" + saveSlot + ".json";
    System.IO.File.WriteAllText(savePath, json);
}
```

## Input System

The game uses Unity's new Input System package for flexible control mapping:

```csharp
public class InputController : MonoBehaviour
{
    // Input actions
    private InputActionMap _gameplayActions;
    private InputActionMap _uiActions;
    
    // Current action map
    private InputActionMap _currentActionMap;
    
    // Input events
    public event System.Action<Vector2> OnMove;
    public event System.Action OnJump;
    public event System.Action OnAttack;
    public event System.Action OnDefend;
    public event System.Action OnSpecialAbility;
    public event System.Action OnOpenMemoryJournal;
}
```

**Usage Example**:
```csharp
// Setting up input in PlayerMovement
void Start()
{
    InputController inputController = GameManager.Instance.InputController;
    inputController.OnMove += HandleMovement;
    inputController.OnJump += HandleJump;
}

void HandleMovement(Vector2 moveInput)
{
    // Process movement input
}
```

## Scene Management

The game is structured with the following key scenes:

1. **Main Menu**: Title screen, save management, options
2. **Waking World Scenes**: Various locations in Lumina City
3. **Dream World Scenes**: Different areas of the dreamscape
4. **Transition Scenes**: Special scenes for world transitions
5. **Memory Flashback Scenes**: Visualization of recovered memories

Scene transitions are managed through a dedicated SceneController:

```csharp
public class SceneController : MonoBehaviour
{
    // Scene references
    private Dictionary<string, string> _sceneMap;
    
    // Transition effects
    public GameObject TransitionEffectPrefab;
    
    // Methods
    public void LoadScene(string sceneId, TransitionType transitionType);
    public void LoadWakingWorldScene(string locationId);
    public void LoadDreamWorldScene(string dreamAreaId);
    public void LoadMemoryFlashback(string memoryId);
}
```

## Performance Optimization

### Object Pooling

Used for frequently instantiated objects like memory fragments and combat effects:

```csharp
public class ObjectPool : MonoBehaviour
{
    public GameObject prefab;
    public int initialSize = 10;
    
    private List<GameObject> pool;
    
    void Start()
    {
        InitializePool();
    }
    
    void InitializePool()
    {
        pool = new List<GameObject>();
        
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }
    
    public GameObject GetObject()
    {
        // Find inactive object in pool
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }
        
        // If no inactive objects, create new one
        GameObject newObj = Instantiate(prefab);
        pool.Add(newObj);
        return newObj;
    }
    
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
    }
}
```

### Asynchronous Loading

Used for scene transitions to prevent hitching:

```csharp
public IEnumerator LoadSceneAsync(string sceneName, TransitionType transitionType)
{
    // Show transition effect
    GameObject transitionEffect = Instantiate(TransitionEffectPrefab);
    
    // Wait for transition animation
    yield return new WaitForSeconds(transitionDuration / 2);
    
    // Load scene asynchronously
    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
    
    // Wait until scene is loaded
    while (!asyncLoad.isDone)
    {
        yield return null;
    }
    
    // Wait for transition animation
    yield return new WaitForSeconds(transitionDuration / 2);
    
    // Clean up transition effect
    Destroy(transitionEffect);
}
```

## Extension and Modification Guidelines

### Adding New Memory Types

1. Create a new class that inherits from `MemoryFragment`
2. Create a corresponding data class that inherits from `MemoryFragmentData`
3. Implement the required methods: `OnCollect()`, `OnView()`, and `ApplyEffects()`
4. Update the `MemoryManager.CreateMemoryFromData()` method to handle the new type
5. Add UI support in the `MemoryJournal` class

### Adding New Abilities

1. Create a new class that inherits from `PlayerAbility`
2. Implement the required methods: `CanUse()`, `Use()`, and `ApplyUpgrade()`
3. Create a new `SkillMemoryFragmentData` that references the ability
4. Add visual effects and sound effects for the ability
5. Update the UI to display the new ability

### Adding New Environments

1. Create a new scene for the environment
2. Add the scene to the build settings
3. Update the `SceneController` to handle the new scene
4. Add any memory fragments or interactive elements
5. Update the `WorldManager` to include the new environment

## Debugging Tools

The game includes several debugging tools to help with development:

```csharp
public class DebugTools : MonoBehaviour
{
    // Debug UI
    public bool showDebugUI = false;
    
    // Methods
    public void ToggleDebugUI();
    public void GiveAllMemories();
    public void UnlockAllAreas();
    public void SetMemoryEnergy(float amount);
    public void TeleportToLocation(string locationId);
}
```

To enable debugging, press Ctrl+Shift+D during gameplay.

## Known Issues and Limitations

1. Memory journal visualization may slow down with very large numbers of memories (>100)
2. Transition effects may cause brief frame rate drops on lower-end hardware
3. Some physics interactions in the dream world can behave unpredictably
4. Memory fragment collection radius may need adjustment in certain environments

## Future Development Considerations

1. Add support for additional memory types
2. Implement a more advanced combat combo system
3. Expand the dream world procedural generation
4. Add more interactive elements to the memory journal
5. Implement additional accessibility features
