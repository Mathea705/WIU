using UnityEngine;
using UnityEngine.LowLevelPhysics2D;
using UnityEngine.Splines.ExtrusionShapes;

public class AI_Stingray : MonoBehaviour
{
    [SerializeField] private GameObject PlayerShip;

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
    private float pivotSpeed;
    private float aggroRange;
    private float wanderTargetRange;

    private float aggroTimer;
    private const float aggroTimerMAX = 2.0f;

    private bool randomAttack;

    private float dir;
    private float rad;

    private void Start()
    {
        speed = 10.0f;
        pivotSpeed = 30.0f;
        dir = 0.0f;
        aggroRange = 50.0f;

        wanderTargetRange = 100.0f;

        currentState = State.WANDER;

        aggroTimer = aggroTimerMAX;
    }

    private void Update()
    {
        CheckStateTransitions();
        RunStateCode();
        ParseInputs();
        ClampDir();
        HandleTransform();

        //Debug.Log(currentState);

        
    }

    void OnDrawGizmos()
    {
        
        Gizmos.color = Color.red;

        
        Gizmos.DrawSphere(targetPos, 10);
    }

    private void CheckStateTransitions()
    {
        switch (currentState)
        {
            case State.WANDER:
                if ((PlayerShip.transform.position - transform.position).magnitude <= aggroRange)
                {
                    currentState = State.AGGRO;
                }
                break;
            case State.AGGRO:
                if (aggroTimer < 0.0f)
                {
                    aggroTimer = aggroTimerMAX;

                    randomAttack = Random.Range(1, 3) == 1;
                    currentState = randomAttack ? State.SPRING : State.STING;
                }
                break;
            case State.SPRING:
                currentState = State.WANDER;
                break;
            case State.STING:
                currentState = State.WANDER;
                break;
            case State.RECUPERATE:
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

        Debug.Log(CTRL_left || CTRL_right);
    }

    private void RunStateCode()
    {
        switch (currentState)
        {
            case State.WANDER:
                if ((targetPos - transform.position).magnitude <= wanderTargetRange)
                {
                    targetPos = new Vector3(
                        PlayerShip.transform.position.x + Random.Range(-wanderTargetRange, wanderTargetRange),
                        0,
                        PlayerShip.transform.position.z + Random.Range(-wanderTargetRange, wanderTargetRange));
                }
                break;
            case State.AGGRO:
                if (aggroTimer >= 0.0f)
                {
                   aggroTimer -= Time.deltaTime;
                }
                break;
            case State.SPRING:
                break;
            case State.STING:
                break;
            case State.RECUPERATE:
                break;
            default:
                break;

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
