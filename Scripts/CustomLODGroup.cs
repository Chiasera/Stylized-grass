using System.Collections;
using UnityEngine;

[System.Serializable]
public class CustomLOD
{
    [Range(0, 100)]
    public float nextTransition;
    private float previousTransition;
    public float PreviousTransition
    {
        get
        {
            return previousTransition;
        }
        set
        {
            previousTransition = value;
        }
    }
    public Renderer renderer;
}

public class CustomLODGroup : MonoBehaviour
{
    [SerializeField]
    private CustomLOD[] _lods;
    [Tooltip("To ensure smooth fade transitions between LODs, use the material properties of the object, under the 'LOD' section")]
    [SerializeField]
    [Range(0, 1)]
    private float renderLoopInterval = 0.1f;

    private Transform mainCameraTransform;
    private Bounds[] allBounds;
    private Vector3[] allBoundsSize;
    private Vector3[] allRendererPositions;
    private float fieldOfView;
    private float lodBias;

    private int currentLOD;

    private Coroutine LODRenderCoroutine;

    private void Awake()
    {
        InitializeLODSettings();
        LODRenderCoroutine = StartCoroutine(DoLODRenderLoop());
    }

    IEnumerator DoLODRenderLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(renderLoopInterval);
            if (_lods.Length == 0 || allBounds == null) yield return null;
            float screenRelativeHeight = CalculateScreenRelativeSize();
            UpdateLODRendering(screenRelativeHeight);
        }
    }

    private void InitializeLODSettings()
    {
        mainCameraTransform = Camera.main.transform;
        fieldOfView = Camera.main.fieldOfView;
        lodBias = QualitySettings.lodBias;

        allBounds = new Bounds[_lods.Length];
        allBoundsSize = new Vector3[_lods.Length];
        allRendererPositions = new Vector3[_lods.Length];

        for (int i = 0; i < _lods.Length; i++)
        {
            var lodRenderer = _lods[i].renderer;
            allBounds[i] = lodRenderer.bounds;
            allRendererPositions[i] = lodRenderer.transform.position;
            allBoundsSize[i] = allBounds[i].size;
        }
        for (int i = 0; i < _lods.Length; i++)
        {
            if (i == 0)
            {
                _lods[i].PreviousTransition = 100;
            }
            else
            {
                _lods[i].PreviousTransition = _lods[i - 1].nextTransition;
            }
        }
        currentLOD = DetermineInitialLOD();
    }

    private int DetermineInitialLOD()
    {
        float initialScreenRelativeHeight = CalculateScreenRelativeSize();
        for (int i = 0; i < _lods.Length; i++)
        {
            if (initialScreenRelativeHeight > _lods[i].nextTransition / 100)
            {
                return i;
            }
        }
        return 0;
    }

    private void UpdateLODRendering(float screenRelativeHeight)
    {
        for (int i = 0; i < _lods.Length; i++)
        {
            _lods[i].renderer.enabled = screenRelativeHeight > _lods[i].nextTransition / 100 &&
                                        screenRelativeHeight < _lods[i].PreviousTransition / 100;
        }
    }

    private float CalculateScreenRelativeSize()
    {
        float size = Mathf.Max(allBoundsSize[currentLOD].x, allBoundsSize[currentLOD].y, allBoundsSize[currentLOD].z);
        float distance = Vector3.Distance(mainCameraTransform.position, allRendererPositions[currentLOD]);
        float screenSpaceHeight = Mathf.Abs(Mathf.Atan2(size, distance) * Mathf.Rad2Deg * lodBias) / fieldOfView;
        return screenSpaceHeight;
    }

    private void OnDisable()
    {
        StopCoroutine(LODRenderCoroutine);
    }
}
