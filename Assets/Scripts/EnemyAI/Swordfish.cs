


using UnityEngine;

public class Swordfish : BossAI
{
    [Header("Detection (Cone - Z Forward)")]
    public float detectionRange = 30f;
    public float viewAngle = 90f;

    [Header("Movement")]
    public float swimSpeed = 6f;
    public float dashSpeed = 25f;
    public float turnSpeed = 5f;

    [Header("Attack")]
    public float attackCooldown = 4f;
    public float aimDuration = 1f;
    public float attackDamage = 20f;

    [Header("Rest")]
    public float retreatSpeed = 8f;
    public float restDuration = 5f;

    [Header("Hit Detection")]
    public float hitDistance = 2.5f; 

    [Header("Y-Axis Control")]
    [SerializeField] private float YOffset = 0f;

    private float lastAttackTime;
    private Vector3 moveDirection;
    private Vector3 lockedPosition;
    private float stateTimer;

    private enum State { Swim, Aim, Dash, Rest }
    private State currentState;
    private bool hasHitBoatThisDash = false;

    protected override void Start()
    {
        base.Start();
        currentState = State.Swim;
        
        // Initial positioning
        Vector3 startPos = transform.position;
        startPos.y = YOffset;
        transform.position = startPos;

        PickRandomDirection();
    }

    protected override void Update()
    {
        base.Update(); // Keeps health bar working

        if (shipObject == null || currentHealth <= 0) return;

        Vector3 pos = transform.position;
        pos.y = YOffset;
        transform.position = pos;

        switch (currentState)
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

    // ================= LOGIC =================

    void Swim()
    {
        Move(moveDirection, swimSpeed);
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0) PickRandomDirection();
    }

    void DetectPlayer()
    {
        Vector3 dirToBoat = shipObject.transform.position - transform.position;
        dirToBoat.y = 0; // Ignore height for detection
        
        if (dirToBoat.magnitude > detectionRange) return;
        if (Time.time < lastAttackTime + attackCooldown) return;

        float angle = Vector3.Angle(transform.forward, dirToBoat.normalized);
        if (angle < viewAngle / 2f)
        {
            currentState = State.Aim;
            stateTimer = aimDuration;
            lockedPosition = shipObject.transform.position;
            lockedPosition.y = YOffset; // Lock the target point to the same height
            hasHitBoatThisDash = false;
        }
    }

    void Aim()
    {
        stateTimer -= Time.deltaTime;
        Vector3 dir = (shipObject.transform.position - transform.position).normalized;
        RotateTowards(dir);

        if (stateTimer <= 0) currentState = State.Dash;
    }

    void Dash()
    {
        Vector3 dir = (lockedPosition - transform.position).normalized;
        dir.y = 0; // Ensure dash move is purely horizontal

        Vector3 previousPosition = transform.position;
        Move(dir, dashSpeed);

        if (!hasHitBoatThisDash)
        {
            float currentDist = Vector3.Distance(transform.position, shipObject.transform.position);
            // Check distance but ignore Y height difference for hit detection
            Vector3 flatFish = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 flatShip = new Vector3(shipObject.transform.position.x, 0, shipObject.transform.position.z);

            if (Vector3.Distance(flatFish, flatShip) <= hitDistance)
            {
                DealDamageToShip(attackDamage);
                hasHitBoatThisDash = true;
                currentState = State.Rest;
                stateTimer = restDuration;
            }
        }

        if (Vector3.Distance(transform.position, lockedPosition) < 1.5f)
        {
            currentState = State.Rest;
            stateTimer = restDuration;
        }
    }

    void Rest()
    {
        stateTimer -= Time.deltaTime;
        Vector3 awayDir = (transform.position - shipObject.transform.position).normalized;
        awayDir.y = 0;
        Move(awayDir, retreatSpeed);

        if (stateTimer <= 0)
        {
            currentState = State.Swim;
            lastAttackTime = Time.time;
            PickRandomDirection();
        }
    }

    // ================= HELPERS =================

    void Move(Vector3 dir, float speed)
    {
        if (dir == Vector3.zero) return;
        dir.y = 0; 
        transform.position += dir * speed * Time.deltaTime;
        RotateTowards(dir);
    }

    void RotateTowards(Vector3 dir)
    {
        if (dir == Vector3.zero) return;
        dir.y = 0; // Prevents the nose from tilting up or down
        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnSpeed * Time.deltaTime);
    }

    void PickRandomDirection()
    {
        moveDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        stateTimer = Random.Range(2f, 4f);
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



