using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public enum GameState { MainMenu, Playing, Won, GameOver }
    public GameState State { get; private set; } = GameState.MainMenu;

    [Header("Painel Principal (Menu Inicial + Game Over)")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private Text       mainMenuTitleText;
    [SerializeField] private Text       mainMenuSubtitleText;
    [SerializeField] private Text       mainMenuButtonText;

    [Header("Painel de Vitória")]
    [SerializeField] private GameObject winPanel;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (SanitySystem.Instance != null)
            SanitySystem.Instance.OnSanityDepleted.AddListener(TriggerGameOver);

        if (winPanel != null) winPanel.SetActive(false);

        ShowMainMenu(isGameOver: false);
    }

    // ── Menu Inicial ──────────────────────────────────────────────────────
    void ShowMainMenu(bool isGameOver)
    {
        State = isGameOver ? GameState.GameOver : GameState.MainMenu;
        Time.timeScale = 0f;

        if (mainMenuTitleText   != null) mainMenuTitleText.text    = "OBLIVION";
        if (mainMenuSubtitleText != null) mainMenuSubtitleText.text = isGameOver ? "SANIDADE ESGOTADA" : "";
        if (mainMenuButtonText  != null) mainMenuButtonText.text   = isGameOver ? "REINICIAR" : "INICIAR";

        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);

        // Oculta HUD do jogo durante o menu
        var hud = GameObject.Find("GameCanvas");
        if (hud != null)
        {
            var sanityBg = hud.transform.Find("SanityBarBG");
            if (sanityBg != null) sanityBg.gameObject.SetActive(false);
        }
    }

    // ── Botão Iniciar / Reiniciar ─────────────────────────────────────────
    public void StartGame()
    {
        if (State == GameState.Playing) return;

        // Se game over ou win, recarrega a cena
        if (State == GameState.GameOver || State == GameState.Won)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        // Primeira vez: inicia a partir do menu
        State = GameState.Playing;
        Time.timeScale = 1f;

        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);

        // Mostra HUD
        var hud = GameObject.Find("GameCanvas");
        if (hud != null)
        {
            var sanityBg = hud.transform.Find("SanityBarBG");
            if (sanityBg != null) sanityBg.gameObject.SetActive(true);
        }

        AudioManager.Instance?.StartAmbient();

        // OBS: na versão FPS a câmera é filha do Player (MouseLook + CameraEffects),
        // então não precisa mais de "snap" manual como na antiga câmera top-down.

        Debug.Log("[Oblivion] Jogo iniciado!");
    }

    // ── Vitória ───────────────────────────────────────────────────────────
    public void TriggerWin()
    {
        if (State != GameState.Playing) return;
        State = GameState.Won;
        Time.timeScale = 0f;
        AudioManager.Instance?.StopAmbient();
        if (winPanel != null) winPanel.SetActive(true);
        Debug.Log("[Oblivion] VITÓRIA!");
    }

    // ── Game Over ─────────────────────────────────────────────────────────
    public void TriggerGameOver()
    {
        if (State != GameState.Playing) return;
        AudioManager.Instance?.StopAmbient();
        ShowMainMenu(isGameOver: true);
        Debug.Log("[Oblivion] GAME OVER - Sanidade zerada.");
    }

    // ── Botões dos painéis ────────────────────────────────────────────────
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
