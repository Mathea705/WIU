using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WhirlpoolHazard : MonoBehaviour
{
    [SerializeField] private float damagePerSecond = 10f;
    [SerializeField] private float rotateSpeed     = 300f;
    [SerializeField] private float fadeTime        = 1f;
    [SerializeField] private Image whirlpoolImage;

    [SerializeField] private float damageRadius = 10f;

    private BossAI     _boss;
    private GameObject _ship;

    public void Init(BossAI boss)
    {
        _boss = boss;
        _ship = GameObject.FindWithTag("Ship");
    }

    private void Start()
    {
        SetAlpha(0f);
        StartCoroutine(Fade(0f, 1f));
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);

        if (_boss == null || _ship == null) return;

        float dist = Vector3.Distance(
            new Vector3(transform.position.x, 0f, transform.position.z),
            new Vector3(_ship.transform.position.x, 0f, _ship.transform.position.z));

        if (dist <= damageRadius)
            _boss.DealDamageToShip(damagePerSecond * Time.deltaTime);
    }

    public void Dissolve()
    {
        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        yield return Fade(1f, 0f);
        Destroy(gameObject);
    }

    private IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            SetAlpha(Mathf.Lerp(from, to, t / fadeTime));
            yield return null;
        }
        SetAlpha(to);
    }

    private void SetAlpha(float alpha)
    {
        if (whirlpoolImage == null) return;
        Color c = whirlpoolImage.color;
        c.a = alpha;
        whirlpoolImage.color = c;
    }


}
