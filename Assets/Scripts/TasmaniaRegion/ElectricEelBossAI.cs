using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class ElectricEelBossAI : BossAI
{


    private enum BossState
    {
        Patrol,
        Chase,
        Attack,
        Retreat
    }

    private BossState currentState = BossState.Patrol;



    public Transform[] patroPoints;
    private NavMeshAgent agent;
    private int currentLoc;

    //detect ship
    public float detectionRadius = 10f;
    public LayerMask shipLayer;
    private Transform shipl;
    private bool wasChasing = false;


    //lighting strike
    [Header("Attack Settings")]
    public float attackRange = 3f;
    public float attackCooldown = 5f;
    public GameObject lightningPrefab;

    private float lastAttackTime;
    private bool isAttacking = false; //testing



    [Header("Retreat State")]
    public float retreatRange = 50f;
    public float retreatCooldown = 5f;
    public bool isRetreating = false;


    //testing cause ai keeps tracking and returning to player while retreating
    //works then dosen't work so very confusing

    private float ignorePlayerUntil = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        if (patroPoints.Length > 0)
        {
            agent.SetDestination(patroPoints[currentLoc].position); //set where its going to pathfind too
        }
    }
    //change everything to state switching HHHHH
    // Update is called once per frame
    protected override void Update()
    {


        base.Update(); //call from base class to update health bar and all t hat
      

        switch (currentState)
        {
            case BossState.Patrol:
                HandlePatrol();
                break;

            case BossState.Chase:
                HandleChase();
                break;

            case BossState.Attack:
                break; 

            case BossState.Retreat:
                break; 
        }

        //Debug.Log($"shipl: {shipl}, isRetreating: {isRetreating}, agent: {agent}"); //testing
        //EncounterPlayer();
        ////if (!isRetreating && Time.time > ignorePlayerUntil)
        ////{
        ////    EncounterPlayer();
        ////}
        //if (isRetreating)
        //    return;

        ////if (Time.time > ignorePlayerUntil) //hope
        ////{
        ////    EncounterPlayer();
        ////}
        ////else
        ////{
        ////    shipl = null;
        ////}
        


        //if (shipl != null)
        //{
          
        //    float distance = Vector3.Distance(transform.position, shipl.position);

        //    if (distance > attackRange)
        //    {
        //        agent.isStopped = false;
               
        //        agent.SetDestination(shipl.position);
        //    }
        //    else
        //    {
        //        ////attack
        //        //wasChasing = true;

        //        //if (Time.time >= lastAttackTime + attackCooldown)
        //        //{
        //        //    StartCoroutine(Attack());
        //        //}
        //        if (!isAttacking && Time.time >= lastAttackTime + attackCooldown)
        //        {
        //            StartCoroutine(Attack());
        //        }
        //    }
        //}
        //else
        //{
        //    //if (!agent.pathPending && agent.remainingDistance < 0.5)
        //    //{
        //    //    GoToNextPoint(); //subject to change
        //    //}
        //    if (wasChasing)
        //    {
        //        wasChasing = false;
        //        GoToNextPoint(); //restart patrol after chasing
        //    }

        //    // Normal patrol
        //    if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        //    {
        //        GoToNextPoint();
        //    }
        //}
       
       


    }

    void HandlePatrol()
    {
        EncounterPlayer();

        if (shipl != null)
        {
          
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            GoToNextPoint();
        }
    }

    void HandleChase()
    {

        //EncounterPlayer();
        if (shipl == null)
        {
            currentState = BossState.Patrol;
            return;
        }
        agent.isStopped = false;
        float distance = Vector3.Distance(transform.position, shipl.position);

        if (distance > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(shipl.position);
        }
        else
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                currentState = BossState.Attack;
                StartCoroutine(Attack());
            }
        }



    }

    public void EncounterPlayer()
    {


        if (Time.time < ignorePlayerUntil) //testing
            return;

        if (currentState == BossState.Retreat || isAttacking) //testing
            return;
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, shipLayer);
        if (hits.Length > 0)
        {
            shipl = hits[0].transform;
            wasChasing = true;
            currentState = BossState.Chase;
            Debug.Log("Player detected: " + shipl.name);
        }
        else
        {
            //shipl = null; //if move out then continuing patrolling
            //clear when far
            if (shipl != null && Vector3.Distance(transform.position, shipl.position) > detectionRadius + 2f)
            {
                shipl = null;
                currentState = BossState.Patrol;
            }
        }

        //testing for now
    }
    public void GoToNextPoint()
    {
        if (patroPoints.Length == 0)
            return;

        //currentLoc = (currentLoc + 1) % patroPoints.Length;
        currentLoc = Random.Range(0, patroPoints.Length);
        agent.SetDestination(patroPoints[currentLoc].position);
    }

    //attack test
    IEnumerator Attack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
       // agent.isStopped = true;
        int boltCount = Random.Range(2, 6); //change later see how

        for (int i = 0; i < boltCount; i++)
        {
            SpawnLighting();
            yield return new WaitForSeconds(0.3f);
        }

        StartCoroutine(Retreat());
        currentState = BossState.Retreat;
        isAttacking = false;
    }

    IEnumerator Retreat()
    {

        if (shipl == null)
        {
            currentState = BossState.Patrol;
            yield break;
        }
        isRetreating = true;
        agent.isStopped = false;

        Vector3 directionAway = (transform.position - shipl.position).normalized;
        Vector3 retreatPoint = transform.position + directionAway * retreatRange; //retreating distance away from player

        NavMeshHit hit;
        if (NavMesh.SamplePosition(retreatPoint, out hit, 5f, NavMesh.AllAreas)) //find valid position in mes
        {
            agent.SetDestination(hit.position);
        }

        // yield return new WaitForSeconds(retreatCooldown);

        //retreat then only wait a while before going back
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f); //wait bit more for now lol
        isRetreating = false;
        isAttacking = false;
        ignorePlayerUntil = Time.time + 3f; //fix should pls work
        shipl = null;
        currentState = BossState.Patrol;
        // Resume patrol cleanly
        //GoToNextPoint();
    }
    public void SpawnLighting()
    {
        Vector3 strikePosition;

      //50% hit ship
        if (Random.value < 0.5f)
        {
            strikePosition = shipl.position;
        }
        else
        {
            //randomly around ship
            Vector2 randomCircle = Random.insideUnitCircle * 3f;
            strikePosition = shipl.position + new Vector3(randomCircle.x, 0, randomCircle.y);
        }

        Instantiate(lightningPrefab, strikePosition + Vector3.up * 10f, Quaternion.identity);
    }
}
