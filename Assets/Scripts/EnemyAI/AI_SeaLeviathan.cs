using System.Collections;
using UnityEngine;

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

    [Header("Swim")]
    [SerializeField] private float orbitRadius   = 40f;
    [SerializeField] private float orbitSpeed    = 20f;
    [SerializeField] private float swimSpeed     = 4f;
    [SerializeField] private float turnSpeed     = 2f;
    [SerializeField] private float swimDepth     = -5f;
    [SerializeField] private float swimDuration  = 20f;

    [Header("Slam")]
    [SerializeField] private float slamAimDuration  = 1.5f;  // time spent rotating to face ship
    [SerializeField] private float slamRiseHeight   = 20f;   // how high it arcs above the ship
    [SerializeField] private float slamRiseDuration = 0.8f;  // time to reach the peak
    [SerializeField] private float slamCrashSpeed   = 35f;   // speed of the dive
    [SerializeField] private float slamDamage       = 40f;
    [SerializeField] private float slamHitDist      = 8f;

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

        Debug.Log(currentState);
    }

    private void CheckStateTransitions()
    {
        switch (currentState)
        {
            case State.SWIM:
                _swimTimer -= Time.deltaTime;
                if (_swimTimer <= 0f)
                {
                    currentState = State.SLAM;
                    StartCoroutine(SlamSequence());
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
                break; // handled by SlamSequence coroutine
            case State.SUBMERGE:
                break;
            case State.RESURFACE:
                break;
        }
    }

    // ================= SWIM =================
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

    // ================= SLAM =================
    private IEnumerator SlamSequence()
    {
        if (shipObject == null) yield break;

        // --- Phase 1: AIM ---
        // Stop and slowly rotate to face the ship
        float t = 0f;
        while (t < slamAimDuration)
        {
            t += Time.deltaTime;
            Vector3 toShip = (shipObject.transform.position - transform.position).normalized;
            if (Vector3.zero != toShip)
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(toShip),
                    turnSpeed * Time.deltaTime);
            yield return null;
        }

        // --- Phase 2: RISE ---
        // Arc upward above the ship like an arrow being launched
        Vector3 riseStart  = transform.position;
        Vector3 riseTarget = shipObject.transform.position + Vector3.up * slamRiseHeight;

        t = 0f;
        while (t < slamRiseDuration)
        {
            t += Time.deltaTime;
            float p = t / slamRiseDuration;
            transform.position = Vector3.Lerp(riseStart, riseTarget, p);

            Vector3 riseDir = (riseTarget - transform.position).normalized;
            if (Vector3.zero != riseDir)
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(riseDir),
                    turnSpeed * 4f * Time.deltaTime);
            yield return null;
        }

        // --- Phase 3: CRASH ---
        // Dive down fast, grazing the ship
        bool hasDealtDamage = false;
        Vector3 crashTarget = shipObject.transform.position + Vector3.down * 10f;

        while (Vector3.Distance(transform.position, crashTarget) > 1.5f)
        {
            Vector3 prevPos  = transform.position;
            Vector3 crashDir = (crashTarget - transform.position).normalized;
            transform.position += crashDir * slamCrashSpeed * Time.deltaTime;

            if (crashDir != Vector3.zero)
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(crashDir),
                    turnSpeed * 6f * Time.deltaTime);

            if (!hasDealtDamage)
            {
                float dist     = Vector3.Distance(transform.position, shipObject.transform.position);
                float prevDist = Vector3.Distance(prevPos, shipObject.transform.position);
                if (dist <= slamHitDist || prevDist <= slamHitDist)
                {
                    DealDamageToShip(slamDamage);
                    hasDealtDamage = true;
                }
            }
            yield return null;
        }

        // Back to swim
        _swimTimer   = swimDuration;
        currentState = State.SWIM;
    }

    protected override void OnDeath()
    {
        base.OnDeath();
    }
}
