using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class ProjectPackager : MonoBehaviour
{
    // Package settings
    [Header("Package Settings")]
    [SerializeField] private string projectName = "ShatterLightChronicles";
    [SerializeField] private string version = "0.1.0";
    [SerializeField] private bool includeSourceCode = true;
    [SerializeField] private bool includeDocumentation = true;
    
    // Build settings
    [Header("Build Settings")]
    [SerializeField] private bool buildForWindows = true;
    [SerializeField] private bool buildForMac = false;
    [SerializeField] private bool buildForLinux = false;
    [SerializeField] private bool buildForWebGL = true;
    
    // Documentation
    [Header("Documentation")]
    [SerializeField] private TextAsset readmeFile;
    [SerializeField] private TextAsset installationGuideFile;
    [SerializeField] private TextAsset controlsReferenceFile;
    
    // UI
    [Header("UI")]
    [SerializeField] private UnityEngine.UI.Text statusText;
    [SerializeField] private UnityEngine.UI.Button packageButton;
    [SerializeField] private UnityEngine.UI.Toggle sourceCodeToggle;
    [SerializeField] private UnityEngine.UI.Toggle documentationToggle;
    
    private void Start()
    {
        // Initialize UI
        if (sourceCodeToggle != null)
        {
            sourceCodeToggle.isOn = includeSourceCode;
            sourceCodeToggle.onValueChanged.AddListener((value) => includeSourceCode = value);
        }
        
        if (documentationToggle != null)
        {
            documentationToggle.isOn = includeDocumentation;
            documentationToggle.onValueChanged.AddListener((value) => includeDocumentation = value);
        }
        
        if (packageButton != null)
        {
            packageButton.onClick.AddListener(PackageProject);
        }
        
        UpdateStatus("Ready to package project");
    }
    
    public void PackageProject()
    {
        StartCoroutine(PackageProjectCoroutine());
    }
    
    private IEnumerator PackageProjectCoroutine()
    {
        // Disable button during packaging
        if (packageButton != null)
        {
            packageButton.interactable = false;
        }
        
        UpdateStatus("Starting packaging process...");
        yield return new WaitForSeconds(0.5f);
        
        // Create package directory
        string packageDir = Path.Combine(Application.dataPath, "..", "Package");
        if (!Directory.Exists(packageDir))
        {
            Directory.CreateDirectory(packageDir);
        }
        
        UpdateStatus("Creating package directory structure...");
        yield return new WaitForSeconds(0.5f);
        
        // Create subdirectories
        string buildDir = Path.Combine(packageDir, "Builds");
        string docsDir = Path.Combine(packageDir, "Documentation");
        string sourceDir = Path.Combine(packageDir, "Source");
        
        if (!Directory.Exists(buildDir))
        {
            Directory.CreateDirectory(buildDir);
        }
        
        if (!Directory.Exists(docsDir))
        {
            Directory.CreateDirectory(docsDir);
        }
        
        if (!Directory.Exists(sourceDir))
        {
            Directory.CreateDirectory(sourceDir);
        }
        
        // Package documentation
        if (includeDocumentation)
        {
            UpdateStatus("Packaging documentation...");
            yield return new WaitForSeconds(0.5f);
            
            // Create documentation files
            if (readmeFile != null)
            {
                File.WriteAllText(Path.Combine(packageDir, "README.md"), readmeFile.text);
            }
            
            if (installationGuideFile != null)
            {
                File.WriteAllText(Path.Combine(docsDir, "Installation_Guide.md"), installationGuideFile.text);
            }
            
            if (controlsReferenceFile != null)
            {
                File.WriteAllText(Path.Combine(docsDir, "Controls_Reference.md"), controlsReferenceFile.text);
            }
            
            // Create additional documentation
            CreateGameDesignDocument(docsDir);
            CreateCodeDocumentation(docsDir);
            CreateAssetList(docsDir);
        }
        
        // Package source code
        if (includeSourceCode)
        {
            UpdateStatus("Packaging source code...");
            yield return new WaitForSeconds(0.5f);
            
            // Copy scripts
            CopyDirectory(Path.Combine(Application.dataPath, "Scripts"), Path.Combine(sourceDir, "Scripts"));
            
            // Create project files
            CreateProjectFiles(sourceDir);
        }
        
        // Build for platforms
        if (buildForWindows)
        {
            UpdateStatus("Building for Windows...");
            yield return new WaitForSeconds(0.5f);
            
            // In a real implementation, this would call BuildPipeline.BuildPlayer
            // For this demo, we'll just create a placeholder file
            File.WriteAllText(Path.Combine(buildDir, $"{projectName}_Windows.txt"), 
                "This is a placeholder for the Windows build.\n" +
                "In a real implementation, this would be the executable build.");
        }
        
        if (buildForMac)
        {
            UpdateStatus("Building for Mac...");
            yield return new WaitForSeconds(0.5f);
            
            // Placeholder
            File.WriteAllText(Path.Combine(buildDir, $"{projectName}_Mac.txt"), 
                "This is a placeholder for the Mac build.\n" +
                "In a real implementation, this would be the app bundle.");
        }
        
        if (buildForLinux)
        {
            UpdateStatus("Building for Linux...");
            yield return new WaitForSeconds(0.5f);
            
            // Placeholder
            File.WriteAllText(Path.Combine(buildDir, $"{projectName}_Linux.txt"), 
                "This is a placeholder for the Linux build.\n" +
                "In a real implementation, this would be the executable build.");
        }
        
        if (buildForWebGL)
        {
            UpdateStatus("Building for WebGL...");
            yield return new WaitForSeconds(0.5f);
            
            // Placeholder
            File.WriteAllText(Path.Combine(buildDir, $"{projectName}_WebGL.txt"), 
                "This is a placeholder for the WebGL build.\n" +
                "In a real implementation, this would be the WebGL build files.");
        }
        
        // Create zip archive
        UpdateStatus("Creating zip archive...");
        yield return new WaitForSeconds(0.5f);
        
        // In a real implementation, this would create a zip file
        // For this demo, we'll just create a placeholder file
        File.WriteAllText(Path.Combine(Application.dataPath, "..", $"{projectName}_v{version}.zip.txt"), 
            "This is a placeholder for the zip archive.\n" +
            "In a real implementation, this would be the zipped package.");
        
        // Complete
        UpdateStatus("Packaging complete!");
        
        // Re-enable button
        if (packageButton != null)
        {
            packageButton.interactable = true;
        }
    }
    
    private void UpdateStatus(string status)
    {
        Debug.Log(status);
        
        if (statusText != null)
        {
            statusText.text = status;
        }
    }
    
    private void CopyDirectory(string sourceDir, string targetDir)
    {
        // Create the target directory if it doesn't exist
        if (!Directory.Exists(targetDir))
        {
            Directory.CreateDirectory(targetDir);
        }
        
        // Copy files
        foreach (string file in Directory.GetFiles(sourceDir))
        {
            string fileName = Path.GetFileName(file);
            string targetFile = Path.Combine(targetDir, fileName);
            File.Copy(file, targetFile, true);
        }
        
        // Copy subdirectories
        foreach (string directory in Directory.GetDirectories(sourceDir))
        {
            string dirName = Path.GetFileName(directory);
            string targetSubDir = Path.Combine(targetDir, dirName);
            CopyDirectory(directory, targetSubDir);
        }
    }
    
    private void CreateGameDesignDocument(string docsDir)
    {
        string content = 
            "# Shatter Light Chronicles - Game Design Document\n\n" +
            
            "## Game Overview\n\n" +
            "Shatter Light Chronicles is a narrative-driven action-adventure game about a character who has lost his memory " +
            "and fights to recover fragments while asleep. The game features a dual-world gameplay mechanic, where the player " +
            "navigates between the waking world and dream world, collecting memory fragments to piece together their past.\n\n" +
            
            "## Core Gameplay Loop\n\n" +
            "1. Explore the waking world to find clues and interact with NPCs\n" +
            "2. Find sleep points to transition to the dream world\n" +
            "3. Navigate the dream world, fighting enemies and collecting memory fragments\n" +
            "4. Return to the waking world with new memories that unlock abilities and story progression\n" +
            "5. Repeat with new areas and challenges unlocked by recovered memories\n\n" +
            
            "## Memory Fragment System\n\n" +
            "Memory fragments come in three types:\n" +
            "- **Core Memories**: Critical to story progression, unlock major abilities and world changes\n" +
            "- **Skill Memories**: Unlock new combat abilities and techniques\n" +
            "- **Peripheral Memories**: Provide context, lore, and minor gameplay benefits\n\n" +
            
            "## Dual-World Mechanics\n\n" +
            "- **Waking World**: Realistic, exploration-focused, NPC interactions, limited abilities\n" +
            "- **Dream World**: Surreal, combat-focused, memory collection, enhanced abilities\n" +
            "- **Transition**: Sleep points in waking world, memory portals in dream world\n\n" +
            
            "## Character Progression\n\n" +
            "As players collect memory fragments, they unlock:\n" +
            "- New abilities and combat techniques\n" +
            "- Increased stats and power\n" +
            "- Story progression and world changes\n" +
            "- New areas to explore\n\n" +
            
            "## Art Style\n\n" +
            "- Waking World: Realistic with slightly muted colors\n" +
            "- Dream World: Surreal with vibrant colors and distorted geometry\n" +
            "- Memory Fragments: Glowing, ethereal objects with distinct colors based on type\n\n" +
            
            "## Audio Design\n\n" +
            "- Waking World: Ambient, subtle music with realistic sound effects\n" +
            "- Dream World: Dynamic, otherworldly music with distorted sound effects\n" +
            "- Memory Collection: Distinctive sound cues for different memory types\n\n" +
            
            "## Target Platforms\n\n" +
            "- PC (Windows, Mac, Linux)\n" +
            "- WebGL for browser play\n" +
            "- Potential console ports in the future\n";
        
        File.WriteAllText(Path.Combine(docsDir, "Game_Design_Document.md"), content);
    }
    
    private void CreateCodeDocumentation(string docsDir)
    {
        string content = 
            "# Shatter Light Chronicles - Code Documentation\n\n" +
            
            "## Core Systems\n\n" +
            
            "### GameManager\n" +
            "Central controller for the game that manages state transitions and coordinates other systems.\n" +
            "- Manages game state (main menu, waking world, dream world, paused)\n" +
            "- Coordinates other manager classes\n" +
            "- Handles scene transitions\n\n" +
            
            "### PlayerManager\n" +
            "Manages the player character and its states between worlds.\n" +
            "- Controls player state (normal, disabled, in combat)\n" +
            "- Manages player stats and abilities\n" +
            "- Handles transitions between waking and dream states\n\n" +
            
            "### MemoryManager\n" +
            "Handles memory fragment collection, storage, and effects.\n" +
            "- Tracks collected memory fragments\n" +
            "- Manages memory journal visualization\n" +
            "- Applies memory effects to gameplay\n\n" +
            
            "### WorldManager\n" +
            "Manages world state and environment changes.\n" +
            "- Controls world transitions\n" +
            "- Handles environment changes based on memories\n" +
            "- Manages NPCs and enemies\n\n" +
            
            "### UIManager\n" +
            "Manages all user interface elements.\n" +
            "- Controls HUD elements\n" +
            "- Manages menus and panels\n" +
            "- Handles notifications and prompts\n\n" +
            
            "## Player Components\n\n" +
            
            "### PlayerMovement\n" +
            "Controls player character movement and navigation.\n" +
            "- Handles walking, running, and jumping\n" +
            "- Manages ground detection and gravity\n" +
            "- Controls animation states\n\n" +
            
            "### PlayerCombat\n" +
            "Implements the combat system and memory abilities.\n" +
            "- Handles basic attacks\n" +
            "- Manages abilities and cooldowns\n" +
            "- Controls memory energy\n\n" +
            
            "### PlayerMemory\n" +
            "Handles memory detection, interaction, and resonance ability.\n" +
            "- Detects nearby memory fragments\n" +
            "- Manages memory resonance ability\n" +
            "- Handles memory interaction\n\n" +
            
            "### PlayerStats\n" +
            "Manages player statistics, health, and memory boosts.\n" +
            "- Tracks health and stats\n" +
            "- Applies memory boosts\n" +
            "- Handles damage and healing\n\n" +
            
            "## Memory System\n\n" +
            
            "### MemoryFragment\n" +
            "Base class for all memory fragments.\n" +
            "- Defines common properties and methods\n" +
            "- Handles collection and effects\n\n" +
            
            "### CoreMemoryFragment\n" +
            "Critical memories that drive story progression.\n" +
            "- Triggers story events\n" +
            "- Unlocks major abilities\n" +
            "- Changes world state\n\n" +
            
            "### SkillMemoryFragment\n" +
            "Memories that unlock combat abilities.\n" +
            "- Provides new abilities\n" +
            "- Can be upgraded\n" +
            "- Enhances combat options\n\n" +
            
            "### PeripheralMemoryFragment\n" +
            "Contextual memories that provide lore and minor benefits.\n" +
            "- Creates connections between memories\n" +
            "- Provides narrative context\n" +
            "- Offers minor gameplay benefits\n\n" +
            
            "### MemoryTrigger\n" +
            "In-game representation of memory fragments.\n" +
            "- Handles player interaction\n" +
            "- Manages visual effects\n" +
            "- Triggers collection events\n\n" +
            
            "### MemoryJournal\n" +
            "Visualization system for collected memories.\n" +
            "- Displays memory connections\n" +
            "- Shows memory details\n" +
            "- Tracks collection progress\n\n" +
            
            "## World System\n\n" +
            
            "### WorldTransitionManager\n
(Content truncated due to size limit. Use line ranges to read in chunks)