using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class AI_SeaLeviathan : BossAI
{
    private enum State
    {
        SWIM,
        SLAM,
        SUBMERGE,
        RESURFACE,
    }

    private State currentState;

    [SerializeField] private float orbitRadius   = 40f;
    [SerializeField] private float orbitSpeed    = 20f;
    [SerializeField] private float swimSpeed     = 4f;
    [SerializeField] private float turnSpeed     = 2f;
    [SerializeField] private float swimDepth     = -5f;
    [SerializeField] private float swimDuration  = 20f;

    [SerializeField] private float slamAimDuration  = 1.5f;   
    [SerializeField] private float slamAimTurnSpeed = 120f;   
    [SerializeField] private float slamArcDuration  = 2.0f;   
    [SerializeField] private float slamRiseHeight   = 20f;   
    [SerializeField] private float slamDamage        = 40f;
    [SerializeField] private GameObject              splashPrefab;
    [SerializeField] private float                   splashScale = 1f;
    [SerializeField] private CinemachineImpulseSource impulseSource;


    [SerializeField] private float submergeDepth      = -30f;  // how deep it dives
    [SerializeField] private float submergeDiveTime   = 2f;    // seconds to reach depth
    [SerializeField] private float submergeDuration   = 8f;    // time spent underwater
    [SerializeField] private float submergeRiseTime   = 2f;    // seconds to resurface
    [SerializeField] private GameObject whirlpoolPrefab;
    [SerializeField] private int        whirlpoolCount     = 5;
    [SerializeField] private float      whirlpoolMinRadius = 4f;
    [SerializeField] private float      whirlpoolMaxRadius = 12f;

    private float _orbitAngle;
    private float _swimTimer;

    protected override void Start()
    {
        base.Start();

        if (shipObject != null)
        {
            Vector3 offset = transform.position - shipObject.transform.position;
            _orbitAngle = Mathf.Atan2(offset.z, offset.x) * Mathf.Rad2Deg;
        }

        currentState = State.SWIM;
        _swimTimer   = swimDuration;
    }

    protected override void Update()
    {
        base.Update();

        CheckStateTransitions();
        RunStateCode();

        // Debug.Log(currentState);
    }

    private void CheckStateTransitions()
    {
        switch (currentState)
        {
            case State.SWIM:
                _swimTimer -= Time.deltaTime;
                if (_swimTimer <= 0f)
                {
                    if (Random.value < 0.5f)
                    {
                        currentState = State.SLAM;
                        StartCoroutine(SlamSequence());
                    }
                    else
                    {
                        currentState = State.SUBMERGE;
                        StartCoroutine(SubmergeSequence());
                    }

                    // currentState = State.SUBMERGE;
                    // StartCoroutine(SubmergeSequence());
                }
                break;
            case State.SLAM:
                break;
            case State.SUBMERGE:
                break;
            case State.RESURFACE:
                break;
        }
    }

    private void RunStateCode()
    {
        switch (currentState)
        {
            case State.SWIM:
                Swim();
                break;
            case State.SLAM:
                break; 
            case State.SUBMERGE:
                break;
            case State.RESURFACE:
                break;
        }
    }


    private void Swim()
    {
        if (shipObject == null) return;

        _orbitAngle += orbitSpeed * Time.deltaTime;
        if (_orbitAngle >= 360f) _orbitAngle -= 360f;

        float rad = _orbitAngle * Mathf.Deg2Rad;
        Vector3 targetPos = shipObject.transform.position
            + new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * orbitRadius
            + Vector3.up * swimDepth;

        Vector3 dir = (targetPos - transform.position).normalized;
        transform.position += dir * swimSpeed * Time.deltaTime;

        if (Vector3.zero != dir)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
        }
    }


    private IEnumerator SlamSequence()
    {
        if (shipObject == null) yield break;

        AudioManager.Instance.Play("Bloop1");

   
        float t = 0f;
        while (t < slamAimDuration)
        {
            t += Time.deltaTime;
            Vector3 toShip = (shipObject.transform.position - transform.position).normalized;
            if (toShip != Vector3.zero)
            {
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    Quaternion.LookRotation(toShip),
                    slamAimTurnSpeed * Time.deltaTime);
            }
            yield return null;
        }

        
        Vector3 arcStart = transform.position;

        Vector3 toShipFlat = shipObject.transform.position - arcStart;
        toShipFlat.y = 0f;
        Vector3 arcEnd = arcStart + toShipFlat * 2f;
        arcEnd.y = arcStart.y;

        t = 0f;
        while (t < slamArcDuration)
        {
            t += Time.deltaTime;
            float p = t / slamArcDuration;

            Vector3 prevPos = transform.position;
            Vector3 flatPos = Vector3.Lerp(arcStart, arcEnd, p);
            float   arcY    = Mathf.Sin(p * Mathf.PI) * slamRiseHeight;
            transform.position = new Vector3(flatPos.x, arcStart.y + arcY, flatPos.z);

            Vector3 moveDir = transform.position - prevPos;
            if (moveDir != Vector3.zero)
            {
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    Quaternion.LookRotation(moveDir.normalized),
                    slamAimTurnSpeed * 4f * Time.deltaTime);
            }
            yield return null;
        }

        GameObject splash = Instantiate(splashPrefab, new Vector3(transform.position.x, 0f, transform.position.z), Quaternion.identity);
        splash.transform.localScale = Vector3.one * splashScale;
         AudioManager.Instance.Play("BigWaterSplashSFX");
        Destroy(splash, 2f);
        DealDamageToShip(slamDamage);
        impulseSource.GenerateImpulse();

        Vector3 currentOffset = transform.position - shipObject.transform.position;
        _orbitAngle = Mathf.Atan2(currentOffset.z, currentOffset.x) * Mathf.Rad2Deg;

        _swimTimer   = swimDuration;
        currentState = State.SWIM;
    }


    private IEnumerator SubmergeSequence()
    {
        if (shipObject == null) yield break;

          AudioManager.Instance.Play("Bloop2");

        _invulnerable = true;

 
        currentState = State.SUBMERGE;
        Vector3 diveStart = transform.position;
        Vector3 diveEnd   = new(transform.position.x, submergeDepth, transform.position.z);
        float t = 0f;
        while (t < submergeDiveTime)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(diveStart, diveEnd, t / submergeDiveTime);
            yield return null;
        }

  
        GameObject[] whirlpools = new GameObject[whirlpoolCount];
        for (int i = 0; i < whirlpoolCount; i++)
        {
            float angle  = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float radius = Random.Range(whirlpoolMinRadius, whirlpoolMaxRadius);
            Vector3 pos = shipObject.transform.position
                + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
            whirlpools[i] = Instantiate(whirlpoolPrefab, pos, Quaternion.Euler(90.0f, 0f, 0f));
            whirlpools[i].GetComponent<WhirlpoolHazard>().Init(this);
        }


        yield return new WaitForSeconds(submergeDuration);


        foreach (GameObject w in whirlpools)
            if (w != null) w.GetComponent<WhirlpoolHazard>().Dissolve();


        currentState = State.RESURFACE;
        Vector3 riseStart  = transform.position;
        Vector3 riseOffset = transform.position - shipObject.transform.position;
        riseOffset.y = 0f;
        if (riseOffset == Vector3.zero) riseOffset = Vector3.right;
        Vector3 riseEnd = shipObject.transform.position
            + riseOffset.normalized * orbitRadius
            + Vector3.up * swimDepth;

        t = 0f;
        while (t < submergeRiseTime)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(riseStart, riseEnd, t / submergeRiseTime);
            yield return null;
        }

    
            GameObject splash = Instantiate(splashPrefab, new Vector3(transform.position.x, 0f, transform.position.z), Quaternion.identity);
             AudioManager.Instance.Play("BigWaterSplashSFX");
            splash.transform.localScale = Vector3.one * splashScale;
            Destroy(splash, 2f);
        
   impulseSource.GenerateImpulse();

        Vector3 resurfaceOffset = transform.position - shipObject.transform.position;
        _orbitAngle = Mathf.Atan2(resurfaceOffset.z, resurfaceOffset.x) * Mathf.Rad2Deg;

        _invulnerable = false;
        _swimTimer   = swimDuration;
        currentState = State.SWIM;
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        AudioManager.Instance.Play("Bloop2");
    }
}
