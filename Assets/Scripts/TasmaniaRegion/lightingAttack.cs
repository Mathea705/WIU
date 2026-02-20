using UnityEngine;

public class lightingAttack : MonoBehaviour
{

    public float speed = 40f;
    public float groundY = 0f;

    private bool HasHit = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Destroy(gameObject, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!HasHit) //travel downwards
        {
            transform.position += Vector3.down * speed * Time.deltaTime;

            if (transform.position.y <= groundY)
            {
                HitGround();
            }
        }
    }

    public void HitGround()
    {
        HasHit = true;

        //player and ship damage to be done here

        Destroy(gameObject, 0.1f);
    }
}
