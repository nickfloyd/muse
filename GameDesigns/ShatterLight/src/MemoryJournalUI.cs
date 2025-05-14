using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MemoryJournalUI : MonoBehaviour
{
    // UI Elements
    [Header("UI Elements")]
    [SerializeField] private RectTransform journalContainer;
    [SerializeField] private GameObject memoryDetailPanel;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button categoriesButton;
    [SerializeField] private Button connectionsButton;
    [SerializeField] private Button timelineButton;
    
    // Memory Node Prefabs
    [Header("Memory Node Prefabs")]
    [SerializeField] private GameObject coreMemoryNodePrefab;
    [SerializeField] private GameObject skillMemoryNodePrefab;
    [SerializeField] private GameObject peripheralMemoryNodePrefab;
    [SerializeField] private GameObject connectionPrefab;
    
    // Memory Detail Elements
    [Header("Memory Detail Elements")]
    [SerializeField] private Text memoryTitleText;
    [SerializeField] private Text memoryDescriptionText;
    [SerializeField] private Image memoryIconImage;
    [SerializeField] private Text memoryTypeText;
    [SerializeField] private GameObject connectedMemoriesContainer;
    [SerializeField] private GameObject connectedMemoryPrefab;
    
    // View Controls
    [Header("View Controls")]
    [SerializeField] private Slider zoomSlider;
    [SerializeField] private Button resetViewButton;
    
    // Animation
    [Header("Animation")]
    [SerializeField] private Animator journalAnimator;
    
    // Audio
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pageFlipSound;
    [SerializeField] private AudioClip memorySelectSound;
    
    // Private variables
    private Vector2 panOffset = Vector2.zero;
    private float currentZoom = 1.0f;
    private bool isDragging = false;
    private Vector2 lastMousePosition;
    private Dictionary<string, GameObject> memoryNodeObjects = new Dictionary<string, GameObject>();
    private string selectedMemoryId = null;
    private JournalViewMode currentViewMode = JournalViewMode.Categories;
    
    private enum JournalViewMode
    {
        Categories,
        Connections,
        Timeline
    }
    
    private void Start()
    {
        // Set up button listeners
        SetupButtonListeners();
        
        // Hide memory detail panel initially
        if (memoryDetailPanel != null)
        {
            memoryDetailPanel.SetActive(false);
        }
        
        // Set initial view mode
        SetViewMode(JournalViewMode.Categories);
    }
    
    private void Update()
    {
        // Handle pan and zoom input
        HandlePanInput();
        HandleZoomInput();
    }
    
    private void SetupButtonListeners()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }
        
        if (categoriesButton != null)
        {
            categoriesButton.onClick.AddListener(() => SetViewMode(JournalViewMode.Categories));
        }
        
        if (connectionsButton != null)
        {
            connectionsButton.onClick.AddListener(() => SetViewMode(JournalViewMode.Connections));
        }
        
        if (timelineButton != null)
        {
            timelineButton.onClick.AddListener(() => SetViewMode(JournalViewMode.Timeline));
        }
        
        if (resetViewButton != null)
        {
            resetViewButton.onClick.AddListener(ResetView);
        }
        
        if (zoomSlider != null)
        {
            zoomSlider.onValueChanged.AddListener(OnZoomSliderChanged);
        }
    }
    
    // Handle pan input
    private void HandlePanInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Check if we're clicking on a memory node
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject.CompareTag("MemoryNode"))
            {
                // Select memory node
                MemoryNodeUI nodeUI = hit.collider.gameObject.GetComponent<MemoryNodeUI>();
                if (nodeUI != null)
                {
                    SelectMemory(nodeUI.MemoryId);
                }
            }
            else
            {
                // Start dragging
                isDragging = true;
                lastMousePosition = Input.mousePosition;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
        
        if (isDragging)
        {
            Vector2 delta = (Vector2)Input.mousePosition - lastMousePosition;
            panOffset += delta / currentZoom;
            lastMousePosition = Input.mousePosition;
            
            // Apply pan offset to journal container
            if (journalContainer != null)
            {
                journalContainer.anchoredPosition = panOffset;
            }
        }
    }
    
    // Handle zoom input
    private void HandleZoomInput()
    {
        float scrollDelta = Input.mouseScrollDelta.y;
        if (scrollDelta != 0)
        {
            float zoomDelta = scrollDelta * 0.1f;
            float newZoom = Mathf.Clamp(currentZoom + zoomDelta, 0.5f, 2.0f);
            
            // Apply zoom
            SetZoom(newZoom);
            
            // Update zoom slider
            if (zoomSlider != null)
            {
                zoomSlider.value = currentZoom;
            }
        }
    }
    
    // Set zoom level
    private void SetZoom(float zoom)
    {
        currentZoom = zoom;
        
        if (journalContainer != null)
        {
            journalContainer.localScale = new Vector3(zoom, zoom, 1);
        }
    }
    
    // On zoom slider changed
    private void OnZoomSliderChanged(float value)
    {
        SetZoom(value);
    }
    
    // Reset view
    private void ResetView()
    {
        panOffset = Vector2.zero;
        currentZoom = 1.0f;
        
        if (journalContainer != null)
        {
            journalContainer.anchoredPosition = panOffset;
            journalContainer.localScale = Vector3.one;
        }
        
        if (zoomSlider != null)
        {
            zoomSlider.value = 1.0f;
        }
    }
    
    // Set view mode
    private void SetViewMode(JournalViewMode mode)
    {
        currentViewMode = mode;
        
        // Update button states
        if (categoriesButton != null)
        {
            categoriesButton.interactable = mode != JournalViewMode.Categories;
        }
        
        if (connectionsButton != null)
        {
            connectionsButton.interactable = mode != JournalViewMode.Connections;
        }
        
        if (timelineButton != null)
        {
            timelineButton.interactable = mode != JournalViewMode.Timeline;
        }
        
        // Play page flip sound
        if (audioSource != null && pageFlipSound != null)
        {
            audioSource.PlayOneShot(pageFlipSound);
        }
        
        // Reset view
        ResetView();
        
        // Update visualization
        UpdateJournalVisualization();
    }
    
    // Update journal visualization
    public void UpdateJournalVisualization()
    {
        // Clear existing visualization
        ClearJournalVisualization();
        
        // Get memory manager
        if (GameManager.Instance == null || GameManager.Instance.MemoryManager == null)
        {
            Debug.LogWarning("Cannot visualize journal: Memory Manager not found");
            return;
        }
        
        // Get collected memories
        List<MemoryFragment> memories = GameManager.Instance.MemoryManager.GetCollectedMemories();
        
        if (memories.Count == 0)
        {
            // No memories collected yet
            return;
        }
        
        // Create visualization based on current view mode
        switch (currentViewMode)
        {
            case JournalViewMode.Categories:
                VisualizeByCategories(memories);
                break;
                
            case JournalViewMode.Connections:
                VisualizeByConnections(memories);
                break;
                
            case JournalViewMode.Timeline:
                VisualizeByTimeline(memories);
                break;
        }
    }
    
    // Clear journal visualization
    private void ClearJournalVisualization()
    {
        if (journalContainer == null)
            return;
            
        // Clear all children
        foreach (Transform child in journalContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Clear node dictionary
        memoryNodeObjects.Clear();
    }
    
    // Visualize memories by categories
    private void VisualizeByCategories(List<MemoryFragment> memories)
    {
        // Group memories by type
        Dictionary<MemoryType, List<MemoryFragment>> groupedMemories = new Dictionary<MemoryType, List<MemoryFragment>>();
        
        foreach (MemoryFragment memory in memories)
        {
            if (!groupedMemories.ContainsKey(memory.Type))
            {
                groupedMemories[memory.Type] = new List<MemoryFragment>();
            }
            
            groupedMemories[memory.Type].Add(memory);
        }
        
        // Create category headers and nodes
        float yOffset = 200f;
        
        // Core memories at top
        if (groupedMemories.ContainsKey(MemoryType.Core))
        {
            CreateCategoryHeader("Core Memories", new Vector2(0, yOffset));
            CreateMemoryNodes(groupedMemories[MemoryType.Core], new Vector2(0, yOffset - 100), 300f);
            yOffset -= 300f;
        }
        
        // Skill memories in middle
        if (groupedMemories.ContainsKey(MemoryType.Skill))
        {
            CreateCategoryHeader("Skill Memories", new Vector2(0, yOffset));
            CreateMemoryNodes(groupedMemories[MemoryType.Skill], new Vector2(0, yOffset - 100), 300f);
            yOffset -= 300f;
        }
        
        // Peripheral memories at bottom
        if (groupedMemories.ContainsKey(MemoryType.Peripheral))
        {
            CreateCategoryHeader("Peripheral Memories", new Vector2(0, yOffset));
            CreateMemoryNodes(groupedMemories[MemoryType.Peripheral], new Vector2(0, yOffset - 100), 300f);
        }
    }
    
    // Visualize memories by connections
    private void VisualizeByConnections(List<MemoryFragment> memories)
    {
        // Create nodes for all memories
        foreach (MemoryFragment memory in memories)
        {
            // Calculate position based on memory type
            Vector2 position = CalculateNodePosition(memory);
            
            // Create node
            GameObject nodeObj = CreateMemoryNode(memory, position);
            memoryNodeObjects[memory.Id] = nodeObj;
        }
        
        // Create connections
        foreach (MemoryFragment memory in memories)
        {
            if (memory is PeripheralMemoryFragment peripheralMemory)
            {
                foreach (string connectedId in peripheralMemory.ConnectedMemoryIds)
                {
                    // Check if connected memory exists and is collected
                    MemoryFragment connectedMemory = GameManager.Instance.MemoryManager.GetMemoryById(connectedId);
                    if (connectedMemory != null && connectedMemory.IsCollected)
                    {
                        CreateConnection(memory.Id, connectedId);
                    }
                }
            }
        }
    }
    
    // Visualize memories by timeline
    private void VisualizeByTimeline(List<MemoryFragment> memories)
    {
        // Sort memories by collection time
        List<MemoryFragment> sortedMemories = new List<MemoryFragment>(memories);
        sortedMemories.Sort((a, b) => a.CollectionTime.CompareTo(b.CollectionTime));
        
        // Create timeline
        float xStart = -400f;
        float xEnd = 400f;
        float yPos = 0f;
        
        // Create timeline line
        GameObject timelineObj = new GameObject("Timeline");
        timelineObj.transform.SetParent(journalContainer, false);
        
        Image timelineImage = timelineObj.AddComponent<Image>();
        timelineImage.color = new Color(0.8f, 0.8f, 0.8f, 0.5f);
        
        RectTransform timelineRect = timelineObj.GetComponent<RectTransform>();
        timelineRect.anchoredPosition = new Vector2((xStart + xEnd) / 2, yPos);
        timelineRect.sizeDelta = new Vector2(xEnd - xStart, 5f);
        
        // Create nodes along timeline
        if (sortedMemories.Count > 0)
        {
            float timeRange = sortedMemories[sortedMemories.Count - 1].CollectionTime - sortedMemories[0].CollectionTime;
            if (timeRange <= 0) timeRange = 1; // Avoid division by zero
            
            foreach (MemoryFragment memory in sortedMemories)
            {
                // Calculate position based on collection time
                float normalizedTime = (memory.CollectionTime - sortedMemories[0].CollectionTime) / timeRange;
                float xPos = Mathf.Lerp(xStart, xEnd, normalizedTime);
                
                // Offset Y position based on memory type to avoid overlap
                float yOffset = 0f;
                switch (memory.Type)
                {
                    case MemoryType.Core:
                        yOffset = 100f;
                        break;
                    case MemoryType.Skill:
                        yOffset = -100f;
                        break;
                    case MemoryType.Peripheral:
                        yOffset = 0f;
                        break;
                }
                
                // Create node
                GameObject nodeObj = CreateMemoryNode(memory, new Vector2(xPos, yPos + yOffset));
                memoryNodeObjects[memory.Id] = nodeObj;
                
                // Create connection to timeline
                CreateTimelineMarker(new Vector2(xPos, yPos), new Vector2(xPos, yPos + yOffset));
            }
        }
    }
    
    // Create category header
    private void CreateCategoryHeader(string title, Vector2 position)
    {
        GameObject headerObj = new GameObject(title);
        headerObj.transform.SetParent(journalContainer, false);
        
        Text headerText = headerObj.AddComponent<Text>();
        headerText.text = title;
        headerText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        headerText.fontSize = 24;
        headerText.alignment = TextAnchor.MiddleCenter;
        headerText.color = Color.white;
        
        RectTransform headerRect = headerObj.GetComponent<RectTransform>();
        headerRect.anchoredPosition = position;
        headerRect.sizeDelta = new Vector2(300, 50);
    }
    
    // Create memory nodes in a grid layout
    private void CreateMemoryNodes(List<MemoryFragment> memories, Vector2 centerPosition, float width)
    {
        int columns = Mathf.CeilToInt(Mathf.Sqrt(memories.Count));
        float spacing = width / columns;
        
        for (int i = 0; i < memories.Count; i++)
        {
            int row = i / columns;
            int col = i % columns;
            
            float xPos = centerPosition.x - (width / 2) + (col * spacing) + (spacing / 2);
            float yPos = centerPosition.y - (row * spacing);
            
            GameObject nodeObj = CreateMemoryNode(memories[i], new Vector2(xPos, yPos));
            memoryNodeObjects[memories[i].Id] = nodeObj;
        }
    }
    
    // Create a single memory node
    private GameObject CreateMemoryNode(MemoryFragment memory, Vector2 position)
    {
        // Select prefab based on memory type
        GameObject prefab = null;
        
        switch (memory.Type)
        {
            ca
(Content truncated due to size limit. Use line ranges to read in chunks)