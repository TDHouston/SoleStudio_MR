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

    [Header("Input")]
    public XRNode inputSource = XRNode.RightHand;
    public InputHelpers.Button spawnButton = InputHelpers.Button.PrimaryButton; // A button
    public float activationThreshold = 0.1f;

    void Update()
    {
        if (dropdown != null && dropdown.value == 0)
        {
            InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(inputSource), spawnButton, out bool isPressed, activationThreshold);
            if (isPressed)
            {
                Spawn();
            }
        }
    }

    void Spawn()
    {
        if (modelToSpawn != null && spawnPoint != null)
        {
            Instantiate(modelToSpawn, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("Spawned model.");
        }
        else
        {
            Debug.LogWarning("Missing spawn target or location.");
        }
    }
}