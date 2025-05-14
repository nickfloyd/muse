using UnityEngine;
using System.Collections;

public class SampleLevelGenerator : MonoBehaviour
{
    // Level settings
    [Header("Level Settings")]
    [SerializeField] private int wakingWorldSeed = 12345;
    [SerializeField] private int dreamWorldSeed = 54321;
    [SerializeField] private Vector3 playerStartPosition = new Vector3(0, 1, 0);
    
    // Environment prefabs
    [Header("Environment Prefabs")]
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private GameObject[] wakingWorldBuildingPrefabs;
    [SerializeField] private GameObject[] dreamWorldStructurePrefabs;
    [SerializeField] private GameObject[] treePrefabs;
    [SerializeField] private GameObject[] rockPrefabs;
    
    // Memory fragment prefabs
    [Header("Memory Fragment Prefabs")]
    [SerializeField] private GameObject memoryFragmentPrefab;
    [SerializeField] private GameObject hiddenMemoryFragmentPrefab;
    [SerializeField] private GameObject[] memoryEffectPrefabs;
    
    // NPC prefabs
    [Header("NPC Prefabs")]
    [SerializeField] private GameObject[] wakingWorldNPCPrefabs;
    [SerializeField] private GameObject[] dreamWorldEnemyPrefabs;
    
    // World transition
    [Header("World Transition")]
    [SerializeField] private GameObject sleepPointPrefab;
    [SerializeField] private GameObject dreamWorldExitPrefab;
    
    // Level dimensions
    [Header("Level Dimensions")]
    [SerializeField] private float levelWidth = 100f;
    [SerializeField] private float levelLength = 100f;
    
    // Level containers
    private GameObject wakingWorldContainer;
    private GameObject dreamWorldContainer;
    
    private void Awake()
    {
        // Create containers
        wakingWorldContainer = new GameObject("WakingWorld");
        dreamWorldContainer = new GameObject("DreamWorld");
        
        // Set initial active state
        wakingWorldContainer.SetActive(true);
        dreamWorldContainer.SetActive(false);
    }
    
    private void Start()
    {
        // Generate level
        GenerateWakingWorld();
        GenerateDreamWorld();
        
        // Place player
        PlacePlayer();
        
        // Register with game manager
        if (GameManager.Instance != null && GameManager.Instance.WorldManager != null)
        {
            GameManager.Instance.WorldManager.RegisterWorldContainers(wakingWorldContainer, dreamWorldContainer);
        }
    }
    
    // Generate waking world
    private void GenerateWakingWorld()
    {
        // Set random seed
        Random.InitState(wakingWorldSeed);
        
        // Create ground
        CreateGround(wakingWorldContainer, new Color(0.3f, 0.5f, 0.2f));
        
        // Create buildings
        int buildingCount = Random.Range(10, 15);
        for (int i = 0; i < buildingCount; i++)
        {
            PlaceRandomBuilding(wakingWorldContainer);
        }
        
        // Create trees
        int treeCount = Random.Range(30, 50);
        for (int i = 0; i < treeCount; i++)
        {
            PlaceRandomTree(wakingWorldContainer);
        }
        
        // Create rocks
        int rockCount = Random.Range(20, 30);
        for (int i = 0; i < rockCount; i++)
        {
            PlaceRandomRock(wakingWorldContainer);
        }
        
        // Create NPCs
        int npcCount = Random.Range(5, 10);
        for (int i = 0; i < npcCount; i++)
        {
            PlaceRandomNPC(wakingWorldContainer);
        }
        
        // Create memory fragments
        int memoryCount = Random.Range(3, 5);
        for (int i = 0; i < memoryCount; i++)
        {
            PlaceRandomMemoryFragment(wakingWorldContainer, false);
        }
        
        // Create sleep points
        int sleepPointCount = 3;
        for (int i = 0; i < sleepPointCount; i++)
        {
            PlaceSleepPoint(wakingWorldContainer, i);
        }
    }
    
    // Generate dream world
    private void GenerateDreamWorld()
    {
        // Set random seed
        Random.InitState(dreamWorldSeed);
        
        // Create ground
        CreateGround(dreamWorldContainer, new Color(0.2f, 0.2f, 0.4f));
        
        // Create dream structures
        int structureCount = Random.Range(8, 12);
        for (int i = 0; i < structureCount; i++)
        {
            PlaceRandomDreamStructure(dreamWorldContainer);
        }
        
        // Create distorted trees
        int treeCount = Random.Range(15, 25);
        for (int i = 0; i < treeCount; i++)
        {
            PlaceRandomTree(dreamWorldContainer, true);
        }
        
        // Create floating rocks
        int rockCount = Random.Range(10, 20);
        for (int i = 0; i < rockCount; i++)
        {
            PlaceRandomRock(dreamWorldContainer, true);
        }
        
        // Create enemies
        int enemyCount = Random.Range(8, 15);
        for (int i = 0; i < enemyCount; i++)
        {
            PlaceRandomEnemy(dreamWorldContainer);
        }
        
        // Create memory fragments
        int memoryCount = Random.Range(5, 8);
        for (int i = 0; i < memoryCount; i++)
        {
            PlaceRandomMemoryFragment(dreamWorldContainer, true);
        }
        
        // Create hidden memory fragments
        int hiddenMemoryCount = Random.Range(2, 4);
        for (int i = 0; i < hiddenMemoryCount; i++)
        {
            PlaceRandomHiddenMemoryFragment(dreamWorldContainer);
        }
        
        // Create dream world exits
        int exitCount = 3;
        for (int i = 0; i < exitCount; i++)
        {
            PlaceDreamWorldExit(dreamWorldContainer, i);
        }
    }
    
    // Create ground
    private void CreateGround(GameObject parent, Color groundColor)
    {
        if (groundPrefab == null)
        {
            Debug.LogWarning("Ground prefab not assigned");
            return;
        }
        
        GameObject ground = Instantiate(groundPrefab, Vector3.zero, Quaternion.identity, parent.transform);
        ground.transform.localScale = new Vector3(levelWidth / 10f, 1f, levelLength / 10f);
        
        // Set ground color
        Renderer renderer = ground.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = groundColor;
        }
    }
    
    // Place random building
    private void PlaceRandomBuilding(GameObject parent)
    {
        if (wakingWorldBuildingPrefabs == null || wakingWorldBuildingPrefabs.Length == 0)
        {
            Debug.LogWarning("No building prefabs assigned");
            return;
        }
        
        // Select random building prefab
        GameObject prefab = wakingWorldBuildingPrefabs[Random.Range(0, wakingWorldBuildingPrefabs.Length)];
        
        // Random position
        Vector3 position = GetRandomPosition();
        
        // Random rotation
        Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        
        // Create building
        GameObject building = Instantiate(prefab, position, rotation, parent.transform);
        
        // Random scale
        float scale = Random.Range(0.8f, 1.2f);
        building.transform.localScale = new Vector3(scale, scale, scale);
    }
    
    // Place random dream structure
    private void PlaceRandomDreamStructure(GameObject parent)
    {
        if (dreamWorldStructurePrefabs == null || dreamWorldStructurePrefabs.Length == 0)
        {
            Debug.LogWarning("No dream structure prefabs assigned");
            return;
        }
        
        // Select random structure prefab
        GameObject prefab = dreamWorldStructurePrefabs[Random.Range(0, dreamWorldStructurePrefabs.Length)];
        
        // Random position
        Vector3 position = GetRandomPosition();
        
        // Add some height for floating structures
        if (Random.value > 0.5f)
        {
            position.y += Random.Range(1f, 5f);
        }
        
        // Random rotation
        Quaternion rotation = Quaternion.Euler(
            Random.Range(-10f, 10f),
            Random.Range(0, 360),
            Random.Range(-10f, 10f)
        );
        
        // Create structure
        GameObject structure = Instantiate(prefab, position, rotation, parent.transform);
        
        // Random scale
        float scale = Random.Range(0.7f, 1.5f);
        structure.transform.localScale = new Vector3(scale, scale, scale);
    }
    
    // Place random tree
    private void PlaceRandomTree(GameObject parent, bool distorted = false)
    {
        if (treePrefabs == null || treePrefabs.Length == 0)
        {
            Debug.LogWarning("No tree prefabs assigned");
            return;
        }
        
        // Select random tree prefab
        GameObject prefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
        
        // Random position
        Vector3 position = GetRandomPosition();
        
        // Random rotation
        Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        
        // Create tree
        GameObject tree = Instantiate(prefab, position, rotation, parent.transform);
        
        // Random scale
        float scaleX = Random.Range(0.8f, 1.2f);
        float scaleY = Random.Range(0.8f, 1.2f);
        float scaleZ = Random.Range(0.8f, 1.2f);
        
        // Apply distortion in dream world
        if (distorted)
        {
            scaleY *= Random.Range(1.2f, 1.8f); // Taller trees
            rotation = Quaternion.Euler(
                Random.Range(-5f, 5f),
                Random.Range(0, 360),
                Random.Range(-5f, 5f)
            );
            
            // Change tree color
            Renderer[] renderers = tree.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                if (renderer.material.HasProperty("_Color"))
                {
                    Color originalColor = renderer.material.color;
                    renderer.material.color = new Color(
                        originalColor.r * 0.7f,
                        originalColor.g * 0.8f,
                        originalColor.b * 1.2f,
                        originalColor.a
                    );
                }
            }
        }
        
        tree.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
    }
    
    // Place random rock
    private void PlaceRandomRock(GameObject parent, bool floating = false)
    {
        if (rockPrefabs == null || rockPrefabs.Length == 0)
        {
            Debug.LogWarning("No rock prefabs assigned");
            return;
        }
        
        // Select random rock prefab
        GameObject prefab = rockPrefabs[Random.Range(0, rockPrefabs.Length)];
        
        // Random position
        Vector3 position = GetRandomPosition();
        
        // Add height for floating rocks
        if (floating)
        {
            position.y += Random.Range(0.5f, 3f);
        }
        
        // Random rotation
        Quaternion rotation = Quaternion.Euler(
            Random.Range(0, 360),
            Random.Range(0, 360),
            Random.Range(0, 360)
        );
        
        // Create rock
        GameObject rock = Instantiate(prefab, position, rotation, parent.transform);
        
        // Random scale
        float scale = Random.Range(0.5f, 2f);
        rock.transform.localScale = new Vector3(scale, scale, scale);
    }
    
    // Place random NPC
    private void PlaceRandomNPC(GameObject parent)
    {
        if (wakingWorldNPCPrefabs == null || wakingWorldNPCPrefabs.Length == 0)
        {
            Debug.LogWarning("No NPC prefabs assigned");
            return;
        }
        
        // Select random NPC prefab
        GameObject prefab = wakingWorldNPCPrefabs[Random.Range(0, wakingWorldNPCPrefabs.Length)];
        
        // Random position
        Vector3 position = GetRandomPosition();
        position.y = 0; // Ensure on ground
        
        // Random rotation
        Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        
        // Create NPC
        GameObject npc = Instantiate(prefab, position, rotation, parent.transform);
    }
    
    // Place random enemy
    private void PlaceRandomEnemy(GameObject parent)
    {
        if (dreamWorldEnemyPrefabs == null || dreamWorldEnemyPrefabs.Length == 0)
        {
            Debug.LogWarning("No enemy prefabs assigned");
            return;
        }
        
        // Select random enemy prefab
        GameObject prefab = dreamWorldEnemyPrefabs[Random.Range(0, dreamWorldEnemyPrefabs.Length)];
        
        // Random position
        Vector3 position = GetRandomPosition();
        position.y = 0; // Ensure on ground
        
        // Random rotation
        Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        
        // Create enemy
        GameObject enemy = Instantiate(prefab, position, rotation, parent.transform);
    }
    
    // Place random memory fragment
    private void PlaceRandomMemoryFragment(GameObject parent, bool isDreamWorld)
    {
        if (memoryFragmentPrefab == null)
        {
            Debug.LogWarning("Memory fragment prefab not assigned");
            return;
        }
        
        // Random position
        Vector3 position = GetRandomPosition();
        position.y = 1f; // Hover above ground
        
        // Create memory fragment
        GameObject memoryFragment = Instantiate(memoryFragmentPrefab, position, Quaternion.identity, parent.transform);
        
        // Configure memory trigger
        MemoryTrigger trigger = memoryFragment.GetComponent<MemoryTrigger>();
        if (trigger != null)
        {
            // Assign random memory ID based on world
            string memoryId;
            MemoryType memoryType;
            
            if (isDreamWorld)
            {
                // Dream world has more skill and core memories
                float rand = Random.value;
                if (rand < 0.4f)
                {
                    memoryType = MemoryType.Core;
                    memoryId = "core_memory_" + Random.Range(1, 6);
                }
                else if (rand < 0.8f)
                {
                    memoryType = MemoryType.Skill;
                    memoryId = "skill_memory_" + Random.Range(1, 8);
                }
                else
                {
                    memoryType = MemoryType.Peripheral;
                    memoryId = "peripheral_memory_" + Random.Range(1, 10);
                }
            }
            else
            {
                // Waking world has more peripheral memories
                float rand = Random.value;
                if (rand < 0.2f)
                {
                    memoryType = MemoryType.Core;
                    memoryId = "core_memory_" + Random.Range(1, 3);
                }
                else if (rand < 0.4f)
                {
                    memoryType = MemoryType.Skill;
                    memoryId = "skill_memory_" + Random.Range(1, 4);
                }
                else
                {
                    memoryType = MemoryType.Peripheral;
                    memoryId = "peripheral_memory_" + Random.Range(1, 15);
                }
            }
            
            // Set memory properties
            // In a real implementation, these would be set from ScriptableObject data
            // For this sample, we're just setting them directly
            
            // Add memory effect
            if (memoryEffectPrefabs != null && memoryEffectPrefabs.Length > 0)
            {
                GameObject effectPrefab = memoryEffectPrefabs[Random.Range(0, memoryEffectPrefabs.Length)];
                GameObject effect = Instantiate(effectPrefab, memoryFragment.transform);
                
                // Set effect color based on memory type
                ParticleSystem particles = effect.GetComponent<Particle
(Content truncated due to size limit. Use line ranges to read in chunks)