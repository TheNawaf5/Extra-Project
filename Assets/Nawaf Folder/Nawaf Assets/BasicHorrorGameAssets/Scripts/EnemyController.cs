using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    public Transform[] waypoints;
    public float idleTime = 2f;
    public float walkSpeed = 2f; // Walking speed.
    public float chaseSpeed = 4f; // Chasing speed.
    public float sightDistance = 10f;
    
    [Header("Attack")]
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public float attackAnimationDuration = 1f; // Length of attack animation
    public int attackDamage = 1;
    
    [Header("Audio")]
    public AudioClip idleSound;
    public AudioClip walkingSound;
    public AudioClip chasingSound;
    public AudioClip attackSound;

    private int currentWaypointIndex = 0;
    private NavMeshAgent agent;
    private Animator animator;
    private float idleTimer = 0f;
    private Transform player;
    private AudioSource audioSource;
    private PlayerHealth playerHealth;
    private float lastAttackTime = 0f;
    private float attackStartTime = 0f;
    private bool hasDealtDamage = false;

    private enum EnemyState { Idle, Walk, Chase, Attack }
    private EnemyState currentState = EnemyState.Idle;

    private bool isChasingAnimation = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        audioSource = GetComponent<AudioSource>();
        
        // Debug animator setup
        if (animator == null)
        {
            Debug.LogError("No Animator component found on " + gameObject.name);
        }
        else
        {
            Debug.Log($"Animator found on {gameObject.name}. Controller: {animator.runtimeAnimatorController?.name}");
            
            // Check if required parameters exist
            CheckAnimatorParameters();
        }
        
        // Get player health component
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }
        
        SetDestinationToWaypoint();
    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                idleTimer += Time.deltaTime;
                SetAnimatorBool("IsWalking", false);
                SetAnimatorBool("IsChasing", false); // Ensure IsChasing is set to false in the idle state.
                PlaySound(idleSound);

                if (idleTimer >= idleTime)
                {
                    NextWaypoint();
                }

                CheckForPlayerDetection();
                break;

            case EnemyState.Walk:
                idleTimer = 0f;
                SetAnimatorBool("IsWalking", true);
                SetAnimatorBool("IsChasing", false); // Set IsChasing to false when walking.
                PlaySound(walkingSound);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    currentState = EnemyState.Idle;
                }

                CheckForPlayerDetection();
                break;

            case EnemyState.Chase:
                idleTimer = 0f;
                agent.speed = chaseSpeed; // Set the chase speed.
                
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);
                
                // Check if close enough to attack
                if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
                {
                    currentState = EnemyState.Attack;
                    agent.SetDestination(transform.position); // Stop moving
                    break;
                }
                
                agent.SetDestination(player.position);
                isChasingAnimation = true; // Set to true in chase state.
                SetAnimatorBool("IsChasing", true); // Set IsChasing to true in chase state.

                // Play chasing sound.
                PlaySound(chasingSound);

                // Check if the player is out of sight and go back to the walk state.
                if (distanceToPlayer > sightDistance)
                {
                    currentState = EnemyState.Walk;
                    agent.speed = walkSpeed; // Restore walking speed.
                }
                break;
                
            case EnemyState.Attack:
                // Stop all movement
                agent.SetDestination(transform.position);
                agent.velocity = Vector3.zero;
                
                // Record attack start time if this is the first frame of attack
                if (attackStartTime == 0f)
                {
                    attackStartTime = Time.time;
                    hasDealtDamage = false;
                    Debug.Log("Enemy attack animation started!");
                    
                    // Set attack animation with debugging
                    SetAnimatorBool("IsWalking", false);
                    SetAnimatorBool("IsChasing", false);
                    SetAnimatorBool("IsAttacking", true);
                }
                
                // Play attack sound (only once at the start)
                if (Time.time <= attackStartTime + 0.1f) // Play sound in first 0.1 seconds
                {
                    PlaySound(attackSound);
                }
                
                // Deal damage at the middle of the attack animation
                float damageTime = attackStartTime + (attackAnimationDuration * 0.5f); // Halfway through animation
                if (Time.time >= damageTime && !hasDealtDamage)
                {
                    AttackPlayer();
                    hasDealtDamage = true;
                    lastAttackTime = Time.time;
                }
                
                // Return to chase state after attack animation completes
                if (Time.time >= attackStartTime + attackAnimationDuration)
                {
                    SetAnimatorBool("IsAttacking", false);
                    attackStartTime = 0f; // Reset for next attack
                    currentState = EnemyState.Chase;
                    Debug.Log("Enemy attack animation completed!");
                }
                break;
        }
    }

    private void CheckForPlayerDetection()
    {
        RaycastHit hit;
        Vector3 playerDirection = player.position - transform.position;

        if (Physics.Raycast(transform.position, playerDirection.normalized, out hit, sightDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                currentState = EnemyState.Chase;
                Debug.Log("Player detected!");
            }
        }
    }

    private void PlaySound(AudioClip soundClip)
    {
        if (!audioSource.isPlaying || audioSource.clip != soundClip)
        {
            audioSource.clip = soundClip;
            audioSource.Play();
        }
    }

    private void NextWaypoint()
    {
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        SetDestinationToWaypoint();
    }

    private void SetDestinationToWaypoint()
    {
        agent.SetDestination(waypoints[currentWaypointIndex].position);
        currentState = EnemyState.Walk;
        agent.speed = walkSpeed; // Set the walking speed.
        animator.enabled = true;
    }

    private void AttackPlayer()
    {
        if (playerHealth != null && !playerHealth.IsDead)
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log($"Enemy attacked player! Player health: {playerHealth.CurrentHealth}");
        }
    }

    private void CheckAnimatorParameters()
    {
        if (animator == null || animator.runtimeAnimatorController == null) return;
        
        string[] requiredParams = { "IsWalking", "IsChasing", "AttackTrigger" };
        
        foreach (string param in requiredParams)
        {
            bool hasParam = false;
            foreach (AnimatorControllerParameter parameter in animator.parameters)
            {
                if (parameter.name == param)
                {
                    hasParam = true;
                    Debug.Log($"✓ Found animator parameter: {param} (Type: {parameter.type})");
                    break;
                }
            }
            
            if (!hasParam)
            {
                Debug.LogError($"✗ Missing animator parameter: {param}");
            }
        }
    }

    private void SetAnimatorBool(string parameterName, bool value)
    {
        if (animator == null)
        {
            Debug.LogError("Animator is null!");
            return;
        }
        
        if (animator.runtimeAnimatorController == null)
        {
            Debug.LogError("Animator has no controller assigned!");
            return;
        }
        
        // Check if parameter exists
        bool parameterExists = false;
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == parameterName)
            {
                parameterExists = true;
                break;
            }
        }
        
        if (!parameterExists)
        {
            Debug.LogError($"Animator parameter '{parameterName}' does not exist!");
            return;
        }
        
        animator.SetBool(parameterName, value);
        Debug.Log($"Set animator parameter '{parameterName}' to {value}");
    }

    // Draw a green raycast line at all times and switch to red when the player is detected.
    private void OnDrawGizmos()
    {
        if (player != null)
        {
            // Sight line
            Gizmos.color = currentState == EnemyState.Chase ? Color.red : Color.green;
            Gizmos.DrawLine(transform.position, player.position);
            
            // Attack range circle
            Gizmos.color = currentState == EnemyState.Attack ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            
            // Sight distance circle
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, sightDistance);
        }
    }
}
