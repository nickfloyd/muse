// PlayerCombat.cs - Handles player combat abilities and interactions
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCombat : MonoBehaviour
{
    // Combat settings
    [Header("Combat Settings")]
    [SerializeField] private float basicAttackDamage = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private LayerMask enemyLayers;
    
    // Memory light settings
    [Header("Memory Light Settings")]
    [SerializeField] private float maxMemoryEnergy = 100f;
    [SerializeField] private float memoryEnergyRegenRate = 5f;
    [SerializeField] private GameObject memoryLightEffectPrefab;
    
    // Visual effects
    [Header("Visual Effects")]
    [SerializeField] private GameObject basicAttackEffectPrefab;
    [SerializeField] private Transform attackPoint;
    
    // Audio
    [Header("Audio")]
    [SerializeField] private AudioClip[] attackSounds;
    [SerializeField] private AudioClip[] memoryAbilitySounds;
    private AudioSource audioSource;
    
    // Animation
    [SerializeField] private Animator animator;
    
    // Abilities
    private Dictionary<string, PlayerAbility> abilities = new Dictionary<string, PlayerAbility>();
    private List<string> activeAbilityIds = new List<string>();
    
    // Combat state
    private bool inCombatMode = false;
    private bool canAttack = true;
    private float currentMemoryEnergy;
    
    // Animation parameters
    private readonly int animAttack = Animator.StringToHash("Attack");
    private readonly int animSpecialAttack = Animator.StringToHash("SpecialAttack");
    private readonly int animInCombat = Animator.StringToHash("InCombat");
    
    private void Awake()
    {
        // Get components
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Initialize memory energy
        currentMemoryEnergy = maxMemoryEnergy / 2; // Start with half energy
    }
    
    private void Update()
    {
        // Check if player is disabled or not in combat mode
        if (GameManager.Instance.PlayerManager.CurrentPlayerState == PlayerState.Disabled || 
            !inCombatMode)
        {
            return;
        }
        
        // Basic attack input
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            BasicAttack();
        }
        
        // Special ability inputs (1-4 keys)
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            UseAbility(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            UseAbility(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            UseAbility(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            UseAbility(3);
        }
        
        // Regenerate memory energy
        if (currentMemoryEnergy < maxMemoryEnergy)
        {
            currentMemoryEnergy += memoryEnergyRegenRate * Time.deltaTime;
            currentMemoryEnergy = Mathf.Min(currentMemoryEnergy, maxMemoryEnergy);
            
            // Update UI
            UpdateEnergyUI();
        }
    }
    
    private void BasicAttack()
    {
        // Start attack cooldown
        StartCoroutine(AttackCooldown());
        
        // Play attack animation
        if (animator != null)
        {
            animator.SetTrigger(animAttack);
        }
        
        // Play attack sound
        if (audioSource != null && attackSounds.Length > 0)
        {
            audioSource.PlayOneShot(attackSounds[Random.Range(0, attackSounds.Length)]);
        }
        
        // Spawn attack effect
        if (basicAttackEffectPrefab != null && attackPoint != null)
        {
            Instantiate(basicAttackEffectPrefab, attackPoint.position, attackPoint.rotation);
        }
        
        // Detect enemies in range
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
        
        // Apply damage
        foreach (Collider enemy in hitEnemies)
        {
            // Try to get enemy component
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.TakeDamage(basicAttackDamage);
            }
            
            // Try to get memory guardian component
            MemoryGuardian guardianComponent = enemy.GetComponent<MemoryGuardian>();
            if (guardianComponent != null)
            {
                guardianComponent.TakeDamage(basicAttackDamage);
            }
        }
    }
    
    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
    
    private void UseAbility(int index)
    {
        // Check if ability exists at this index
        if (index < 0 || index >= activeAbilityIds.Count)
        {
            return;
        }
        
        string abilityId = activeAbilityIds[index];
        
        if (!abilities.ContainsKey(abilityId))
        {
            return;
        }
        
        PlayerAbility ability = abilities[abilityId];
        
        // Check if we have enough energy
        if (!ability.CanUse(currentMemoryEnergy))
        {
            // Show "not enough energy" feedback
            GameManager.Instance.UIManager.ShowNotEnoughEnergyFeedback();
            return;
        }
        
        // Get target position from mouse
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 targetPosition;
        
        if (Physics.Raycast(ray, out hit))
        {
            targetPosition = hit.point;
        }
        else
        {
            targetPosition = ray.GetPoint(20f); // Default distance
        }
        
        // Use ability
        ability.Use(GetComponent<PlayerCharacter>(), targetPosition);
        
        // Consume energy
        currentMemoryEnergy -= ability.MemoryEnergyCost;
        
        // Update UI
        UpdateEnergyUI();
        
        // Play special attack animation
        if (animator != null)
        {
            animator.SetTrigger(animSpecialAttack);
        }
        
        // Play ability sound
        if (audioSource != null && memoryAbilitySounds.Length > 0)
        {
            audioSource.PlayOneShot(memoryAbilitySounds[Random.Range(0, memoryAbilitySounds.Length)]);
        }
    }
    
    public void EnableCombatMode()
    {
        inCombatMode = true;
        
        // Update animation
        if (animator != null)
        {
            animator.SetBool(animInCombat, true);
        }
        
        // Show combat UI
        GameManager.Instance.UIManager.ShowCombatUI();
    }
    
    public void DisableCombatMode()
    {
        inCombatMode = false;
        
        // Update animation
        if (animator != null)
        {
            animator.SetBool(animInCombat, false);
        }
        
        // Hide combat UI
        GameManager.Instance.UIManager.HideCombatUI();
    }
    
    public void EnableDreamAbilities()
    {
        // Dream world has access to all unlocked abilities
        UpdateAbilities();
    }
    
    public void DisableDreamAbilities()
    {
        // Waking world has limited abilities
        List<string> wakingWorldAbilities = new List<string>();
        
        foreach (string abilityId in activeAbilityIds)
        {
            if (abilities.ContainsKey(abilityId) && abilities[abilityId].AvailableInWakingWorld)
            {
                wakingWorldAbilities.Add(abilityId);
            }
        }
        
        activeAbilityIds = wakingWorldAbilities;
        
        // Update UI
        GameManager.Instance.UIManager.UpdateAbilityUI(activeAbilityIds);
    }
    
    public void UpdateAbilities()
    {
        // Get all skill memories
        List<MemoryFragment> skillMemories = GameManager.Instance.MemoryManager.GetCollectedMemoriesByType(MemoryType.Skill);
        
        // Clear active abilities
        activeAbilityIds.Clear();
        
        // Add abilities from skill memories
        foreach (MemoryFragment memory in skillMemories)
        {
            if (memory is SkillMemoryFragment skillMemory)
            {
                PlayerAbility ability = skillMemory.UnlockedAbility;
                
                if (ability != null)
                {
                    // Add or update ability
                    abilities[ability.AbilityId] = ability;
                    
                    // Add to active list if in dream world or available in waking world
                    bool isInDreamWorld = GameManager.Instance.CurrentGameState == GameState.DreamWorld;
                    
                    if (isInDreamWorld || ability.AvailableInWakingWorld)
                    {
                        activeAbilityIds.Add(ability.AbilityId);
                    }
                }
            }
        }
        
        // Limit to 4 active abilities
        if (activeAbilityIds.Count > 4)
        {
            activeAbilityIds = activeAbilityIds.GetRange(0, 4);
        }
        
        // Update UI
        GameManager.Instance.UIManager.UpdateAbilityUI(activeAbilityIds);
    }
    
    private void UpdateEnergyUI()
    {
        // Update memory energy UI
        GameManager.Instance.UIManager.UpdateMemoryEnergyUI(currentMemoryEnergy, maxMemoryEnergy);
    }
    
    // Method to add energy (from pickups, etc.)
    public void AddMemoryEnergy(float amount)
    {
        currentMemoryEnergy += amount;
        currentMemoryEnergy = Mathf.Min(currentMemoryEnergy, maxMemoryEnergy);
        
        // Update UI
        UpdateEnergyUI();
    }
    
    // Method to get current energy level (percentage)
    public float GetEnergyPercentage()
    {
        return currentMemoryEnergy / maxMemoryEnergy;
    }
    
    // Debug visualization
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
            
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
