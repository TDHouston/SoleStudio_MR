using UnityEngine;
using TMPro;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class DropdownControlledSpawner : MonoBehaviour
{
    [Header("UI Reference")] 
    public TMP_Dropdown dropdown;

    [Header("Spawning")] 
    public GameObject modelToSpawn;
    public Transform spawnPoint;

    [Header("Draw Integration")] 
    public XRControllerDraw drawTool;

    [Header("Input")] 
    public XRNode inputSource = XRNode.RightHand;
    public InputHelpers.Button spawnButton = InputHelpers.Button.PrimaryButton;
    public float activationThreshold = 0.1f;

    private GameObject lastSpawnedModel;

    void Update()
    {
        if (dropdown != null && dropdown.value == 0)
        {
            InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(inputSource), spawnButton, out bool isPressed,
                activationThreshold);
            if (isPressed && lastSpawnedModel == null) // ✅ prevent repeat spawns
            {
                Spawn();
            }
        }
    }

    void Spawn()
    {
        if (modelToSpawn != null && spawnPoint != null)
        {
            lastSpawnedModel = Instantiate(modelToSpawn, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("Spawned model.");

            if (drawTool != null)
            {
                drawTool.currentShoeModel = lastSpawnedModel; // ✅ Link to draw tool
                Debug.Log("Linked model to draw tool.");
            }
        }
        else
        {
            Debug.LogWarning("Missing spawn target or location.");
        }
    }
}