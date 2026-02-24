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
    [SerializeField] private float slamAimDuration  = 1.5f;   // time spent rotating to face ship
    [SerializeField] private float slamAimTurnSpeed = 120f;   // degrees per second
    [SerializeField] private float slamArcDuration  = 2.0f;   // total time of the arc
    [SerializeField] private float slamRiseHeight   = 20f;    // peak height of the arc
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

        // --- Phase 2: ARC ---
        // Ballistic arc over the ship — sin curve handles the height naturally
        Vector3 arcStart = transform.position;
        // Land on the far side of the ship, same depth as start
        Vector3 toShipFlat = shipObject.transform.position - arcStart;
        toShipFlat.y = 0f;
        Vector3 arcEnd = arcStart + toShipFlat * 2f;
        arcEnd.y = arcStart.y;

        bool hasDealtDamage = false;
        t = 0f;
        while (t < slamArcDuration)
        {
            t += Time.deltaTime;
            float p = t / slamArcDuration;

            Vector3 prevPos  = transform.position;
            Vector3 flatPos  = Vector3.Lerp(arcStart, arcEnd, p);
            float   arcY     = Mathf.Sin(p * Mathf.PI) * slamRiseHeight;
            transform.position = new Vector3(flatPos.x, arcStart.y + arcY, flatPos.z);

            // Face direction of travel
            Vector3 moveDir = transform.position - prevPos;
            if (moveDir != Vector3.zero)
            {
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    Quaternion.LookRotation(moveDir.normalized),
                    slamAimTurnSpeed * 4f * Time.deltaTime);
            }

            // Damage when grazing the ship
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

        // Recalculate orbit angle from actual landing position so Swim() targets the
        // nearest point on the orbit ring — no snap, just a smooth glide back in.
        Vector3 currentOffset = transform.position - shipObject.transform.position;
        _orbitAngle = Mathf.Atan2(currentOffset.z, currentOffset.x) * Mathf.Rad2Deg;

        _swimTimer   = swimDuration;
        currentState = State.SWIM;
    }

    protected override void OnDeath()
    {
        base.OnDeath();
    }
}
