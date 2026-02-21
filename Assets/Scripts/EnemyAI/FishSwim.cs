using UnityEngine;

public class FishSwim : MonoBehaviour
{
    [SerializeField] private float swimSpeed      = 1.5f;
    [SerializeField] private float turnSpeed      = 2f;
    [SerializeField] private float wanderRadius   = 8f;
    [SerializeField] private float arrivalRadius  = 0.8f;
    [SerializeField] private float waterSurfaceY  = 0f;
    [SerializeField] private float modelYOffset   = 0f;   // adjust until fish faces forward (try 90, -90, 180)

    private Vector3 _origin;
    private Vector3 _target;

    void Start()
    {
        _origin = transform.position;
        PickNewTarget();
    }

    void Update()
    {
     
        Vector3 dir = _target - transform.position;
        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir) * Quaternion.Euler(0f, modelYOffset, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * turnSpeed);
        }

  
        transform.position += transform.forward * swimSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, _target) < arrivalRadius)
            PickNewTarget();
    }

    private void PickNewTarget()
    {
        Vector3 point = _origin + Random.insideUnitSphere * wanderRadius;
        point.y = Mathf.Min(point.y, waterSurfaceY - 0.5f); 
        _target = point;
    }
}
