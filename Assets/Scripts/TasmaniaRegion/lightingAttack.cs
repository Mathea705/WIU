using UnityEngine;

public class lightingAttack : BossAI
{

    public float speed = 40f;
    public float groundY = 0f;
    public GameObject target;
    public float stopDistance = 0.5f;
    private bool HasHit = false;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    // Update is called once per frame
    protected override void Update()
    {
        if (!HasHit) //travel downwards
        {
            //transform.position += Vector3.down * speed * Time.deltaTime;
            //Vector3 dir = (transform.position - transform.position).normalized;
            //transform.position += dir * speed * Time.deltaTime;


            //if (Vector3.Distance(transform.position, target.transform.position) <= stopDistance)
            //{
            //    HitTarget();
            //}
            //if (transform.position.y <= groundY)
            //{
            //    HitGround();
            //}
            if (target != null)
            {
                
                Vector3 dir = (target.transform.position - transform.position).normalized;
                transform.position += dir * speed * Time.deltaTime;

                if (Vector3.Distance(transform.position, target.transform.position) <= stopDistance)
                {
                    HitTarget();
                }
            }
            else
            {
                
                transform.position += Vector3.down * speed * Time.deltaTime;

                if (transform.position.y <= groundY)
                {
                    HitGround();
                }
            }
        }
    }
    void HitTarget()
    {
        HasHit = true;

        //damaee
        Debug.Log("Player hit by lightning!");
        DealDamageToShip(5);
        Destroy(gameObject, 0.1f);
    }
    public void HitGround()
    {
        HasHit = true;

        //player and ship damage to be done here //nvm

        Destroy(gameObject, 0.1f);
    }
}
