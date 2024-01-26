using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomLODGroupSLow
{
    [Range(100, 0)]
    public float nextTransition;
    [Range(100, 0)]
    public float previousTransition;
    [Range(0, 1)]
    public float fadeWidth;
    public Renderer renderer;
    [SerializeField]
    public float fadeOut;
    public float fadeIn;
    private int _fade;
    private Material _material;

    public void Initialize()
    {
        _fade = Shader.PropertyToID("_Fade");
        _material = renderer.material;
    }

    public void ComputeFade(float screenRelativeHeight)
    {
        if (_material != null)
        {
            float transitionPercentage = GetTransitionPercentage();
            fadeOut = Mathf.InverseLerp(nextTransition / 100 - transitionPercentage,
                (nextTransition / 100 + transitionPercentage), screenRelativeHeight);
            fadeIn = Mathf.InverseLerp(previousTransition / 100 + transitionPercentage,
                (previousTransition / 100 - transitionPercentage), screenRelativeHeight);
            _material.SetFloat(_fade, Mathf.Min(fadeIn, fadeOut));
            if (fadeOut == 0)
            {
                Hide();
            }
            else if (fadeOut != 0)
            {
                Show();
            }
        }
    }

    private float GetTransitionPercentage()
    {
        return (fadeWidth * Mathf.Abs(previousTransition - nextTransition)) / 100;
    }


    public void Hide()
    {
        renderer.enabled = false;
    }

    public void Show()
    {
        renderer.enabled = true;
    }

    public void FadeOut(float t)
    {
        if (renderer != null)
        {
            _material.SetFloat(_fade, 0);
        }
        if (t <= 0)
        {
            if (renderer != null)
                renderer.enabled = false;
        }
    }
}

public class CustomLODGroupBackup : MonoBehaviour
{

    private CustomLODGroupSLow currentLOD;
    private CustomLODGroupSLow previousLOD;
    public Renderer currentRenderer;
    public Renderer previousRenderer;
    public float nextTransition;
    public float previousTransition;
    public CustomLODGroupSLow[] _lods;

    private void Awake()
    {
        GetCurrentLOD();
        for (int i = 0; i < _lods.Length; i++)
        {
            _lods[i].Initialize();
        }
    }

    private void Update()
    {
        if (_lods.Length != 0)
        {
            float screenRelativeHeight = CalculateScreenRelativeSize(currentRenderer.bounds, Camera.current);
            for (int i = 0; i < _lods.Length; i++)
            {
                if (screenRelativeHeight > _lods[i].nextTransition / 100)
                {
                    if (_lods[i].renderer != currentLOD.renderer)
                    {
                        previousLOD = currentLOD;
                    }
                    currentLOD = _lods[i];
                    currentRenderer = currentLOD.renderer;
                    previousRenderer = previousLOD.renderer;
                    break;
                }
            }
            currentLOD.ComputeFade(screenRelativeHeight);
            previousLOD.ComputeFade(screenRelativeHeight);
        }
    }

    private void GetCurrentLOD()
    {
        if (_lods.Length != 0)
        {
            for (int i = 0; i < _lods.Length; i++)
            {
                float screenRelativeHeight = CalculateScreenRelativeSize(_lods[i].renderer.bounds, Camera.current);
                if (screenRelativeHeight > _lods[i].nextTransition / 100)
                {
                    currentLOD = _lods[i];
                    previousLOD = _lods[i];
                    currentRenderer = currentLOD.renderer;
                    previousRenderer = previousLOD.renderer;
                    break;
                }
                else
                {
                    _lods[i].Hide();
                }
            }
        }
    }

    float CalculateScreenRelativeSize(Bounds bounds, Camera camera)
    {
        float size = bounds.max.y;
        //Cache all the stuff above on awake or somewhere...
        float distance = Vector3.Distance(camera.transform.position, currentRenderer.transform.position);
        float screenSpaceHeight = Mathf.Abs(Mathf.Atan2(size, distance) * Mathf.Rad2Deg * QualitySettings.lodBias) / camera.fieldOfView;
        return screenSpaceHeight;
    }
}
