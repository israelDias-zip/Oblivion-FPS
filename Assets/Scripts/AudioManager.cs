using UnityEngine;
using System.Collections;

/// <summary>
/// Oblivion - AudioManager
/// Toca sons ambiente e reage à sanidade do jogador.
///
/// SETUP:
/// 1. Importe clips de áudio (.wav / .mp3) para Assets/Audio/
/// 2. Arraste nos campos abaixo:
///    - ambientLoop   → som de caverna úmida em loop (ex: cave_ambience.wav)
///    - lowSanityHiss → chiado/estático que aumenta com a insanidade (ex: static_hiss.wav)
/// Sites para áudio CC0: freesound.org  (buscar: "cave ambient", "static hiss", "dripping water")
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Áudio Ambiente")]
    [SerializeField] private AudioClip ambientLoop;
    [SerializeField, Range(0f, 1f)] private float ambientVolume = 0.35f;

    [Header("Som de Insanidade")]
    [SerializeField] private AudioClip lowSanityHiss;
    [SerializeField, Range(0f, 1f)] private float hissMaxVolume = 0.6f;

    private AudioSource ambientSource;
    private AudioSource hissSource;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // Cria duas AudioSources para separar ambiente e chiado
        ambientSource = gameObject.AddComponent<AudioSource>();
        ambientSource.loop          = true;
        ambientSource.playOnAwake   = false;
        ambientSource.spatialBlend  = 0f; // 2D
        ambientSource.volume        = ambientVolume;

        hissSource = gameObject.AddComponent<AudioSource>();
        hissSource.loop         = true;
        hissSource.playOnAwake  = false;
        hissSource.spatialBlend = 0f;
        hissSource.volume       = 0f;
    }

    void Start()
    {
        if (SanitySystem.Instance != null)
            SanitySystem.Instance.OnSanityChanged.AddListener(OnSanityChanged);

        if (GameController.Instance != null)
        {
            // Só toca quando o jogo começar (não no menu)
        }
    }

    public void StartAmbient()
    {
        if (ambientLoop != null && !ambientSource.isPlaying)
        {
            ambientSource.clip = ambientLoop;
            ambientSource.Play();
        }

        if (lowSanityHiss != null && !hissSource.isPlaying)
        {
            hissSource.clip = lowSanityHiss;
            hissSource.volume = 0f;
            hissSource.Play();
        }
    }

    public void StopAmbient()
    {
        ambientSource.Stop();
        hissSource.Stop();
    }

    void OnSanityChanged(float normalizedSanity)
    {
        // Chiado aumenta conforme sanidade cai abaixo de 50%
        float hissTarget = Mathf.Lerp(hissMaxVolume, 0f, Mathf.Clamp01(normalizedSanity * 2f));
        hissSource.volume = Mathf.MoveTowards(hissSource.volume, hissTarget, Time.deltaTime * 0.5f);
    }
}
