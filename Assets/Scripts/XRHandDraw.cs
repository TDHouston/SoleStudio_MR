using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class XRControllerDraw : MonoBehaviour
{
    public enum BrushShape
    {
        Round,
        Ribbon,
        Square
    }

    [Header("Drawing Source")] [SerializeField]
    private Transform drawPoint;

    [Header("Input Settings")] [SerializeField]
    private InputActionReference triggerAction;

    [Header("Draw Parameters")] [SerializeField]
    private float minDistanceBeforeNewPoint = 0.008f;

    [SerializeField] private float tubeDefaultWidth = 0.010f;
    [SerializeField] private int tubeSides = 8;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Material defaultLineMaterial;
    [SerializeField] private bool enableGravity = false;
    [SerializeField] private bool colliderTrigger = false;
    public GameObject currentShoeModel;

    private float radiusX = 0.01f;
    private float radiusY = 0.01f;

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

        if (currentShoeModel != null)
        {
            go.transform.SetParent(currentShoeModel.transform, worldPositionStays: true);
        }

        TubeRenderer goTubeRenderer = go.AddComponent<TubeRenderer>();
        tubeRenderers.Add(goTubeRenderer);

        var renderer = go.GetComponent<MeshRenderer>();
        renderer.material = defaultLineMaterial;

        goTubeRenderer.ColliderTrigger = colliderTrigger;
        goTubeRenderer.SetPositions(points.ToArray());

        goTubeRenderer._radiusOne = radiusX;
        goTubeRenderer._radiusTwo = radiusY;
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

    public void SetLineWidth(float newWidth)
    {
        radiusX = newWidth;
        radiusY = newWidth;
        if (currentTubeRenderer != null)
        {
            currentTubeRenderer._radiusOne = newWidth;
            currentTubeRenderer._radiusTwo = newWidth;
        }
    }

    public void UpdateLineWidth(float newValue)
    {
        SetLineWidth(newValue);
    }

    public void UpdateLineColor(Color color)
    {
        defaultColor = color;
        defaultLineMaterial.color = color;
        if (currentTubeRenderer != null)
            currentTubeRenderer.material = defaultLineMaterial;
    }

    public void UpdateLineMinDistance(float newValue)
    {
        minDistanceBeforeNewPoint = newValue;
    }

    public void ClearAllDrawnMeshes()
    {
        foreach (var tube in tubeRenderers)
        {
            if (tube != null)
            {
                Destroy(tube.gameObject);
            }
        }

        tubeRenderers.Clear();
        points.Clear();
        AddNewTubeRenderer();
    }

    public void SetBrushShape(BrushShape shape)
    {
        switch (shape)
        {
            case BrushShape.Round:
                tubeSides = 8;
                radiusX = 0.01f;
                radiusY = 0.01f;
                break;
            case BrushShape.Ribbon:
                tubeSides = 8;
                radiusX = 0.015f;
                radiusY = 0.003f;
                break;
            case BrushShape.Square:
                tubeSides = 4;
                radiusX = 0.01f;
                radiusY = 0.01f;
                break;
        }
    }
}