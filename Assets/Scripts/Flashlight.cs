using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Oblivion - Flashlight
/// Lanterna com bateria limitada. Tecla F liga/desliga.
///
/// SETUP:
/// 1. Crie um GameObject filho da câmera chamado "Flashlight".
/// 2. Adicione um componente Light (Type = Spot, Range ~15, Spot Angle ~45,
///    cor branco-quente).
/// 3. Adicione este script no mesmo GameObject (ele pega o Light sozinho).
/// </summary>
[RequireComponent(typeof(Light))]
public class Flashlight : MonoBehaviour
{
    [Header("Bateria")]
    [SerializeField] private float maxBattery = 100f;
    [SerializeField] private float drainPerSecond = 4f;
    [SerializeField] private float rechargePerSecondWhenOff = 1.5f;
    [SerializeField] private KeyCode toggleKey = KeyCode.F;

    [Header("Eventos")]
    public UnityEvent<float> OnBatteryChanged; // 0-1 normalizado
    public UnityEvent<bool> OnToggled;

    private Light spotLight;
    private float battery;
    private bool isOn = true;

    void Awake()
    {
        spotLight = GetComponent<Light>();
        battery = maxBattery;
    }

    void Start()
    {
        ApplyState();
    }

    void Update()
    {
        if (GameController.Instance != null && GameController.Instance.State != GameController.GameState.Playing)
            return;

        if (Input.GetKeyDown(toggleKey) && (battery > 0f || isOn))
        {
            isOn = !isOn;
            ApplyState();
        }

        if (isOn)
        {
            battery = Mathf.Clamp(battery - drainPerSecond * Time.deltaTime, 0f, maxBattery);
            if (battery <= 0f)
            {
                isOn = false;
                ApplyState();
            }
        }
        else if (rechargePerSecondWhenOff > 0f)
        {
            battery = Mathf.Clamp(battery + rechargePerSecondWhenOff * Time.deltaTime, 0f, maxBattery);
        }

        OnBatteryChanged?.Invoke(battery / maxBattery);
    }

    void ApplyState()
    {
        spotLight.enabled = isOn;
        OnToggled?.Invoke(isOn);
    }

    public bool IsOn() => isOn;
    public float GetBatteryNormalized() => battery / maxBattery;
}
