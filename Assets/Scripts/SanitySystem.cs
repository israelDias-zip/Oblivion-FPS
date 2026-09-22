using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Oblivion - SanitySystem
/// Controla a barra de sanidade do personagem.
/// Outros sistemas (MapMemory, PostProcessing) escutam os eventos aqui.
/// </summary>
public class SanitySystem : MonoBehaviour
{
    // ── Singleton simples ─────────────────────────────────────────────────
    public static SanitySystem Instance { get; private set; }

    // ── Configurações ─────────────────────────────────────────────────────
    [Header("Sanidade")]
    [SerializeField, Range(0f, 100f)] private float maxSanity       = 100f;
    [SerializeField, Range(0f, 100f)] private float startingSanity  = 100f;
    [SerializeField] private float passiveDrainPerSecond = 0.4f; // 100/0.4 = 250s base (~4min)

    [Header("Thresholds (%)")]
    [SerializeField] private float lowSanityThreshold    = 40f;  // começa efeitos visuais leves
    [SerializeField] private float criticalSanityThreshold = 20f; // efeitos pesados + mapa sumindo rápido

    // ── Estado ───────────────────────────────────────────────────────────
    private float currentSanity;
    private bool isDead = false;

    // ── Eventos ──────────────────────────────────────────────────────────
    [Header("Eventos")]
    public UnityEvent<float> OnSanityChanged;       // 0-1 normalizado
    public UnityEvent        OnSanityLow;            // cruzou threshold baixo
    public UnityEvent        OnSanityCritical;       // cruzou threshold crítico
    public UnityEvent        OnSanityDepleted;       // chegou a zero → game over

    private bool lowFired      = false;
    private bool criticalFired = false;

    // ─────────────────────────────────────────────────────────────────────

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        currentSanity = startingSanity;
    }

    void Update()
    {
        if (isDead) return;
        if (GameController.Instance != null && GameController.Instance.State != GameController.GameState.Playing) return;

        DrainSanity(passiveDrainPerSecond * Time.deltaTime);
    }

    // ─── API Pública ─────────────────────────────────────────────────────

    /// <summary>Remove sanidade (jumpscares, puzzles, etc.)</summary>
    public void DrainSanity(float amount)
    {
        if (isDead) return;

        currentSanity = Mathf.Clamp(currentSanity - amount, 0f, maxSanity);
        float normalized = currentSanity / maxSanity;

        OnSanityChanged?.Invoke(normalized);
        CheckThresholds(normalized);

        if (currentSanity <= 0f)
            TriggerGameOver();
    }

    /// <summary>Restaura sanidade (itens, eventos de narrativa)</summary>
    public void RestoreSanity(float amount)
    {
        if (isDead) return;

        currentSanity = Mathf.Clamp(currentSanity + amount, 0f, maxSanity);
        OnSanityChanged?.Invoke(currentSanity / maxSanity);

        // Reset de flags se saiu das zonas críticas
        if (currentSanity / maxSanity > lowSanityThreshold / 100f)
        {
            lowFired      = false;
            criticalFired = false;
        }
    }

    public float GetSanityNormalized() => currentSanity / maxSanity;
    public float GetSanityRaw()        => currentSanity;
    public bool  IsLow()               => (currentSanity / maxSanity) <= (lowSanityThreshold / 100f);
    public bool  IsCritical()          => (currentSanity / maxSanity) <= (criticalSanityThreshold / 100f);

    // ─── Privado ─────────────────────────────────────────────────────────

    void CheckThresholds(float normalized)
    {
        if (!lowFired && normalized <= lowSanityThreshold / 100f)
        {
            lowFired = true;
            OnSanityLow?.Invoke();
        }
        if (!criticalFired && normalized <= criticalSanityThreshold / 100f)
        {
            criticalFired = true;
            OnSanityCritical?.Invoke();
        }
    }

    void TriggerGameOver()
    {
        isDead = true;
        OnSanityDepleted?.Invoke();
        Debug.Log("[Oblivion] Sanidade zerada → Game Over");
    }
}
