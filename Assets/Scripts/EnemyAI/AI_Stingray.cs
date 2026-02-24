using UnityEngine;
using UnityEngine.LowLevelPhysics2D;
using UnityEngine.Splines.ExtrusionShapes;

public class AI_Stingray : BossAI
{
    private enum State
    {
        WANDER,
        AGGRO,
        SPRING,
        STING,
        RECUPERATE,
    }

    private State currentState;

    private Vector3 targetPos;

    private bool CTRL_left;
    private bool CTRL_right;
    private bool CTRL_jump;
    private bool CTRL_surface;

    private float speed;

    private const float SPEEDDEFAULT = 10.0f;
    private const float SPEEDFAST = 20.0f;
    private const float SPEEDSLOW = 5.0f;

    private float pivotSpeed;

    private const float PIVOTDEFAULT = 30.0f;
    private const float PIVOTFAST = 90.0f;

    private float aggroRange;
    private float springJumpAngleThreshold;
    private float wanderTargetRange;
    private float wanderTargetDetectionRange;

    private const float JUMPDISENGAGEDIST = 10.0f;

    private float aggroTimer;
    private const float AGGROTIMERMAX = 2.0f;

    private float springTimer;
    private const float SPRINGTIMERMAX = 2.0f;

    private bool randomAttack;

    private const int dmg_lowerBound = 5;
    private const int dmg_upperBound = 10;

    private bool hasAttemptedSpring;

    private float dir;
    private float rad;

    private const float ELEVATIONSURFACE = 0.0f;
    private const float ELEVATIONSUBMERGED = -8.0f;
    private const float ELEVATIONJUMP = 3.0f;

    private const float SPEEDLERP = 5.0f;

    private Vector3 DEBUGSPHEREPOS;

    private void Start()
    {
        speed = SPEEDDEFAULT;
        pivotSpeed = PIVOTDEFAULT;
        dir = 0.0f;
        aggroRange = 50.0f;

        springJumpAngleThreshold = 0.9f;

        wanderTargetRange = 50.0f;
        wanderTargetDetectionRange = 20.0f;

        currentState = State.WANDER;

        aggroTimer = AGGROTIMERMAX;
        springTimer = SPRINGTIMERMAX;

        shipObject = GameObject.FindWithTag("Ship");
        shipHealth = shipObject.GetComponent<HealthSystem>();
    }

    private void Update()
    {
        CheckStateTransitions();
        HandleBossSpeedsAndElevations();
        RunStateCode();
        ParseInputs();
        ClampDir();
        HandleTransform();

        Debug.Log(currentState);
        //Debug.Log(shipHealth);




    }

    void OnDrawGizmos()
    {

        Gizmos.color = Color.red;

        DEBUGSPHEREPOS = Vector3.Lerp(DEBUGSPHEREPOS, targetPos, Time.deltaTime * SPEEDLERP);

        Gizmos.DrawSphere(DEBUGSPHEREPOS, 2);
    }

    private void CheckStateTransitions()
    {
        switch (currentState)
        {
            case State.WANDER:
                if ((shipObject.transform.position - transform.position).magnitude <= aggroRange)
                {
                    currentState = State.AGGRO;
                }
                break;
            case State.AGGRO:
                if (aggroTimer < 0.0f)
                {
                    aggroTimer = AGGROTIMERMAX;

                    randomAttack = Random.Range(1, 3) == 1;
                    currentState = randomAttack ? State.SPRING : State.STING;
                }
                break;
            case State.SPRING:
                //currentState = State.WANDER;
                if ((shipObject.transform.position - transform.position).magnitude < JUMPDISENGAGEDIST && CTRL_jump)
                {
                    hasAttemptedSpring = true;
                }
                else if ((shipObject.transform.position - transform.position).magnitude >= JUMPDISENGAGEDIST && CTRL_jump && hasAttemptedSpring)
                {
                    currentState = State.RECUPERATE;
                }

                springTimer -= Time.deltaTime;
                if (springTimer <= 0f)
                {
                    springTimer = SPRINGTIMERMAX;
                    currentState = State.RECUPERATE;
                }
                break;
            case State.STING:
                currentState = State.RECUPERATE;
                break;
            case State.RECUPERATE:
                if ((shipObject.transform.position - transform.position).magnitude >= aggroRange)
                {
                    currentState = State.WANDER;
                }
                break;
            default:
                break;

        }
        rad = dir * Mathf.Deg2Rad;
        Vector3 forward = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad));
        Vector3 toTarget = (targetPos - transform.position).normalized;

        float cross = (forward.x * toTarget.z) - (forward.z * toTarget.x);
        CTRL_left = cross < 0;
        CTRL_right = cross > 0;

        
    }

    private void HandleBossSpeedsAndElevations()
    {
        switch (currentState)
        {
            case State.WANDER:
                speed = Mathf.Lerp(speed, SPEEDDEFAULT, Time.deltaTime * SPEEDLERP);
                pivotSpeed = PIVOTDEFAULT;

                transform.position = new Vector3(
                        transform.position.x,
                        Mathf.Lerp(transform.position.y, ELEVATIONSURFACE, Time.deltaTime * SPEEDLERP),
                        transform.position.z);
                break;
            case State.AGGRO:
                speed = Mathf.Lerp(speed, SPEEDSLOW, Time.deltaTime * SPEEDLERP);
                pivotSpeed = PIVOTFAST;

                transform.position = new Vector3(
                        transform.position.x,
                        Mathf.Lerp(transform.position.y, ELEVATIONSURFACE, Time.deltaTime * SPEEDLERP),
                        transform.position.z);
                break;
            case State.SPRING:
                if (!CTRL_jump)
                {
                    speed = Mathf.Lerp(speed, SPEEDSLOW, Time.deltaTime * SPEEDLERP);
                    pivotSpeed = PIVOTFAST;
                }
                else
                {
                    speed = SPEEDFAST;
                    pivotSpeed = PIVOTDEFAULT;

                    transform.position = new Vector3(
                        transform.position.x,
                        Mathf.Lerp(transform.position.y, ELEVATIONJUMP, Time.deltaTime * SPEEDLERP),
                        transform.position.z);
                }

                break;
            case State.STING:

                break;
            case State.RECUPERATE:
                speed = Mathf.Lerp(speed, SPEEDFAST, Time.deltaTime * SPEEDLERP);
                pivotSpeed = PIVOTDEFAULT;

                transform.position = new Vector3(
                        transform.position.x,
                        Mathf.Lerp(transform.position.y, ELEVATIONSUBMERGED, Time.deltaTime * SPEEDLERP),
                        transform.position.z);
                break;
        }
    }

    private void RunStateCode()
    {
        switch (currentState)
        {
            case State.WANDER:

                if ((targetPos - transform.position).magnitude <= wanderTargetDetectionRange)
                {
                    targetPos = new Vector3(
                        shipObject.transform.position.x + Random.Range(-wanderTargetRange, wanderTargetRange),
                        0,
                        shipObject.transform.position.z + Random.Range(-wanderTargetRange, wanderTargetRange));
                }

                break;
            case State.AGGRO:

                if (aggroTimer >= 0.0f)
                {
                    aggroTimer -= Time.deltaTime;
                }

                break;
            case State.SPRING:
                targetPos = shipObject.transform.position;
                if (!CTRL_jump)
                {
                    rad = dir * Mathf.Deg2Rad;
                    Vector3 forward = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad));
                    Vector3 toTarget = (targetPos - transform.position).normalized;
                    float dot = (forward.x * toTarget.x) + (forward.z * toTarget.z);
                    float similarity = dot / (forward.magnitude * toTarget.magnitude);

                    if (similarity >= springJumpAngleThreshold)
                    {
                        CTRL_jump = true;
                    }
                }
                

                break;
            case State.STING:
                break;
            case State.RECUPERATE:
                hasAttemptedSpring = false;
                CTRL_jump = false;

                Vector3 awayFromPlayer = (transform.position - shipObject.transform.position).normalized;

                targetPos = transform.position + awayFromPlayer * 50f;
                targetPos.y = ELEVATIONSURFACE;
                break;
            default:
                break;

        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (currentState == State.SPRING)
        {
            if (other.gameObject == shipObject)
            {
                DealDamageToShip(Random.Range(dmg_lowerBound, dmg_upperBound + 1));

                dir += 180;
                currentState = State.RECUPERATE;
            }
        }
    }

    private void ParseInputs()
    {
        dir += CTRL_left && !CTRL_right ? -pivotSpeed * Time.deltaTime : !CTRL_left && CTRL_right ? pivotSpeed * Time.deltaTime : 0.0f;
    }

    private void ClampDir()
    {
        dir = dir > 360f ? dir - 360f : dir < 0f ? dir + 360f : dir;
    }

    private void HandleTransform()
    {
        rad = dir * Mathf.Deg2Rad;
        Vector3 dirVec = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad));
        dirVec.Normalize();

        transform.position += new Vector3(dirVec.x * speed * Time.deltaTime, 0, dirVec.z * speed * Time.deltaTime);
    }

    
}