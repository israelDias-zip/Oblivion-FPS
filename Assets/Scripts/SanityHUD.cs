using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Oblivion - SanityHUD
///
/// Atualiza a barra de sanidade na UI e aplica efeitos visuais de tela
/// (tint vermelho, tremor de câmera) conforme a sanidade cai.
///
/// ATUALIZADO PARA FPS: o shake agora é aplicado via CameraEffects.cs
/// (câmera filha do Player), no lugar do antigo CameraFollow (top-down 2D).
///
/// SETUP:
/// 1. Crie um Canvas → Image de fundo → Image de preenchimento (tipo Filled,
///    Fill Method = Horizontal).
/// 2. Arraste a Image de preenchimento em "sanityBar".
/// 3. Arraste a câmera principal em "mainCamera".
/// </summary>
public class SanityHUD : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image sanityBar;
    [SerializeField] private Gradient barGradient;  // verde → amarelo → vermelho

    [Header("Efeito de Tela")]
    [SerializeField] private Image screenOverlay;   // Image de cor preta/vermelha em fullscreen (alpha 0)
    [SerializeField] private float maxOverlayAlpha = 0.4f;

    [Header("Câmera Shake")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float shakeIntensityMax = 0.15f;

    private Coroutine shakeCoroutine;
    private CameraEffects cameraEffects; // shake é aplicado via ShakeOffset, nunca via transform direto

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera != null) cameraEffects = mainCamera.GetComponent<CameraEffects>();

        // Escuta SanitySystem
        if (SanitySystem.Instance != null)
        {
            SanitySystem.Instance.OnSanityChanged.AddListener(UpdateHUD);
            SanitySystem.Instance.OnSanityLow.AddListener(OnLow);
            SanitySystem.Instance.OnSanityCritical.AddListener(OnCritical);
        }
    }

    // ─────────────────────────────────────────────────────────────────────

    void UpdateHUD(float normalizedSanity)
    {
        // Barra
        if (sanityBar != null)
        {
            sanityBar.fillAmount = normalizedSanity;
            if (barGradient != null)
                sanityBar.color = barGradient.Evaluate(normalizedSanity);
        }

        // Overlay de tela (quanto menos sanidade, mais vermelho/escuro)
        if (screenOverlay != null)
        {
            float alpha = Mathf.Lerp(maxOverlayAlpha, 0f, normalizedSanity);
            Color c = screenOverlay.color;
            c.a = alpha;
            screenOverlay.color = c;
        }

        // Shake leve contínuo em sanidade crítica
        if (SanitySystem.Instance != null && SanitySystem.Instance.IsCritical())
        {
            if (shakeCoroutine == null)
                shakeCoroutine = StartCoroutine(ContinuousShake());
        }
        else
        {
            StopShake();
        }
    }

    void OnLow()
    {
        StartCoroutine(PulseOverlay(0.3f, 0.5f)); // pisca levemente
        Debug.Log("[HUD] Sanidade baixa!");
    }

    void OnCritical()
    {
        StartCoroutine(PulseOverlay(0.6f, 0.8f)); // pisca forte
        Debug.Log("[HUD] Sanidade crítica!");
    }

    // ─── Efeitos ─────────────────────────────────────────────────────────

    IEnumerator PulseOverlay(float targetAlpha, float duration)
    {
        if (screenOverlay == null) yield break;

        float elapsed = 0f;
        Color c = screenOverlay.color;
        float startAlpha = c.a;

        // Fade in
        while (elapsed < duration / 2f)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, targetAlpha, elapsed / (duration / 2f));
            screenOverlay.color = c;
            yield return null;
        }

        elapsed = 0f;
        // Fade out
        while (elapsed < duration / 2f)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(targetAlpha, startAlpha, elapsed / (duration / 2f));
            screenOverlay.color = c;
            yield return null;
        }
    }

    IEnumerator ContinuousShake()
    {
        if (cameraEffects == null) yield break;

        while (true)
        {
            float sanity = SanitySystem.Instance != null
                ? SanitySystem.Instance.GetSanityNormalized()
                : 1f;

            float intensity = shakeIntensityMax * (1f - sanity);
            cameraEffects.ShakeOffset = (Vector3)Random.insideUnitCircle * intensity;
            yield return null;
        }
    }

    void StopShake()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            shakeCoroutine = null;
        }
        if (cameraEffects != null)
            cameraEffects.ShakeOffset = Vector3.zero;
    }
}
