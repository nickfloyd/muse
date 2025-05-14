using UnityEngine;
using System.Collections;

public class WorldTransitionManager : MonoBehaviour
{
    // Transition settings
    [Header("Transition Settings")]
    [SerializeField] private float transitionDuration = 2.0f;
    [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    // Visual effects
    [Header("Visual Effects")]
    [SerializeField] private Material transitionMaterial;
    [SerializeField] private Color wakingWorldColor = new Color(0.8f, 0.9f, 1.0f);
    [SerializeField] private Color dreamWorldColor = new Color(0.5f, 0.3f, 0.8f);
    
    // Audio
    [Header("Audio")]
    [SerializeField] private AudioSource transitionAudioSource;
    [SerializeField] private AudioClip toWakingWorldSound;
    [SerializeField] private AudioClip toDreamWorldSound;
    
    // Post-processing
    [Header("Post-Processing")]
    [SerializeField] private MonoBehaviour wakingWorldPostProcessing;
    [SerializeField] private MonoBehaviour dreamWorldPostProcessing;
    
    // Private variables
    private Camera mainCamera;
    private GameObject transitionOverlay;
    private Material transitionMaterialInstance;
    private Coroutine activeTransition;
    
    private void Awake()
    {
        // Get main camera
        mainCamera = Camera.main;
        
        // Create transition overlay
        CreateTransitionOverlay();
        
        // Initialize post-processing
        if (wakingWorldPostProcessing != null)
        {
            wakingWorldPostProcessing.enabled = true;
        }
        
        if (dreamWorldPostProcessing != null)
        {
            dreamWorldPostProcessing.enabled = false;
        }
    }
    
    private void CreateTransitionOverlay()
    {
        // Create overlay game object
        transitionOverlay = new GameObject("TransitionOverlay");
        transitionOverlay.transform.parent = mainCamera.transform;
        
        // Add quad that fills the camera view
        MeshFilter meshFilter = transitionOverlay.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = transitionOverlay.AddComponent<MeshRenderer>();
        
        // Create quad mesh
        Mesh quadMesh = new Mesh();
        quadMesh.vertices = new Vector3[]
        {
            new Vector3(-1, -1, 0.1f),
            new Vector3(1, -1, 0.1f),
            new Vector3(-1, 1, 0.1f),
            new Vector3(1, 1, 0.1f)
        };
        quadMesh.uv = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        quadMesh.triangles = new int[] { 0, 2, 1, 2, 3, 1 };
        meshFilter.mesh = quadMesh;
        
        // Set material
        if (transitionMaterial != null)
        {
            transitionMaterialInstance = new Material(transitionMaterial);
            meshRenderer.material = transitionMaterialInstance;
            
            // Set initial properties
            transitionMaterialInstance.SetFloat("_TransitionProgress", 0);
            transitionMaterialInstance.SetColor("_WakingWorldColor", wakingWorldColor);
            transitionMaterialInstance.SetColor("_DreamWorldColor", dreamWorldColor);
        }
        
        // Initially hide overlay
        transitionOverlay.SetActive(false);
    }
    
    // Transition to waking world
    public void TransitionToWakingWorld()
    {
        if (activeTransition != null)
        {
            StopCoroutine(activeTransition);
        }
        
        activeTransition = StartCoroutine(PerformTransition(false));
    }
    
    // Transition to dream world
    public void TransitionToDreamWorld()
    {
        if (activeTransition != null)
        {
            StopCoroutine(activeTransition);
        }
        
        activeTransition = StartCoroutine(PerformTransition(true));
    }
    
    // Perform transition coroutine
    private IEnumerator PerformTransition(bool toDreamWorld)
    {
        // Notify game manager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CurrentGameState = GameState.Transition;
        }
        
        // Show overlay
        if (transitionOverlay != null)
        {
            transitionOverlay.SetActive(true);
        }
        
        // Play sound
        if (transitionAudioSource != null)
        {
            AudioClip clip = toDreamWorld ? toDreamWorldSound : toWakingWorldSound;
            if (clip != null)
            {
                transitionAudioSource.clip = clip;
                transitionAudioSource.Play();
            }
        }
        
        // Transition effect
        float startValue = toDreamWorld ? 0 : 1;
        float endValue = toDreamWorld ? 1 : 0;
        float elapsed = 0;
        
        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;
            float curvedT = transitionCurve.Evaluate(t);
            float currentValue = Mathf.Lerp(startValue, endValue, curvedT);
            
            // Update material
            if (transitionMaterialInstance != null)
            {
                transitionMaterialInstance.SetFloat("_TransitionProgress", currentValue);
            }
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Ensure final value
        if (transitionMaterialInstance != null)
        {
            transitionMaterialInstance.SetFloat("_TransitionProgress", endValue);
        }
        
        // Load appropriate scene
        if (GameManager.Instance != null && GameManager.Instance.WorldManager != null)
        {
            if (toDreamWorld)
            {
                GameManager.Instance.WorldManager.LoadDreamWorldScene();
            }
            else
            {
                GameManager.Instance.WorldManager.LoadWakingWorldScene();
            }
        }
        
        // Switch post-processing
        if (wakingWorldPostProcessing != null)
        {
            wakingWorldPostProcessing.enabled = !toDreamWorld;
        }
        
        if (dreamWorldPostProcessing != null)
        {
            dreamWorldPostProcessing.enabled = toDreamWorld;
        }
        
        // Wait a moment for scene to load
        yield return new WaitForSeconds(0.5f);
        
        // Fade out transition
        elapsed = 0;
        startValue = endValue;
        endValue = toDreamWorld ? 0.8f : 0.2f; // Partial fade to show world
        
        while (elapsed < transitionDuration / 2)
        {
            float t = elapsed / (transitionDuration / 2);
            float curvedT = transitionCurve.Evaluate(t);
            float currentValue = Mathf.Lerp(startValue, endValue, curvedT);
            
            // Update material
            if (transitionMaterialInstance != null)
            {
                transitionMaterialInstance.SetFloat("_TransitionProgress", currentValue);
            }
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Hide overlay
        if (transitionOverlay != null)
        {
            transitionOverlay.SetActive(false);
        }
        
        // Update game state
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CurrentGameState = toDreamWorld ? GameState.DreamWorld : GameState.WakingWorld;
        }
        
        // Update player state
        if (GameManager.Instance != null && GameManager.Instance.PlayerManager != null)
        {
            if (toDreamWorld)
            {
                GameManager.Instance.PlayerManager.EnterDreamState();
            }
            else
            {
                GameManager.Instance.PlayerManager.ExitDreamState();
            }
        }
        
        activeTransition = null;
    }
}
