using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

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

    private void Start()
    {
        speed = 5.0f;
        pivotSpeed = 5.0f;
        dir = 0.0f;
        aggroRange = 50.0f;

        wanderTargetRange = 200.0f;

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

        Debug.Log(currentState);
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
        float myCross = (Mathf.Cos(dir) * targetPos.y - transform.position.y) - (Mathf.Sin(dir) * targetPos.x - transform.position.x);
        CTRL_left = myCross < 0;
        CTRL_right = myCross > 0;
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
        Vector3 dirVec = new Vector3(Mathf.Cos(dir), 0, Mathf.Sin(dir));
        dirVec.Normalize();

        transform.position += new Vector3(dirVec.x * speed * Time.deltaTime, 0, dirVec.z * speed * Time.deltaTime);
    }
}
