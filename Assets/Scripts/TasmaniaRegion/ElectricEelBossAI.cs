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

    private float ignorePlayerUntil = 0;
    protected override void Start()
    {
        base.Start();
        currentState = BossState.Patrol;
    shipl = null;
    isAttacking = false;
    isRetreating = false;
    ignorePlayerUntil = 0f;
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
        //EncounterPlayer();

        //if (shipl != null)
        //{

        //    return;
        //}

        //if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        //{
        //    GoToNextPoint();
        //}

        agent.isStopped = false;

        EncounterPlayer();

        if (shipl != null)
            return;

        if (!agent.hasPath || agent.remainingDistance <= agent.stoppingDistance)
        {
            GoToNextPoint();
        }
    }

    void HandleChase()
    {
        Debug.Log("Chasing: " + shipl);
        //EncounterPlayer();
        if (shipl == null)
        {
            currentState = BossState.Patrol;
            return;
        }
        // agent.isStopped = false;
        float distance = Vector3.Distance(transform.position, shipl.position);

        //if (distance > attackRange)
        //{
        //    agent.isStopped = false;
        //    agent.SetDestination(shipl.position);
        //}
        //else
        //{
        //    // Stop moving and attack
        //    agent.isStopped = true;

        //    if (!isAttacking && Time.time >= lastAttackTime + attackCooldown)
        //    {
        //        StartCoroutine(Attack());
        //    }
        //}

        float attackBuffer = 5f; 

        if (distance > attackRange + attackBuffer)
        {
           //buffer and range
            agent.isStopped = false;
            agent.stoppingDistance = attackRange; 
            agent.SetDestination(shipl.position);
            if (Random.value < 0.3f) //possible rare attack
            {
                StartCoroutine(SpecialLightningAttack());
            }
        }
        else
        {
            //if close then attack
            agent.isStopped = true;

            //if (!isAttacking && Time.time >= lastAttackTime + attackCooldown)
            //{
            //    StartCoroutine(Attack());
            //}
            if (!isAttacking && Time.time >= lastAttackTime + attackCooldown)
            {
                StartCoroutine(Attack()); 
               
                isAttacking = true;
            }
        }

    }

    public void EncounterPlayer()
    {


        if (Time.time < ignorePlayerUntil) //testing
            return;

        if (currentState == BossState.Retreat || isAttacking) //testing
            return;
        //Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, shipLayer);
        //if (hits.Length > 0)
        //{
        //    shipl = hits[0].transform;
        //    wasChasing = true;
        //    currentState = BossState.Chase;
        //    Debug.Log("Player detected: " + shipl.name);
        //}
        //else
        //{
        //    //shipl = null; //if move out then continuing patrolling
        //    //clear when far
        //    if (shipl != null && Vector3.Distance(transform.position, shipl.position) > detectionRadius + 5f)
        //    {
        //        shipl = null;
        //        currentState = BossState.Patrol;
        //    }
        //}
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius); //change to tag now
        shipl = null; 

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Ship")) // check for tag instead of layer
            {
                shipl = hit.transform;
                wasChasing = true;
                currentState = BossState.Chase;
                Debug.Log("Player detected: " + shipl.name);
                break; 
            }
        }

        //else return to normal

        if (shipl != null && Vector3.Distance(transform.position, shipl.position) > detectionRadius + 5f)
        {
            shipl = null;
            currentState = BossState.Patrol;
        }
        //testing for now
    }
    public void GoToNextPoint()
    {
        if (patroPoints.Length == 0)
            return;

        int nextLoc = currentLoc;

        //issue is if same point and choose same point, stuck
        while (nextLoc == currentLoc && patroPoints.Length > 1)
        {
            nextLoc = Random.Range(0, patroPoints.Length);
        }

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

        //Instantiate(lightningPrefab, strikePosition + Vector3.up * 10f, Quaternion.identity);
        GameObject bolt = Instantiate(lightningPrefab, strikePosition + Vector3.up * 10f, Quaternion.identity);
        lightingAttack la = bolt.GetComponent<lightingAttack>();
        if (la != null && shipl != null)
        {
            la.target = shipl.gameObject; //target of ai is target here!
        }
    }

    IEnumerator SpecialLightningAttack()
    {
        int boltCount = Random.Range(4, 7); 

        for (int i = 0; i < boltCount; i++)
        {
            //spawn randomly around eel
            Vector2 randomCircle = Random.insideUnitCircle * 5f; //
            Vector3 spawnPos = transform.position + new Vector3(randomCircle.x, 10f, randomCircle.y);

            GameObject bolt = Instantiate(lightningPrefab, spawnPos, Quaternion.identity);

            // assign the player as the target
            lightingAttack la = bolt.GetComponent<lightingAttack>();
            if (la != null)
                la.target = shipl.gameObject; 

            yield return new WaitForSeconds(0.1f); //
        }

        StartCoroutine(Retreat());
        currentState = BossState.Retreat;
        isAttacking = false;
    }
}


//testing cause ai keeps tracking and returning to player while retreating
//works then dosen't work so very confusing

// private float ignorePlayerUntil = 0f;

//    protected override void Start()
//    {
//        base.Start();

//        agent = GetComponent<NavMeshAgent>();
//        if (agent == null)
//        {
//            Debug.LogError("NavMeshAgent not found! Check GameObject structure.");
//        }
//        //if (!agent.isOnNavMesh)
//        //{
//        //    Debug.LogError("Agent is NOT on NavMesh!"); //
//        //    return;
//        //}

//        currentState = BossState.Patrol;
//        GoToNextPatrolPoint();
//    }

//    protected override void Update()
//    {
//        base.Update();

//        if (!agent.isOnNavMesh) 
//            return;

//        switch (currentState)
//        {
//            case BossState.Patrol:
//                UpdatePatrol();
//                break;

//            case BossState.Chase:
//                UpdateChase();
//                break;
//        }
//    }


//    void UpdatePatrol()
//    {
//        DetectPlayer();

//        if (shipl != null)
//        {
//            ChangeState(BossState.Chase);
//            return;
//        }

//        if (!agent.hasPath || agent.remainingDistance <= agent.stoppingDistance)
//        {
//            GoToNextPatrolPoint();
//        }
//    }

//    void GoToNextPatrolPoint()
//    {
//        if (patrolPoints.Length == 0) return;

//        currentLoc = Random.Range(0, patrolPoints.Length);
//        agent.isStopped = false;
//        agent.SetDestination(patrolPoints[currentLoc].position);
//    }


//    void UpdateChase()
//    {
//        if (shipl == null)
//        {
//            ChangeState(BossState.Patrol);
//            return;
//        }

//        agent.isStopped = false;
//        agent.SetDestination(shipl.position);

//        float distance = Vector3.Distance(transform.position, shipl.position);

//        if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown && !isAttacking)
//        {
//            StartCoroutine(AttackRoutine());
//        }

//        // Lose player if too far
//        if (distance > detectionRadius * 1.5f)
//        {
//            shipl = null;
//            ChangeState(BossState.Patrol);
//        }
//    }


//    IEnumerator AttackRoutine()
//    {
//        isAttacking = true;
//        ChangeState(BossState.Attack);

//        lastAttackTime = Time.time;
//        agent.isStopped = true;

//        int bolts = Random.Range(2, 6);

//        for (int i = 0; i < bolts; i++)
//        {
//            SpawnLightning();
//            yield return new WaitForSeconds(0.3f);
//        }

//        yield return RetreatRoutine();

//        isAttacking = false;
//        ChangeState(BossState.Patrol);
//    }

//    IEnumerator RetreatRoutine()
//    {
//        if (shipl == null) 
//            yield break;

//        ChangeState(BossState.Retreat);

//        Vector3 dir = (transform.position - shipl.position).normalized;
//        Vector3 retreatPos = transform.position + dir * retreatRange;

//        NavMeshHit hit;
//        if (NavMesh.SamplePosition(retreatPos, out hit, 5f, NavMesh.AllAreas))
//        {
//            agent.isStopped = false;
//            agent.SetDestination(hit.position);

//            while (agent.pathPending)
//                yield return null;

//            while (agent.hasPath && agent.remainingDistance > agent.stoppingDistance)
//                yield return null;
//        }

//        yield return new WaitForSeconds(retreatCooldown);

//        shipl = null;
//    }

//    // =========================
//    // DETECTION
//    // =========================
//    void DetectPlayer()
//    {
//        if (isAttacking) return;

//        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, shipLayer);

//        if (hits.Length > 0)
//        {
//            shipl = hits[0].transform;
//        }
//    }

//    // =========================
//    // LIGHTNING
//    // =========================
//    void SpawnLightning()
//    {
//        if (shipl == null) return;

//        Vector3 strikePos;

//        if (Random.value < 0.5f)
//        {
//            strikePos = shipl.position;
//        }
//        else
//        {
//            Vector2 rand = Random.insideUnitCircle * 3f;
//            strikePos = shipl.position + new Vector3(rand.x, 0, rand.y);
//        }

//        Instantiate(lightningPrefab, strikePos + Vector3.up * 10f, Quaternion.identity);
//    }

//    void ChangeState(BossState newState)
//    {
//        currentState = newState;
//    }
//}

//something broke so had to redo sadly
//Start is called once before the first execution of Update after the MonoBehaviour is created

