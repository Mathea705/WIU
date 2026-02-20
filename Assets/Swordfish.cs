using UnityEngine;

public class Swordfish : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform eyePoint;

    [Header("Detection")]
    public float detectionRange = 30f;
    public float viewAngle = 90f;
    public LayerMask obstacleMask;

    [Header("Movement")]
    public float swimSpeed = 5f;
    public float attackSpeed = 20f;
    public float restSpeed = 8f;
    public float turnSpeed = 5f;

    [Header("Advanced AI")]
    public float memoryDuration = 2f;

    [Header("Attack Behaviour")]
    public float aimDuration = 1f;

    private enum State { Swimming, Attack, Rest }
    private State currentState;

    private float stateTimer;
    private float memoryTimer;

    private Vector3 randomDirection;
    private Vector3 lastKnownPosition;

    private Vector3 lockedAttackPosition;
    private bool hasLockedPosition = false;

    void Start()
    {
        currentState = State.Swimming;
        PickRandomDirection();
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Swimming:
                Swim();

                if (CanSeePlayer())
                {
                    memoryTimer = memoryDuration;
                    lastKnownPosition = player.position;

                    currentState = State.Attack;
                    stateTimer = aimDuration;
                    hasLockedPosition = false;
                }
                else if (memoryTimer > 0)
                {
                    memoryTimer -= Time.deltaTime;
                }
                break;

            case State.Attack:
                Attack();
                break;

            case State.Rest:
                Rest();
                break;
        }
    }

    // ===================== STATES =====================

    void Swim()
    {
        MoveInDirection(randomDirection, swimSpeed);

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0)
        {
            PickRandomDirection();
        }
    }

    void Attack()
    {
        stateTimer -= Time.deltaTime;

        // ===== AIM PHASE =====
        if (stateTimer > 0)
        {
            // Face player while stopping
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            MoveInDirection(dirToPlayer, 0f);

            // Lock position once
            if (!hasLockedPosition)
            {
                lockedAttackPosition = player.position;
                hasLockedPosition = true;
            }
        }
        else
        {
            // ===== DASH PHASE (NO TRACKING) =====
            Vector3 dirToTarget = (lockedAttackPosition - transform.position).normalized;
            MoveInDirection(dirToTarget, attackSpeed);

            // If reached target → go rest
            float dist = Vector3.Distance(transform.position, lockedAttackPosition);
            if (dist < 1.5f)
            {
                currentState = State.Rest;
                stateTimer = 5f;
            }
        }
    }

    void Rest()
    {
        stateTimer -= Time.deltaTime;

        Vector3 awayDir = (transform.position - player.position).normalized;
        MoveInDirection(awayDir, restSpeed);

        if (stateTimer <= 0)
        {
            currentState = State.Swimming;
            PickRandomDirection();
        }
    }

    // ===================== MOVEMENT =====================

    void MoveInDirection(Vector3 dir, float speed)
    {
        if (dir == Vector3.zero) return;

        transform.position += dir * speed * Time.deltaTime;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            turnSpeed * Time.deltaTime
        );
    }

    void PickRandomDirection()
    {
        randomDirection = new Vector3(
            Random.Range(-1f, 1f),
            0,
            Random.Range(-1f, 1f)
        ).normalized;

        stateTimer = Random.Range(2f, 4f);
    }

    // ===================== DETECTION =====================

    bool CanSeePlayer()
    {
        if (player == null || eyePoint == null)
            return false;

        Vector3 dirToPlayer = (player.position - eyePoint.position).normalized;
        float distance = Vector3.Distance(eyePoint.position, player.position);

        if (distance > detectionRange)
            return false;

        float angle = Vector3.Angle(eyePoint.forward, dirToPlayer);
        if (angle > viewAngle / 2f)
            return false;

        Ray ray = new Ray(eyePoint.position, dirToPlayer);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, detectionRange))
        {
            if (hit.transform != player)
                return false;
        }

        return true;
    }

    // ===================== COLLISION =====================

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            currentState = State.Rest;
            stateTimer = 5f;
        }
    }

    // ===================== DEBUG =====================

    void OnDrawGizmos()
    {
        if (eyePoint == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(eyePoint.position, detectionRange);

        Vector3 left = Quaternion.Euler(0, -viewAngle / 2, 0) * eyePoint.forward;
        Vector3 right = Quaternion.Euler(0, viewAngle / 2, 0) * eyePoint.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(eyePoint.position, eyePoint.position + left * detectionRange);
        Gizmos.DrawLine(eyePoint.position, eyePoint.position + right * detectionRange);
    }
}
