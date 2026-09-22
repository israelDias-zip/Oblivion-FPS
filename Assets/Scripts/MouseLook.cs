using UnityEngine;

/// <summary>
/// Oblivion - MouseLook
/// Anexe à câmera (filha do Player). Controla o olhar em primeira pessoa:
///  - Pitch (cima/baixo) é aplicado na própria câmera.
///  - Yaw (esquerda/direita) é aplicado no corpo do Player ("Player Body").
///
/// SETUP:
/// 1. Arraste o Transform do "Player" (o pai) no campo "Player Body"
///    (se deixar em branco, ele usa o pai automaticamente).
/// 2. O cursor trava/destrava sozinho de acordo com o estado do GameController.
/// </summary>
public class MouseLook : MonoBehaviour
{
    [Header("Sensibilidade")]
    [SerializeField] private float mouseSensitivity = 2.2f;
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;

    [Header("Referências")]
    [SerializeField] private Transform playerBody;

    private float pitch;

    void Start()
    {
        if (playerBody == null && transform.parent != null)
            playerBody = transform.parent;
    }

    void Update()
    {
        bool playing = GameController.Instance == null
            || GameController.Instance.State == GameController.GameState.Playing;

        Cursor.lockState = playing ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !playing;

        if (!playing) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        pitch = Mathf.Clamp(pitch - mouseY, minPitch, maxPitch);
        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        if (playerBody != null)
            playerBody.Rotate(Vector3.up * mouseX);
    }
}
