using UnityEngine;

/// <summary>
/// Oblivion - CameraEffects
/// Substitui o antigo CameraFollow.cs (que era da câmera top-down 2D).
/// Fica na câmera (filha do Player). Cuida de:
///  - Head bob: balanço leve ao caminhar/correr.
///  - Shake: usado pelo SanityHUD quando a sanidade está crítica.
/// Ambos mexem só em localPosition, então não conflitam com o MouseLook
/// (que mexe só em localRotation).
///
/// SETUP:
/// 1. Anexe este script na Main Camera (filha do Player).
/// 2. Arraste o CharacterController do Player em "Player Controller"
///    (opcional — sem isso o head bob simplesmente fica desativado).
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraEffects : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private CharacterController playerController;

    [Header("Head Bob")]
    [SerializeField] private bool enableHeadBob = true;
    [SerializeField] private float bobFrequency = 1.8f;
    [SerializeField] private float bobAmount = 0.035f;
    [SerializeField] private float sprintBobMultiplier = 1.4f;

    /// <summary>Deslocamento de shake, controlado externamente (ex.: SanityHUD).</summary>
    public Vector3 ShakeOffset { get; set; } = Vector3.zero;

    private Vector3 restLocalPos;
    private float bobTimer;

    void Start()
    {
        restLocalPos = transform.localPosition;

        if (playerController == null)
            playerController = GetComponentInParent<CharacterController>();
    }

    void LateUpdate()
    {
        Vector3 bobOffset = Vector3.zero;

        if (enableHeadBob && playerController != null)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            bool moving = (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f) && playerController.isGrounded;
            bool sprinting = Input.GetKey(KeyCode.LeftShift);

            if (moving)
            {
                float freq = bobFrequency * (sprinting ? sprintBobMultiplier : 1f);
                bobTimer += Time.deltaTime * freq * 2f * Mathf.PI;
                bobOffset = new Vector3(0f, Mathf.Sin(bobTimer) * bobAmount, 0f);
            }
            else
            {
                bobTimer = 0f;
            }
        }

        transform.localPosition = restLocalPos + bobOffset + ShakeOffset;
    }
}
