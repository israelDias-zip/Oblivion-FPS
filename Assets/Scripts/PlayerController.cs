using UnityEngine;

/// <summary>
/// Oblivion - PlayerController (Primeira Pessoa)
/// Movimento em primeira pessoa usando CharacterController.
/// A rotação horizontal (yaw) do corpo é aplicada pelo MouseLook.cs
/// (que fica na câmera filha e gira este Transform).
///
/// SETUP:
/// 1. Crie um GameObject vazio "Player".
/// 2. Adicione um CharacterController (Height ~1.8, Radius ~0.4, Center Y ~0.9).
/// 3. Adicione este script e marque a Tag "Player".
/// 4. Como filho do Player, posicione a Main Camera na altura dos olhos
///    (Local Position Y ~1.6) e adicione MouseLook.cs + CameraEffects.cs nela.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float walkSpeed = 3.2f;
    [SerializeField] private float sprintSpeed = 5.5f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Áudio de Passos (opcional)")]
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float stepInterval = 0.5f;

    private CharacterController controller;
    private Vector3 velocity;
    private float stepTimer;

    /// <summary>Evento estático — outros sistemas (ex.: futura versão 3D do MapMemorySystem) podem escutar.</summary>
    public static event System.Action<Vector3> OnPlayerMoved;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Pausa o movimento fora do estado "Playing" (menu, game over, vitória)
        if (GameController.Instance != null && GameController.Instance.State != GameController.GameState.Playing)
            return;

        HandleMovement();
    }

    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        bool sprinting = Input.GetKey(KeyCode.LeftShift) && v > 0f;

        Vector3 inputDir = (transform.right * h + transform.forward * v);
        inputDir = Vector3.ClampMagnitude(inputDir, 1f);

        float speed = sprinting ? sprintSpeed : walkSpeed;
        Vector3 horizontalMove = inputDir * speed;

        // Gravidade simples (mantém o CharacterController "grudado" no chão)
        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMove = horizontalMove + Vector3.up * velocity.y;
        controller.Move(finalMove * Time.deltaTime);

        if (inputDir.sqrMagnitude > 0.001f)
        {
            OnPlayerMoved?.Invoke(transform.position);
            HandleFootsteps(sprinting);
        }
        else
        {
            stepTimer = 0f;
        }
    }

    void HandleFootsteps(bool sprinting)
    {
        if (footstepSource == null || footstepClips == null || footstepClips.Length == 0) return;

        stepTimer -= Time.deltaTime * (sprinting ? 1.6f : 1f);
        if (stepTimer <= 0f)
        {
            stepTimer = stepInterval;
            var clip = footstepClips[Random.Range(0, footstepClips.Length)];
            footstepSource.PlayOneShot(clip, 0.6f);
        }
    }

    // ── Público ──────────────────────────────────────────────────────────
    public void SetWalkSpeed(float speed) => walkSpeed = speed;
    public bool IsGrounded => controller.isGrounded;
}
