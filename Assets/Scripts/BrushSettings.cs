using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BrushSettingsUI : MonoBehaviour
{
    [Header("References")]
    public XRControllerDraw drawTool;

    [Header("UI Components")]
    public TMP_Dropdown shapeDropdown;
    public Slider sizeSlider;
    public Button clearButton;

    private void Start()
    {
        if (shapeDropdown != null)
            shapeDropdown.onValueChanged.AddListener(OnShapeChanged);

        if (sizeSlider != null)
            sizeSlider.onValueChanged.AddListener(OnSizeChanged);

        if (clearButton != null)
            clearButton.onClick.AddListener(OnClearPressed);
    }

    private void OnShapeChanged(int index)
    {
        if (drawTool == null) return;

        switch (index)
        {
            case 0:
                drawTool.SetBrushShape(XRControllerDraw.BrushShape.Round);
                break;
            case 1:
                drawTool.SetBrushShape(XRControllerDraw.BrushShape.Ribbon);
                break;
            case 2:
                drawTool.SetBrushShape(XRControllerDraw.BrushShape.Square);
                break;
        }
    }

    private void OnSizeChanged(float value)
    {
        if (drawTool != null)
            drawTool.SetLineWidth(value);
    }

    private void OnClearPressed()
    {
        if (drawTool != null)
            drawTool.ClearAllDrawnMeshes();
    }
}