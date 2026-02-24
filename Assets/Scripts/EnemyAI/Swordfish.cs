


// using UnityEngine;

// public class Swordfish : BossAI
// {
//     [Header("Detection (Cone - Z Forward)")]
//     [SerializeField] private float detectionRange = 30f;
//     [SerializeField] private float viewAngle = 90f;

//     [Header("Movement")]
//     [SerializeField] private float swimSpeed = 6f;
//     [SerializeField] private float dashSpeed = 25f;
//     [SerializeField] private float turnSpeed = 5f;

//     [Header("Attack Settings")]
//     [SerializeField] private float attackCooldown = 4f;
//     [SerializeField] private float aimDuration = 1f;
//     [SerializeField] private float attackDamage = 20f;

//     [Header("Rest")]
//     [SerializeField] private float retreatSpeed = 8f;
//     [SerializeField] private float restDuration = 5f;

//     [Header("Hit Detection")]
//     [SerializeField] private float hitDistance = 2.5f;

//     private float _lastAttackTime;
//     private Vector3 _moveDirection;
//     private Vector3 _lockedPosition;
//     private float _stateTimer;
//     private bool _hasHitBoatThisDash = false;

//     private enum State { Swim, Aim, Dash, Rest }
//     private State _currentState;

//     protected override void Start()
//     {
//         // 1. Calls BossAI.Start() to setup Health, find "Ship", and cache shipHealth
//         base.Start(); 
        
//         _currentState = State.Swim;
//         PickRandomDirection();
//     }

//     protected override void Update()
//     {
//         // 2. Calls BossAI.Update() to keep the health bar UI lerping
//         base.Update();

//         // Safety: If the ship is gone or health is 0, stop AI logic
//         if (shipObject == null || currentHealth <= 0) return;

//         switch (_currentState)
//         {
//             case State.Swim:
//                 Swim();
//                 DetectPlayer();
//                 break;
//             case State.Aim:
//                 Aim();
//                 break;
//             case State.Dash:
//                 Dash();
//                 break;
//             case State.Rest:
//                 Rest();
//                 break;
//         }
//     }

//     // ================= INTEGRATION WITH BossAI =================

//     private void Dash()
//     {
//         Vector3 dir = (_lockedPosition - transform.position).normalized;
//         Vector3 previousPosition = transform.position;

//         Move(dir, dashSpeed);

//         if (!_hasHitBoatThisDash)
//         {
//             float currentDist = Vector3.Distance(transform.position, shipObject.transform.position);
//             float prevDist = Vector3.Distance(previousPosition, shipObject.transform.position);

//             // 3. USE BossAI.DealDamageToShip
//             // This safely uses the 'shipHealth' component cached in the base class
//             if (currentDist <= hitDistance || prevDist <= hitDistance)
//             {
//                 if (shipHealth != null)
//                 {
//                     DealDamageToShip(attackDamage);
//                     Debug.Log("You have taken " + attackDamage + " damage!");
//                     _hasHitBoatThisDash = true;
//                     TransitionToState(State.Rest, restDuration);
//                 }
//             }
//         }

//         if (Vector3.Distance(transform.position, _lockedPosition) < 1.0f)
//         {
//             TransitionToState(State.Rest, restDuration);
//         }
//     }

//     // 4. OVERRIDING OnDeath
//     // This allows you to add specific Swordfish death logic before the base class destroys the object
//     protected override void OnDeath()
//     {
//         Debug.Log("The Great Swordfish has been defeated!");
        
//         // Add any loot drops or particle effects here
        
//         // Call base.OnDeath to hide the UI panel and finally Destroy(gameObject)
//         base.OnDeath();
//     }

//     // ================= MOVEMENT & AI LOGIC =================

//     private void DetectPlayer()
//     {
//         Vector3 dirToBoat = shipObject.transform.position - transform.position;
//         if (dirToBoat.magnitude > detectionRange) return;
//         if (Time.time < _lastAttackTime + attackCooldown) return;

//         if (Vector3.Angle(transform.forward, dirToBoat.normalized) < viewAngle / 2f)
//         {
//             TransitionToState(State.Aim, aimDuration);
//             _lockedPosition = shipObject.transform.position;
//             _hasHitBoatThisDash = false;
//         }
//     }

//     private void Aim()
//     {
//         _stateTimer -= Time.deltaTime;
//         Vector3 dir = (shipObject.transform.position - transform.position).normalized;
//         RotateTowards(dir);

//         if (_stateTimer <= 0) TransitionToState(State.Dash, 0);
//     }

//     private void Rest()
//     {
//         _stateTimer -= Time.deltaTime;
//         Vector3 awayDir = (transform.position - shipObject.transform.position).normalized;
//         Move(awayDir, retreatSpeed);

//         if (_stateTimer <= 0)
//         {
//             _lastAttackTime = Time.time;
//             TransitionToState(State.Swim, 0);
//             PickRandomDirection();
//         }
//     }

//     private void Swim()
//     {
//         Move(_moveDirection, swimSpeed);
//         _stateTimer -= Time.deltaTime;
//         if (_stateTimer <= 0) PickRandomDirection();
//     }

//     private void TransitionToState(State newState, float duration)
//     {
//         _currentState = newState;
//         _stateTimer = duration;
//     }

//     private void PickRandomDirection()
//     {
//         _moveDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
//         _stateTimer = Random.Range(2f, 4f);
//     }

//     private void Move(Vector3 dir, float speed)
//     {
//         if (dir == Vector3.zero) return;
//         dir.y = 0; 
//         transform.position += dir * speed * Time.deltaTime;
//         RotateTowards(dir);
//     }

//     private void RotateTowards(Vector3 dir)
//     {
//         if (dir == Vector3.zero) return;
//         dir.y = 0;
//         Quaternion rot = Quaternion.LookRotation(dir);
//         transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnSpeed * Time.deltaTime);
//     }

//     void OnDrawGizmos()
//     {
//         if (transform == null) return;
//         Gizmos.color = Color.yellow;
//         Gizmos.DrawWireSphere(transform.position, detectionRange);

//         Vector3 left = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
//         Vector3 right = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;

//         Gizmos.color = Color.red;
//         Gizmos.DrawLine(transform.position, transform.position + left * detectionRange);
//         Gizmos.DrawLine(transform.position, transform.position + right * detectionRange);
//     }
// }



using UnityEngine;

public class Swordfish : MonoBehaviour
{
<<<<<<< Updated upstream
    [Header("References")]
    public Transform player;   // optional
    public Transform boat;     // main target

=======
>>>>>>> Stashed changes
    [Header("Detection (Cone - Z Forward)")]
    [SerializeField] private float detectionRange = 30f;
    [SerializeField] private float viewAngle = 90f;

    [Header("Movement")]
    [SerializeField] private float swimSpeed = 6f;
    [SerializeField] private float dashSpeed = 25f;
    [SerializeField] private float turnSpeed = 5f;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 4f;
    [SerializeField] private float aimDuration = 1f;
    [SerializeField] private float attackDamage = 20f;

    [Header("Rest")]
    [SerializeField] private float retreatSpeed = 8f;
    [SerializeField] private float restDuration = 5f;

    private float _lastAttackTime;
    private Vector3 _moveDirection;
    private Vector3 _lockedPosition;
    private float _stateTimer;
    private bool _hasHitBoatThisDash = false;

    private enum State { Swim, Aim, Dash, Rest }
    private State _currentState;

<<<<<<< Updated upstream
    private bool hasHitBoatThisDash = false;

    // ================= START =================
    void Start()
    {
        currentState = State.Swim;
=======
    protected override void Start()
    {
        base.Start(); // Sets up health and references shipObject
        _currentState = State.Swim;
>>>>>>> Stashed changes
        PickRandomDirection();
    }

    protected override void Update()
    {
        base.Update(); // Updates UI health bar

        if (shipObject == null || currentHealth <= 0) return;

        switch (_currentState)
        {
            case State.Swim:
                Swim();
                DetectPlayer();
                break;
            case State.Aim:
                Aim();
                break;
            case State.Dash:
                Dash();
                break;
            case State.Rest:
                Rest();
                break;
        }
    }

   public override void TakeDamage(float amount)
{
    // Call the base logic (Health reduction, Red Flash, Health Bar UI)
    base.TakeDamage(amount);

    // Add Swordfish-specific reactions here:
    if (currentHealth < maxHealth * 0.5f) 
    {
        // Example: If health is below 50%, make it move faster immediately
        turnSpeed = 10f; 
        Debug.Log("Swordfish is enraged!");
    }

    // Example: If it was resting, being hit wakes it up faster
    if (_currentState == State.Rest)
    {
        _stateTimer -= 1f; // Reduce rest time by 1 second every time it's hit
    }
}

    // ================= PHYSICS COLLISION =================

    private void OnTriggerEnter(Collider other)
    {
<<<<<<< Updated upstream
        if (boat == null) return;

        Vector3 dirToBoat = boat.position - transform.position;
        float distance = dirToBoat.magnitude;

        if (distance > detectionRange) return;

        dirToBoat.Normalize();

        float angle = Vector3.Angle(transform.forward, dirToBoat);

        if (angle < viewAngle / 2f && Time.time >= lastAttackTime + attackCooldown)
        {
            currentState = State.Aim;
            stateTimer = aimDuration;
            lockedPosition = boat.position;
            hasHitBoatThisDash = false;
=======
        // Only register hits if we are in Dash state
        if (_currentState == State.Dash && !_hasHitBoatThisDash)
        {
            // Check if we hit the ship
            if (other.gameObject == shipObject || other.CompareTag("Ship"))
            {
                Debug.Log("collide");
                
                DealDamageToShip(attackDamage);
                _hasHitBoatThisDash = true;

                // Stop the dash immediately and move to rest
                TransitionToState(State.Rest, restDuration);
            }
>>>>>>> Stashed changes
        }
    }

    // ================= STATE LOGIC =================

    private void Swim()
    {
<<<<<<< Updated upstream
        if (boat == null) return;
=======
        Move(_moveDirection, swimSpeed);
        _stateTimer -= Time.deltaTime;
        if (_stateTimer <= 0) PickRandomDirection();
    }
>>>>>>> Stashed changes

    private void DetectPlayer()
    {
        Vector3 dirToBoat = shipObject.transform.position - transform.position;
        if (dirToBoat.magnitude > detectionRange) return;
        if (Time.time < _lastAttackTime + attackCooldown) return;

<<<<<<< Updated upstream
        Vector3 dir = (boat.position - transform.position).normalized;
=======
        float angle = Vector3.Angle(transform.forward, dirToBoat.normalized);
        if (angle < viewAngle / 2f)
        {
            TransitionToState(State.Aim, aimDuration);
            _lockedPosition = shipObject.transform.position;
            _hasHitBoatThisDash = false;
        }
    }

    private void Aim()
    {
        _stateTimer -= Time.deltaTime;
        Vector3 dir = (shipObject.transform.position - transform.position).normalized;
>>>>>>> Stashed changes
        RotateTowards(dir);

        if (_stateTimer <= 0) TransitionToState(State.Dash, 0);
    }

    private void Dash()
    {
        Vector3 dir = (_lockedPosition - transform.position).normalized;
        Move(dir, dashSpeed);

<<<<<<< Updated upstream
        if (!hasHitBoatThisDash && boat != null)
        {
            float currentDist = Vector3.Distance(transform.position, boat.position);
            float previousDist = Vector3.Distance(previousPosition, boat.position);

            if (currentDist <= hitDistance || previousDist <= hitDistance)
            {
                Debug.Log("collide");

                hasHitBoatThisDash = true;

                currentState = State.Rest;
                stateTimer = restDuration;
            }
        }

        // End dash if reached target
        float distToTarget = Vector3.Distance(transform.position, lockedPosition);
        if (distToTarget < 1.5f)
        {
            currentState = State.Rest;
            stateTimer = restDuration;
=======
        // Fail-safe: if we reached the target point but missed the trigger
        if (Vector3.Distance(transform.position, _lockedPosition) < 1.0f)
        {
            TransitionToState(State.Rest, restDuration);
>>>>>>> Stashed changes
        }
    }

    private void Rest()
    {
<<<<<<< Updated upstream
        if (boat == null) return;

        stateTimer -= Time.deltaTime;

        Vector3 awayDir = (transform.position - boat.position).normalized;
=======
        _stateTimer -= Time.deltaTime;
        Vector3 awayDir = (transform.position - shipObject.transform.position).normalized;
>>>>>>> Stashed changes
        Move(awayDir, retreatSpeed);

        if (_stateTimer <= 0)
        {
            _lastAttackTime = Time.time;
            TransitionToState(State.Swim, 0);
            PickRandomDirection();
        }
    }

    // ================= HELPERS =================

    private void TransitionToState(State newState, float duration)
    {
        _currentState = newState;
        _stateTimer = duration;
    }

    private void PickRandomDirection()
    {
        _moveDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        _stateTimer = Random.Range(2f, 4f);
    }

    private void Move(Vector3 dir, float speed)
    {
        if (dir == Vector3.zero) return;
        dir.y = 0; 
        transform.position += dir * speed * Time.deltaTime;
        RotateTowards(dir);
    }

    private void RotateTowards(Vector3 dir)
    {
        if (dir == Vector3.zero) return;
        dir.y = 0;
        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnSpeed * Time.deltaTime);
    }

    protected override void OnDeath()
    {
        // Custom death logic can go here (particles, etc.)
        base.OnDeath(); 
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Vector3 left = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + left * detectionRange);
        Gizmos.DrawLine(transform.position, transform.position + right * detectionRange);
    }
}
