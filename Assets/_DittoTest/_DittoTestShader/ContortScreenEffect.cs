using UnityEngine;
using System.Collections;

public class ContortScreenEffect : PostEffectBase
{


    //采样率
    public float BumpAmt = 1;
    void Awake()
    {
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //RenderTexture temp1 = RenderTexture.GetTemporary(source.width >> downSample, source.height >> downSample, 0);
        //Graphics.Blit(renderTexture, temp1, _Material, 0);
        //_Material.SetTexture("_OriTex", temp1);
        _Material.SetFloat("_BumpAmt", BumpAmt);
        Graphics.Blit(source, destination, _Material, 0);
    }


}
