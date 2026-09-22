using UnityEngine;

/// <summary>
/// Oblivion - WrongPathTrigger (versão 3D)
/// Coloque num GameObject com BoxCollider (Is Trigger = true) num caminho
/// errado do labirinto. Ao entrar, o jogador perde sanidade.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class WrongPathTrigger : MonoBehaviour
{
    [SerializeField] private float sanityDrainAmount = 12f;
    [SerializeField] private bool oneTimeOnly = true;

    private bool triggered = false;

    void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (oneTimeOnly && triggered) return;
        triggered = true;
        SanitySystem.Instance?.DrainSanity(sanityDrainAmount);
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        var col = GetComponent<BoxCollider>();
        if (col == null) return;
        Gizmos.color = new Color(1f, 0.3f, 0f, 0.4f);
        Gizmos.DrawCube(transform.position + col.center, col.size);
        Gizmos.color = new Color(1f, 0.5f, 0f, 1f);
        Gizmos.DrawWireCube(transform.position + col.center, col.size);
    }
#endif
}
