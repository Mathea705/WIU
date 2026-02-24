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

    [SerializeField] private float orbitRadius = 40f;
    [SerializeField] private float orbitSpeed  = 20f;
    [SerializeField] private float swimSpeed   = 4f;
    [SerializeField] private float turnSpeed   = 2f;
    [SerializeField] private float swimDepth   = -5f;

    private float _orbitAngle;

    protected override void Start()
    {
        base.Start();

       
        if (shipObject != null)
        {
            Vector3 offset = transform.position - shipObject.transform.position;
            _orbitAngle = Mathf.Atan2(offset.z, offset.x) * Mathf.Rad2Deg;
        }

        currentState = State.SWIM;
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

    protected override void OnDeath()
    {
        base.OnDeath();
    }
}
