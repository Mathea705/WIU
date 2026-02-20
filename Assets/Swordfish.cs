// using UnityEngine;

// public class Swordfish : MonoBehaviour
// {
//     [Header("References")]
//     public Transform player;
//     public Transform eyePoint;

//     [Header("Detection")]
//     public float detectionRange = 30f;
//     public float viewAngle = 90f;
//     public LayerMask obstacleMask;

//     [Header("Movement")]
//     public float swimSpeed = 5f;
//     public float attackSpeed = 20f;
//     public float restSpeed = 8f;
//     public float turnSpeed = 5f;

//     [Header("Advanced AI")]
//     public float memoryDuration = 2f;

//     [Header("Attack Behaviour")]
//     public float aimDuration = 1f;

//     private enum State { Swimming, Attack, Rest }
//     private State currentState;

//     private float stateTimer;
//     private float memoryTimer;

//     private Vector3 randomDirection;
//     private Vector3 lastKnownPosition;

//     private Vector3 lockedAttackPosition;
//     private bool hasLockedPosition = false;

//     void Start()
//     {
//         currentState = State.Swimming;
//         PickRandomDirection();
//     }

//     void Update()
//     {
//         switch (currentState)
//         {
//             case State.Swimming:
//                 Swim();

//                 if (CanSeePlayer())
//                 {
//                     memoryTimer = memoryDuration;
//                     lastKnownPosition = player.position;

//                     currentState = State.Attack;
//                     stateTimer = aimDuration;
//                     hasLockedPosition = false;
//                 }
//                 else if (memoryTimer > 0)
//                 {
//                     memoryTimer -= Time.deltaTime;
//                 }
//                 break;

//             case State.Attack:
//                 Attack();
//                 break;

//             case State.Rest:
//                 Rest();
//                 break;
//         }
//     }

//     // ===================== STATES =====================

//     void Swim()
//     {
//         MoveInDirection(randomDirection, swimSpeed);

//         stateTimer -= Time.deltaTime;
//         if (stateTimer <= 0)
//         {
//             PickRandomDirection();
//         }
//     }

//     void Attack()
//     {
//         stateTimer -= Time.deltaTime;

//         // ===== AIM PHASE =====
//         if (stateTimer > 0)
//         {
//             // Face player while stopping
//             Vector3 dirToPlayer = (player.position - transform.position).normalized;
//             MoveInDirection(dirToPlayer, 0f);

//             // Lock position once
//             if (!hasLockedPosition)
//             {
//                 lockedAttackPosition = player.position;
//                 hasLockedPosition = true;
//             }
//         }
//         else
//         {
//             // ===== DASH PHASE (NO TRACKING) =====
//             Vector3 dirToTarget = (lockedAttackPosition - transform.position).normalized;
//             MoveInDirection(dirToTarget, attackSpeed);

//             // If reached target → go rest
//             float dist = Vector3.Distance(transform.position, lockedAttackPosition);
//             if (dist < 1.5f)
//             {
//                 currentState = State.Rest;
//                 stateTimer = 5f;
//             }
//         }
//     }

//     void Rest()
//     {
//         stateTimer -= Time.deltaTime;

//         Vector3 awayDir = (transform.position - player.position).normalized;
//         MoveInDirection(awayDir, restSpeed);

//         if (stateTimer <= 0)
//         {
//             currentState = State.Swimming;
//             PickRandomDirection();
//         }
//     }

//     // ===================== MOVEMENT =====================

//     void MoveInDirection(Vector3 dir, float speed)
//     {
//         if (dir == Vector3.zero) return;

//         transform.position += dir * speed * Time.deltaTime;

//         Quaternion targetRot = Quaternion.LookRotation(dir);
//         transform.rotation = Quaternion.Slerp(
//             transform.rotation,
//             targetRot,
//             turnSpeed * Time.deltaTime
//         );
//     }

//     void PickRandomDirection()
//     {
//         randomDirection = new Vector3(
//             Random.Range(-1f, 1f),
//             0,
//             Random.Range(-1f, 1f)
//         ).normalized;

//         stateTimer = Random.Range(2f, 4f);
//     }

//     // ===================== DETECTION =====================

//     bool CanSeePlayer()
//     {
//         if (player == null || eyePoint == null)
//             return false;

//         Vector3 dirToPlayer = (player.position - eyePoint.position).normalized;
//         float distance = Vector3.Distance(eyePoint.position, player.position);

//         if (distance > detectionRange)
//             return false;

//         float angle = Vector3.Angle(eyePoint.forward, dirToPlayer);
//         if (angle > viewAngle / 2f)
//             return false;

//         Ray ray = new Ray(eyePoint.position, dirToPlayer);
//         RaycastHit hit;

//         if (Physics.Raycast(ray, out hit, detectionRange))
//         {
//             if (hit.transform != player)
//                 return false;
//         }

//         return true;
//     }

//     // ===================== COLLISION =====================

//     private void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             currentState = State.Rest;
//             stateTimer = 5f;
//         }
//     }

//     // ===================== DEBUG =====================

//     void OnDrawGizmos()
//     {
//         if (eyePoint == null) return;

//         Gizmos.color = Color.yellow;
//         Gizmos.DrawWireSphere(eyePoint.position, detectionRange);

//         Vector3 left = Quaternion.Euler(0, -viewAngle / 2, 0) * eyePoint.forward;
//         Vector3 right = Quaternion.Euler(0, viewAngle / 2, 0) * eyePoint.forward;

//         Gizmos.color = Color.red;
//         Gizmos.DrawLine(eyePoint.position, eyePoint.position + left * detectionRange);
//         Gizmos.DrawLine(eyePoint.position, eyePoint.position + right * detectionRange);
//     }
// }


using System.Collections;
using UnityEngine;

public class SwordfishAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Detection (Cone)")]
    public float detectionRange = 30f;
    public float viewAngle = 90f; // cone angle

    [Header("Movement")]
    public float swimSpeed = 6f;
    public float dashSpeed = 25f;
    public float turnSpeed = 5f;

    [Header("Attack")]
    public float attackCooldown = 4f;
    public float aimDuration = 1f;

    [Header("Rest")]
    public float retreatSpeed = 8f;
    public float retreatDistance = 30f;
    public float restDuration = 5f;

    private float lastAttackTime;

    private Vector3 moveDirection;
    private Vector3 lockedPosition;

    private float stateTimer;

    private enum State { Swim, Aim, Dash, Rest }
    private State currentState;

    void Start()
    {
        currentState = State.Swim;
        PickRandomDirection();
    }

    void Update()
    {
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

    // ================= SWIM =================
    void Swim()
    {
        Move(moveDirection, swimSpeed);

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0)
        {
            PickRandomDirection();
        }
    }

    void PickRandomDirection()
    {
        moveDirection = new Vector3(
            Random.Range(-1f, 1f),
            0,
            Random.Range(-1f, 1f)
        ).normalized;

        stateTimer = Random.Range(2f, 4f);
    }

    // ================= CONE DETECTION =================
    void DetectPlayer()
    {
        if (player == null) return;

        Vector3 dirToPlayer = (player.position - transform.position);
        float distance = dirToPlayer.magnitude;

        if (distance > detectionRange)
            return;

        dirToPlayer.Normalize();

        // Angle check using forward (Z-axis)
        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        if (angle < viewAngle / 2f)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                currentState = State.Aim;
                stateTimer = aimDuration;

                lockedPosition = player.position;
            }
        }
    }

    // ================= AIM =================
    void Aim()
    {
        stateTimer -= Time.deltaTime;

        Vector3 dir = (player.position - transform.position).normalized;
        RotateTowards(dir);

        if (stateTimer <= 0)
        {
            currentState = State.Dash;
        }
    }

   void Dash()
{
    Vector3 dir = (lockedPosition - transform.position).normalized;
    Move(dir, dashSpeed);

    // Check distance to boat
    if (player != null)
    {
        float distanceToBoat = Vector3.Distance(transform.position, player.position);
        if (distanceToBoat < 1.5f) // adjust hit range
        {
            Debug.Log("Fish hit the boat!");
            // Optional: start rest immediately after hitting
            currentState = State.Rest;
            stateTimer = restDuration;
        }
    }

    float dist = Vector3.Distance(transform.position, lockedPosition);
    if (dist < 1.5f)
    {
        currentState = State.Rest;
        stateTimer = restDuration;
    }
}

    // ================= REST =================
    void Rest()
    {
        stateTimer -= Time.deltaTime;

        Vector3 awayDir = (transform.position - player.position).normalized;
        awayDir.y = 0;
        Move(awayDir, retreatSpeed);

        if (stateTimer <= 0)
        {
            currentState = State.Swim;
            lastAttackTime = Time.time;
            PickRandomDirection();
        }
    }

    // ================= MOVEMENT =================
    void Move(Vector3 dir, float speed)
    {
        if (dir == Vector3.zero) return;

        transform.position += dir * speed * Time.deltaTime;
        RotateTowards(dir);
    }

    void RotateTowards(Vector3 dir)
    {
        if (dir == Vector3.zero) return;

        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnSpeed * Time.deltaTime);
    }

    // ================= DEBUG =================
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