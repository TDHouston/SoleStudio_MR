using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class XRControllerDraw : MonoBehaviour
{
    [Header("Drawing Source")]
    [SerializeField] private Transform drawPoint; // Assign tip of controller or child GameObject

    [Header("Input Settings")]
    [SerializeField] private InputActionReference triggerAction; // Reference XR Trigger Input

    [Header("Draw Parameters")]
    [SerializeField] private float minDistanceBeforeNewPoint = 0.008f;
    [SerializeField] private float tubeDefaultWidth = 0.010f;
    [SerializeField] private int tubeSides = 8;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Material defaultLineMaterial;
    [SerializeField] private bool enableGravity = false;
    [SerializeField] private bool colliderTrigger = false;

    private Vector3 prevPointDistance = Vector3.zero;
    private List<Vector3> points = new List<Vector3>();
    private List<TubeRenderer> tubeRenderers = new List<TubeRenderer>();
    private TubeRenderer currentTubeRenderer;

    private bool wasDrawingLastFrame = false;

    private void Start()
    {
        AddNewTubeRenderer();
    }

    private void Update()
    {
        if (drawPoint == null || triggerAction == null)
            return;

        bool isDrawing = triggerAction.action.ReadValue<float>() > 0.5f;

        if (isDrawing)
        {
            UpdateTube();
            wasDrawingLastFrame = true;
        }
        else if (wasDrawingLastFrame)
        {
            if (enableGravity)
                currentTubeRenderer.EnableGravity();

            AddNewTubeRenderer();
            wasDrawingLastFrame = false;
        }
    }

    private void AddNewTubeRenderer()
    {
        points.Clear();
        GameObject go = new GameObject($"TubeRenderer__{tubeRenderers.Count}");
        go.transform.position = Vector3.zero;

        TubeRenderer goTubeRenderer = go.AddComponent<TubeRenderer>();
        tubeRenderers.Add(goTubeRenderer);

        var renderer = go.GetComponent<MeshRenderer>();
        renderer.material = defaultLineMaterial;

        goTubeRenderer.ColliderTrigger = colliderTrigger;
        goTubeRenderer.SetPositions(points.ToArray());
        goTubeRenderer._radiusOne = tubeDefaultWidth;
        goTubeRenderer._radiusTwo = tubeDefaultWidth;
        goTubeRenderer._sides = tubeSides;

        currentTubeRenderer = goTubeRenderer;
    }

    private void UpdateTube()
    {
        if (prevPointDistance == Vector3.zero)
        {
            prevPointDistance = drawPoint.position;
        }

        if (Vector3.Distance(prevPointDistance, drawPoint.position) >= minDistanceBeforeNewPoint)
        {
            prevPointDistance = drawPoint.position;
            AddPoint(prevPointDistance);
        }
    }

    private void AddPoint(Vector3 position)
    {
        points.Add(position);
        currentTubeRenderer.SetPositions(points.ToArray());
        currentTubeRenderer.GenerateMesh();
    }

    public void UpdateLineWidth(float newValue)
    {
        currentTubeRenderer._radiusOne = newValue;
        currentTubeRenderer._radiusTwo = newValue;
        tubeDefaultWidth = newValue;
    }

    public void UpdateLineColor(Color color)
    {
        defaultColor = color;
        defaultLineMaterial.color = color;
        currentTubeRenderer.material = defaultLineMaterial;
    }

    public void UpdateLineMinDistance(float newValue)
    {
        minDistanceBeforeNewPoint = newValue;
    }
}
