using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Oblivion - FlashlightHUD
/// Atualiza a barra de bateria da lanterna na UI.
///
/// SETUP:
/// 1. Crie uma Image (Image Type = Filled, Fill Method = Horizontal) no Canvas.
/// 2. Arraste-a em "batteryBar".
/// 3. Arraste o componente Flashlight (da câmera) em "flashlight"
///    (se deixar em branco, ele procura sozinho na cena).
/// </summary>
public class FlashlightHUD : MonoBehaviour
{
    [SerializeField] private Flashlight flashlight;
    [SerializeField] private Image batteryBar;
    [SerializeField] private Gradient barGradient; // amarelo → vermelho, por exemplo

    void Start()
    {
        if (flashlight == null)
            flashlight = FindObjectOfType<Flashlight>();

        if (flashlight != null)
            flashlight.OnBatteryChanged.AddListener(UpdateBar);
    }

    void UpdateBar(float normalized)
    {
        if (batteryBar == null) return;

        batteryBar.fillAmount = normalized;
        if (barGradient != null)
            batteryBar.color = barGradient.Evaluate(normalized);
    }
}
