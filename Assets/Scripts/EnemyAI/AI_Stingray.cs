using UnityEngine;

public class AI_Stingray : MonoBehaviour
{
    [SerializeField] private GameObject Player;

    private enum State
    {
        WANDER,
        AGGRO,
        SPRING,
        STING,
        RECUPERATE,
    }

    private bool CTRL_left;
    private bool CTRL_right;
    private bool CTRL_up;
    private bool CTRL_down;

    private float speed;


    private void Start()
    {
        speed = 5f;
    }
}
