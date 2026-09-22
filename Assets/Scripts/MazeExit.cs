using UnityEngine;

/// <summary>
/// Oblivion - MazeExit (versão 3D)
/// Coloque num GameObject com BoxCollider (Is Trigger = true) na saída do labirinto.
/// Uma luz verde + leve pulso de escala chamam a atenção do jogador na escuridão.
///
/// SETUP:
/// 1. Crie um GameObject (ex.: uma porta ou portal) com BoxCollider.
/// 2. Adicione este script.
/// 3. (Opcional) arraste uma Light (Point Light) em "exitLight" e os
///    Renderers da porta em "exitRenderers" para o efeito de brilho verde.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class MazeExit : MonoBehaviour
{
    [Header("Visual da Saída")]
    [SerializeField] private Light exitLight;
    [SerializeField] private Color exitColor = new Color(0.25f, 1f, 0.55f, 1f);
    [SerializeField] private Renderer[] exitRenderers;

    [Header("Pulso (chama a atenção)")]
    [SerializeField] private bool pulse = true;
    [SerializeField] private float pulseSpeed = 2.5f;
    [SerializeField] private float pulseScaleAmount = 0.08f;

    private Vector3 baseScale;

    void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;
        baseScale = transform.localScale;

        if (exitLight != null)
            exitLight.color = exitColor;

        if (exitRenderers != null)
        {
            foreach (var r in exitRenderers)
            {
                if (r == null) continue;
                r.material.color = exitColor;
                r.material.EnableKeyword("_EMISSION");
                r.material.SetColor("_EmissionColor", exitColor);
            }
        }
    }

    void Update()
    {
        if (!pulse) return;

        float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseScaleAmount;
        transform.localScale = baseScale * scale;

        if (exitLight != null)
            exitLight.intensity = 2f + Mathf.Sin(Time.time * pulseSpeed) * 0.6f;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            GameController.Instance?.TriggerWin();
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        var col = GetComponent<BoxCollider>();
        if (col == null) return;
        Gizmos.color = new Color(0f, 1f, 0.4f, 0.35f);
        Gizmos.DrawCube(transform.position + col.center, col.size);
        Gizmos.color = new Color(0f, 1f, 0.4f, 1f);
        Gizmos.DrawWireCube(transform.position + col.center, col.size);
    }
#endif
}
