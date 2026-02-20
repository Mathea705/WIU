using System.Collections;
using UnityEngine;

public class RippleEffect : MonoBehaviour
{
    public int TextureSize = 512;
    public RenderTexture ObjectsRT;
    private RenderTexture CurrRT, PrevRT, TempRT;
    public Shader RippleShader, AddShader;
    private Material RippleMat, AddMat;
    private Renderer _renderer;

   
    public Transform shipTransform;
   
    public Camera rippleCamera;
 
    public Vector2 trackingOffset;

    void Start()
    {
        CurrRT = new RenderTexture(TextureSize, TextureSize, 0, RenderTextureFormat.RFloat) { filterMode = FilterMode.Bilinear };
        PrevRT = new RenderTexture(TextureSize, TextureSize, 0, RenderTextureFormat.RFloat) { filterMode = FilterMode.Bilinear };
        TempRT = new RenderTexture(TextureSize, TextureSize, 0, RenderTextureFormat.RFloat) { filterMode = FilterMode.Bilinear };
        RippleMat = new Material(RippleShader);
        AddMat = new Material(AddShader);

        _renderer = GetComponent<Renderer>();
        _renderer.material.SetTexture("_RippleTex", CurrRT);

        StartCoroutine(Ripples());
    }

    IEnumerator Ripples()
    {
        while (true)
        {
            
            if (shipTransform != null)
            {
                float tx = shipTransform.position.x + trackingOffset.x;
                float tz = shipTransform.position.z + trackingOffset.y;

                transform.position = new Vector3(tx, transform.position.y, tz);

                if (rippleCamera != null)
                {
                    Vector3 cp = rippleCamera.transform.position;
                    cp.x = tx;
                    cp.z = tz;
                    rippleCamera.transform.position = cp;
                }
            }

            
            AddMat.SetTexture("_ObjectsRT", ObjectsRT);
            AddMat.SetTexture("_CurrentRT", CurrRT);
            Graphics.Blit(null, TempRT, AddMat);

            RenderTexture rt0 = TempRT;
            TempRT = CurrRT;
            CurrRT = rt0;

         
            RippleMat.SetTexture("_PrevRT", PrevRT);
            RippleMat.SetTexture("_CurrentRT", CurrRT);
            Graphics.Blit(null, TempRT, RippleMat);
            Graphics.Blit(TempRT, PrevRT);


            RenderTexture rt = PrevRT;
            PrevRT = CurrRT;
            CurrRT = rt;

           
            _renderer.material.SetTexture("_RippleTex", CurrRT);

            yield return null;
        }
    }
}
