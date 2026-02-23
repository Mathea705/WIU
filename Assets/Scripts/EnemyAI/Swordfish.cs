using System.Collections;
using UnityEngine;

public class Swordfish : MonoBehaviour
{
    [Header("References")]
    public Transform player;   // optional
    public Transform boat;     // main target

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

    [Header("Rest")]
    public float retreatSpeed = 8f;
    public float restDuration = 5f;

    [Header("Hit Detection")]
    public float hitDistance = 2f; // collision distance

    private float lastAttackTime;
    private Vector3 moveDirection;
    private Vector3 lockedPosition;
    private float stateTimer;

    private enum State { Swim, Aim, Dash, Rest }
    private State currentState;

    private bool hasHitBoatThisDash = false;

    // ================= START =================
    void Start()
    {
        currentState = State.Swim;
        PickRandomDirection();
    }

    // ================= UPDATE =================
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
            PickRandomDirection();
    }

    void PickRandomDirection()
    {
        moveDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        stateTimer = Random.Range(2f, 4f);
    }

    // ================= DETECTION =================
    void DetectPlayer()
    {
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
        }
    }

    // ================= AIM =================
    void Aim()
    {
        if (boat == null) return;

        stateTimer -= Time.deltaTime;

        Vector3 dir = (boat.position - transform.position).normalized;
        RotateTowards(dir);

        if (stateTimer <= 0)
            currentState = State.Dash;
    }

    // ================= DASH =================
    void Dash()
    {
        Vector3 dir = (lockedPosition - transform.position).normalized;

        // Store previous position (for anti-skip detection)
        Vector3 previousPosition = transform.position;

        Move(dir, dashSpeed);

       
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
        }
    }

    // ================= REST =================
    void Rest()
    {
        if (boat == null) return;

        stateTimer -= Time.deltaTime;

        Vector3 awayDir = (transform.position - boat.position).normalized;
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
        // Detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Cone lines (Z-forward)
        Vector3 left = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + left * detectionRange);
        Gizmos.DrawLine(transform.position, transform.position + right * detectionRange);
    }
}

// using UnityEngine;

// public class Swordfish : MonoBehaviour
// {
//     [Header("References")]
//     public Transform boat;

//     [Header("Detection (Cone - Z Forward)")]
//     public float detectionRange = 30f;
//     public float viewAngle = 90f;

//     [Header("Movement")]
//     public float swimSpeed = 6f;
//     public float dashSpeed = 25f;
//     public float turnSpeed = 5f;

//     [Header("Attack")]
//     public float attackCooldown = 4f;
//     public float aimDuration = 1f;

//     [Header("Rest")]
//     public float retreatSpeed = 8f;
//     public float restDuration = 5f;

//     private float lastAttackTime;
//     private Vector3 moveDirection;
//     private Vector3 lockedPosition;
//     private float stateTimer;

//     private enum State { Swim, Aim, Dash, Rest }
//     private State currentState;

//     private bool hasHitBoatThisDash = false;

//     // ================= START =================
//     void Start()
//     {
//         currentState = State.Swim;
//         PickRandomDirection();
//     }

//     // ================= UPDATE =================
//     void Update()
//     {
//         switch (currentState)
//         {
//             case State.Swim:
//                 Swim();
//                 DetectBoat();
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

//     // ================= SWIM =================
//     void Swim()
//     {
//         Move(moveDirection, swimSpeed);

//         stateTimer -= Time.deltaTime;
//         if (stateTimer <= 0)
//             PickRandomDirection();
//     }

//     void PickRandomDirection()
//     {
//         moveDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
//         stateTimer = Random.Range(2f, 4f);
//     }

//     // ================= DETECTION =================
//     void DetectBoat()
//     {
//         if (boat == null) return;

//         Vector3 dirToBoat = boat.position - transform.position;
//         float distance = dirToBoat.magnitude;

//         if (distance > detectionRange) return;

//         dirToBoat.Normalize();
//         float angle = Vector3.Angle(transform.forward, dirToBoat);

//         if (angle < viewAngle / 2f && Time.time >= lastAttackTime + attackCooldown)
//         {
//             currentState = State.Aim;
//             stateTimer = aimDuration;
//             lockedPosition = boat.position;
//             hasHitBoatThisDash = false;
//         }
//     }

//     // ================= AIM =================
//     void Aim()
//     {
//         if (boat == null) return;

//         stateTimer -= Time.deltaTime;

//         Vector3 dir = (boat.position - transform.position).normalized;
//         RotateTowards(dir);

//         if (stateTimer <= 0)
//             currentState = State.Dash;
//     }

//     // ================= DASH =================
//     void Dash()
//     {
//         Vector3 dir = (lockedPosition - transform.position).normalized;
//         Move(dir, dashSpeed);

//         float dist = Vector3.Distance(transform.position, lockedPosition);
//         if (dist < 1.5f)
//         {
//             currentState = State.Rest;
//             stateTimer = restDuration;
//         }
//     }

//     // ================= REST =================
//     void Rest()
//     {
//         if (boat == null) return;

//         stateTimer -= Time.deltaTime;

//         Vector3 awayDir = (transform.position - boat.position).normalized;
//         Move(awayDir, retreatSpeed);

//         if (stateTimer <= 0)
//         {
//             currentState = State.Swim;
//             lastAttackTime = Time.time;
//             PickRandomDirection();
//         }
//     }

//     // ================= MOVEMENT =================
//     void Move(Vector3 dir, float speed)
//     {
//         if (dir == Vector3.zero) return;

//         transform.position += dir * speed * Time.deltaTime;
//         RotateTowards(dir);
//     }

//     void RotateTowards(Vector3 dir)
//     {
//         if (dir == Vector3.zero) return;

//         Quaternion rot = Quaternion.LookRotation(dir);
//         transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnSpeed * Time.deltaTime);
//     }

//     // ================= TRIGGER HIT =================
//     private void OnTriggerEnter(Collider other)
//     {
//         if (!hasHitBoatThisDash && currentState == State.Dash)
//         {
//             if (other.transform == boat)
//             {
//                 Debug.Log("collide");

//                 hasHitBoatThisDash = true;

//                 currentState = State.Rest;
//                 stateTimer = restDuration;
//             }
//         }
//     }

//     // ================= DEBUG GIZMOS =================
//     void OnDrawGizmos()
//     {
//         Gizmos.color = Color.yellow;
//         Gizmos.DrawWireSphere(transform.position, detectionRange);

//         Vector3 left = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
//         Vector3 right = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;

//         Gizmos.color = Color.red;
//         Gizmos.DrawLine(transform.position, transform.position + left * detectionRange);
//         Gizmos.DrawLine(transform.position, transform.position + right * detectionRange);
//     }
// }