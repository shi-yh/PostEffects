using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDouyin : FLL_PostEffetctBase
{
    public Shader glitchRGBShader;

    private Material glitchRGBMaterial;

    public Material material
    {
        get
        {
            glitchRGBMaterial = CheckShaderAndCreateMaterial(glitchRGBShader, glitchRGBMaterial);
            return glitchRGBMaterial;
        }
    }

    [Range(-0.2f,1)]
    public float _Amplitude  = -0.15f;

    [Range(-5,5)]
    public float _Amount = 0.5f;

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (material!=null)
        {
            material.SetFloat("_Amplitude",_Amplitude);
            material.SetFloat("_Amount",_Amount);
            
            Graphics.Blit(src,dest,material);
        }
        else
        {
            Graphics.Blit(src,dest);
        }
    }
}
