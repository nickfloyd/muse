// PlayerMovement.cs - Handles player character movement and controls
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    // Movement parameters
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5.0f;
    [SerializeField] private float runSpeed = 8.0f;
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float gravity = 20.0f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    
    // Ground check
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    
    // Animation
    [Header("Animation")]
    [SerializeField] private Animator animator;
    
    // Components
    private CharacterController controller;
    private Camera mainCamera;
    
    // Movement state
    private Vector3 moveDirection = Vector3.zero;
    private float turnSmoothVelocity;
    private bool isGrounded;
    private bool isRunning;
    
    // Animation parameters
    private readonly int animIsWalking = Animator.StringToHash("IsWalking");
    private readonly int animIsRunning = Animator.StringToHash("IsRunning");
    private readonly int animJump = Animator.StringToHash("Jump");
    private readonly int animMoveSpeed = Animator.StringToHash("MoveSpeed");
    
    private void Awake()
    {
        // Get components
        controller = GetComponent<CharacterController>();
        
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }
    
    private void Start()
    {
        // Find main camera
        mainCamera = Camera.main;
    }
    
    private void Update()
    {
        // Check if player is disabled
        if (GameManager.Instance.PlayerManager.CurrentPlayerState == PlayerState.Disabled)
        {
            return;
        }
        
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        if (isGrounded && moveDirection.y < 0)
        {
            moveDirection.y = -2f; // Small negative value to keep grounded
        }
        
        // Get input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        // Check for running
        isRunning = Input.GetKey(KeyCode.LeftShift) && vertical > 0;
        
        // Calculate movement
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        
        if (direction.magnitude >= 0.1f)
        {
            // Calculate target angle based on camera
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            
            if (mainCamera != null)
            {
                targetAngle += mainCamera.transform.eulerAngles.y;
            }
            
            // Smooth rotation
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            
            // Calculate move direction
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            
            // Apply speed
            float currentSpeed = isRunning ? runSpeed : walkSpeed;
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);
        }
        
        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            moveDirection.y = jumpForce;
            
            // Trigger jump animation
            if (animator != null)
            {
                animator.SetTrigger(animJump);
            }
        }
        
        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;
        
        // Apply vertical movement
        controller.Move(moveDirection * Time.deltaTime);
        
        // Update animations
        UpdateAnimations(direction.magnitude);
    }
    
    private void UpdateAnimations(float moveMagnitude)
    {
        if (animator == null)
        {
            return;
        }
        
        // Set animation parameters
        animator.SetBool(animIsWalking, moveMagnitude > 0.1f);
        animator.SetBool(animIsRunning, isRunning);
        animator.SetFloat(animMoveSpeed, isRunning ? 1.0f : 0.5f);
    }
    
    // Method to force movement (for cutscenes, etc.)
    public void ForceMove(Vector3 direction, float speed, bool rotateToDirection = true)
    {
        if (rotateToDirection && direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
        
        controller.Move(direction.normalized * speed * Time.deltaTime);
    }
    
    // Method to teleport player
    public void Teleport(Vector3 position)
    {
        controller.enabled = false;
        transform.position = position;
        controller.enabled = true;
    }
}
