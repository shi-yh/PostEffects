using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RenderFeatureControl : MonoBehaviour
{
    [SerializeField]
    private UniversalRendererData _rendererData;
    private BrightnessPosetProcessing myRenderFeature;

    private void Start()
    {
        myRenderFeature = _rendererData.rendererFeatures.OfType<BrightnessPosetProcessing>().FirstOrDefault();
    }

    private float time;
    
    private void Update()
    {
        time += Time.deltaTime;

        if (time>5)
        {
            myRenderFeature.SetActive(false);
        }
        
    }
}
