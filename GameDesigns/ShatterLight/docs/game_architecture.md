# Shatter Light Chronicles - Game Architecture Document

## Game Engine Selection

After evaluating several game engines for the development of Shatter Light Chronicles, we've selected **Unity** as our primary development platform for the following reasons:

1. **Cross-platform capabilities**: Unity allows for deployment across multiple platforms including PC, Mac, consoles, and mobile devices
2. **Robust 2D and 3D support**: The game will feature both 2D elements (UI, memory journal) and 3D environments (dreamscape)
3. **C# programming**: Offers a balance of power and accessibility for implementing complex game systems
4. **Asset Store**: Provides access to a wide range of assets to accelerate development
5. **Strong community support**: Extensive documentation and community resources
6. **Animation tools**: Built-in animation system for character movements and transitions between worlds
7. **Particle systems**: Essential for creating memory fragment effects and dreamscape visuals
8. **Shader support**: Necessary for creating the distinctive visual styles of the waking and dream worlds

## Development Environment Setup

### Required Software
- Unity 2022.3 LTS or newer
- Visual Studio 2022 or JetBrains Rider for C# development
- Git for version control
- GitHub for repository hosting
- Adobe Creative Cloud for asset creation and modification
- Audacity for basic sound editing

### Project Structure
The Unity project will follow a standard organization with some custom folders for game-specific elements:

```
ShatterLightChronicles/
├── Assets/
│   ├── Animations/
│   │   ├── Characters/
│   │   ├── Environment/
│   │   └── UI/
│   ├── Audio/
│   │   ├── Music/
│   │   ├── SFX/
│   │   └── Voice/
│   ├── Materials/
│   │   ├── Characters/
│   │   ├── Environment/
│   │   ├── Effects/
│   │   └── UI/
│   ├── Models/
│   │   ├── Characters/
│   │   ├── Environment/
│   │   └── Props/
│   ├── Prefabs/
│   │   ├── Characters/
│   │   ├── Environment/
│   │   ├── UI/
│   │   └── Systems/
│   ├── Scenes/
│   │   ├── MainMenu/
│   │   ├── WakingWorld/
│   │   ├── DreamWorld/
│   │   └── Transitions/
│   ├── Scripts/
│   │   ├── Characters/
│   │   ├── Combat/
│   │   ├── Core/
│   │   ├── Environment/
│   │   ├── Managers/
│   │   ├── Memory/
│   │   ├── UI/
│   │   └── Utilities/
│   ├── Shaders/
│   │   ├── Characters/
│   │   ├── Environment/
│   │   ├── Memory/
│   │   └── PostProcessing/
│   ├── Textures/
│   │   ├── Characters/
│   │   ├── Environment/
│   │   ├── UI/
│   │   └── Effects/
│   └── ThirdParty/
├── Packages/
├── ProjectSettings/
└── UserSettings/
```

## Class Hierarchy and System Design

### Core Systems

#### GameManager
The central controller that manages game state, scene transitions, and coordinates other manager classes.

```csharp
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
    
    // Methods for state transitions
    public void TransitionToWakingWorld();
    public void TransitionToDreamWorld();
    public void PauseGame();
    public void ResumeGame();
    
    // Game progression tracking
    public float GameProgress { get; private set; }
    public void UpdateGameProgress();
}
```

#### MemoryManager
Handles all aspects of memory fragment collection, storage, and effects.

```csharp
public class MemoryManager : MonoBehaviour
{
    // Collection of all memory fragments
    private Dictionary<string, MemoryFragment> _allMemoryFragments;
    
    // Currently collected memories
    private List<MemoryFragment> _collectedMemories;
    
    // Memory journal visualization
    public MemoryJournal Journal { get; private set; }
    
    // Methods for memory management
    public void CollectMemoryFragment(string fragmentId);
    public void ViewMemoryFragment(string fragmentId);
    public bool HasMemoryFragment(string fragmentId);
    public List<MemoryFragment> GetCollectedMemoriesByType(MemoryType type);
    
    // Memory effects on gameplay
    public void ApplyMemoryEffects();
}
```

#### PlayerManager
Controls the player character, including movement, combat, and state changes between worlds.

```csharp
public class PlayerManager : MonoBehaviour
{
    // Player state
    public PlayerState CurrentPlayerState { get; private set; }
    
    // Player components
    public PlayerMovement Movement { get; private set; }
    public PlayerCombat Combat { get; private set; }
    public PlayerMemory Memory { get; private set; }
    
    // Player stats
    public PlayerStats Stats { get; private set; }
    
    // Methods for player control
    public void UpdatePlayerAppearance(bool isInDreamWorld);
    public void ApplyMemoryEffects(List<MemoryFragment> activeMemories);
    public void EnterDreamState();
    public void ExitDreamState();
}
```

#### WorldManager
Manages the two distinct worlds and transitions between them.

```csharp
public class WorldManager : MonoBehaviour
{
    // World states
    public WorldState CurrentWorldState { get; private set; }
    
    // World components
    public WakingWorld WakingWorld { get; private set; }
    public DreamWorld DreamWorld { get; private set; }
    
    // Methods for world management
    public void TransitionToWakingWorld();
    public void TransitionToDreamWorld();
    public void UpdateWorldBasedOnMemories(List<MemoryFragment> activeMemories);
    public void SpawnMemoryGuardian(string guardianId);
}
```

#### UIManager
Handles all user interface elements and interactions.

```csharp
public class UIManager : MonoBehaviour
{
    // UI components
    public MainMenu MainMenu { get; private set; }
    public HUD GameHUD { get; private set; }
    public MemoryJournalUI JournalUI { get; private set; }
    public DialogueSystem DialogueSystem { get; private set; }
    
    // Methods for UI management
    public void ShowMemoryJournal();
    public void HideMemoryJournal();
    public void DisplayMemoryFlashback(string fragmentId);
    public void UpdateHUD();
}
```

#### SaveManager
Handles game saving and loading functionality.

```csharp
public class SaveManager : MonoBehaviour
{
    // Save data
    private GameSaveData _currentSaveData;
    
    // Methods for save management
    public void SaveGame(int saveSlot);
    public void LoadGame(int saveSlot);
    public bool DoesSaveExist(int saveSlot);
    public void DeleteSave(int saveSlot);
    
    // Autosave functionality
    public void AutoSave();
}
```

### Memory System Classes

#### MemoryFragment
Base class for all memory fragments in the game.

```csharp
public abstract class MemoryFragment
{
    // Basic properties
    public string Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public MemoryType Type { get; private set; }
    public bool IsCollected { get; private set; }
    
    // Visual representation
    public Color FragmentColor { get; private set; }
    public GameObject FragmentPrefab { get; private set; }
    
    // Narrative content
    public string[] DialogueLines { get; private set; }
    public AudioClip VoiceOver { get; private set; }
    
    // Methods
    public virtual void OnCollect();
    public virtual void OnView();
    public virtual void ApplyEffects();
}
```

#### CoreMemoryFragment
Represents essential story memories.

```csharp
public class CoreMemoryFragment : MemoryFragment
{
    // Story progression
    public int StoryPhase { get; private set; }
    
    // World changes triggered by this memory
    public WorldChangeData WorldChanges { get; private set; }
    
    // Override methods
    public override void OnCollect();
    public override void ApplyEffects();
}
```

#### SkillMemoryFragment
Represents memories that unlock player abilities.

```csharp
public class SkillMemoryFragment : MemoryFragment
{
    // Skill data
    public PlayerAbility UnlockedAbility { get; private set; }
    
    // Override methods
    public override void OnCollect();
    public override void ApplyEffects();
}
```

#### PeripheralMemoryFragment
Represents optional backstory memories.

```csharp
public class PeripheralMemoryFragment : MemoryFragment
{
    // Connections to other memories
    public string[] ConnectedMemoryIds { get; private set; }
    
    // Override methods
    public override void OnCollect();
    public override void OnView();
}
```

#### MemoryJournal
Visualizes collected memories and their connections.

```csharp
public class MemoryJournal
{
    // Journal data
    private Dictionary<string, MemoryNode> _memoryNodes;
    private List<MemoryConnection> _memoryConnections;
    
    // Methods
    public void AddMemory(MemoryFragment fragment);
    public void CreateConnection(string sourceId, string targetId);
    public List<MemoryFragment> GetConnectedMemories(string fragmentId);
    public void VisualizeJournal();
}
```

### Character System Classes

#### Character
Base class for all characters in the game.

```csharp
public abstract class Character : MonoBehaviour
{
    // Basic properties
    public string CharacterId { get; protected set; }
    public string CharacterName { get; protected set; }
    
    // Stats
    public CharacterStats Stats { get; protected set; }
    
    // Components
    protected Animator _animator;
    protected Rigidbody _rigidbody;
    
    // Methods
    public abstract void Initialize();
    public virtual void TakeDamage(float damage);
    public virtual void Die();
}
```

#### PlayerCharacter
The playable protagonist character.

```csharp
public class PlayerCharacter : Character
{
    // Player-specific properties
    public PlayerInventory Inventory { get; private set; }
    public PlayerAbilities Abilities { get; private set; }
    
    // World state
    public bool IsInDreamWorld { get; private set; }
    
    // Methods
    public override void Initialize();
    public void SwitchWorldState(bool enterDreamWorld);
    public void CollectMemoryFragment(MemoryFragment fragment);
    public void UseAbility(string abilityId);
}
```

#### MemoryGuardian
Boss enemies that protect memory fragments.

```csharp
public class MemoryGuardian : Character
{
    // Guardian-specific properties
    public string GuardedMemoryId { get; private set; }
    public GuardianPhase[] CombatPhases { get; private set; }
    
    // AI behavior
    private GuardianAI _ai;
    
    // Methods
    public override void Initialize();
    public void ActivatePhase(int phaseIndex);
    public override void Die();
}
```

#### NPC
Non-player characters that interact with the player.

```csharp
public class NPC : Character
{
    // NPC-specific properties
    public NPCType Type { get; private set; }
    public DialogueTree Dialogue { get; private set; }
    public QuestData[] AvailableQuests { get; private set; }
    
    // Methods
    public override void Initialize();
    public void Interact();
    public void UpdateDialogueBasedOnMemories(List<MemoryFragment> collectedMemories);
}
```

### Combat System Classes

#### CombatSystem
Manages combat interactions between characters.

```csharp
public class CombatSystem : MonoBehaviour
{
    // Combat state
    public bool InCombat { get; private set; }
    
    // Active combatants
    private List<Character> _activeCombatants;
    
    // Methods
    public void StartCombat(Character player, Character[] enemies);
    public void EndCombat();
    public void ProcessAttack(Character attacker, Character target, Attack attack);
    public void ProcessDefense(Character defender, Attack incomingAttack);
}
```

#### Attack
Represents an offensive combat action.

```csharp
public class Attack
{
    // Attack properties
    public string AttackName { get; private set; }
    public float BaseDamage { get; private set; }
    public AttackType Type { get; private set; }
    public ElementType Element { get; private set; }
    
    // Visual effects
    public GameObject VisualEffectPrefab { get; private set; }
    public AudioClip SoundEffect { get; private set; }
    
    // Methods
    public float CalculateDamage(CharacterStats attackerStats, CharacterStats defenderStats);
    public void ApplyEffects(Character target);
}
```

#### PlayerAbility
Special actions the player can perform using memory power.

```csharp
public class PlayerAbility
{
    // Ability properties
    public string AbilityId { get; private set; }
    public string AbilityName { get; private set; }
    public string Description { get; private set; }
    public float MemoryEnergyCost { get; private set; }
    public float Cooldown { get; private set; }
    
    // Unlock requirements
    public string RequiredMemoryId { get; private set; }
    
    // Visual representation
    public Sprite AbilityIcon { get; private set; }
    public GameObject VisualEffectPrefab { get; private set; }
    
    // Methods
    public bool CanUse(float currentMemoryEnergy);
    public void Use(PlayerCharacter player, Vector3 targetPosition);
    public void ApplyUpgrade(AbilityUpgrade upgrade);
}
```

## Data Management

### ScriptableObjects
Unity's ScriptableObjects will be used to store game data that doesn't change during runtime:

```csharp
[CreateAssetMenu(fileName = "NewMemoryData", menuName = "Shatter Light/Memory Data")]
public class MemoryFragmentData : ScriptableObject
{
    public string Id;
    public string Title;
    public string Description;
    public MemoryType Type;
    public Color FragmentColor;
    public GameObject FragmentPrefab;
    public string[] DialogueLines;
    public AudioClip VoiceOver;
    
    // Type-specific data
    public int StoryPhase;
    public WorldChangeData WorldChanges;
    public PlayerAbility UnlockedAbility;
    public string[] ConnectedMemoryIds;
}
```

### Save System
Game saves will use JSON serialization to store player progress:

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

## Scene Management

The game will be structured with the following key scenes:

1. **Main Menu**: Title screen, save management, options
2. **Waking World Scenes**: Various locations in Lumina City
3. **Dream World Scenes**: Different areas of the dreamscape
4. **Transition Scenes**: Special scenes for world transitions
5. **Memory Flashback Scenes**: Visualization of recovered memories

Scene transitions will be managed through a dedicated SceneController:

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

## Input System

The game will use Unity's new Input System package for flexible control mapping:

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
    
    // Methods
    public void SwitchToGameplayControls();
    public void SwitchToUIControls();
    public void EnableControls();
    public void DisableControls();
}
```

## Technical Considerations

### Performance Optimization
- Level of Detail (LOD) system for complex dreamscape environments
- Object pooling for memory fragments and combat effects
- Asynchronous loading for scene transitions
- Shader optimization for memory effects

### Memory Management
- Resource loading and unloading strategy
- Asset bundles for downloadable content
- Memory fragment data compression

### Cross-Platform Considerations
- Scalable UI for different screen resolutions
- Input adaptation for keyboard/mouse, controller, and touch
- Graphics quality settings for different hardware capabilities

## Development Roadmap

1. **Prototype Phase**
   - Basic player movement and camera
   - Simple waking/dream world transition
   - Memory fragment collection prototype
   - Combat system basics

2. **Alpha Phase**
   - Complete core gameplay loop
   - Basic memory system implementation
   - Initial character models and animations
   - Preliminary UI design

3. **Beta Phase**
   - Complete story implementation
   - Full memory system with journal
   - Refined combat and abilities
   - Complete level design
   - Audio implementation

4. **Polish Phase**
   - Visual effects refinement
   - Performance optimization
   - Bug fixing
   - Balancing
   - Localization

## Testing Strategy

1. **Unit Testing**
   - Core systems functionality
   - Memory system logic
   - Combat calculations

2. **Integration Testing**
   - World transition system
   - Memory effects on gameplay
   - Save/load functionality

3. **Playtesting**
   - Difficulty balance
   - Narrative clarity
   - Control responsiveness
   - Overall game feel

## Conclusion

This architecture provides a solid foundation for developing Shatter Light Chronicles, with a focus on the memory fragment system that drives both gameplay and narrative. The modular design allows for flexibility during development while maintaining a clear structure that supports the unique dual-world concept and memory-based progression.
