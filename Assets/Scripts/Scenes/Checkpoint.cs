using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    [Header("Checkpoint Visual")]
    public Color inactiveColor = Color.gray;
    public Color activeColor = Color.green;
    private Renderer meshRenderer;
    private bool isActivated = false;

    [Header("Spawn Settings")]
    [Tooltip("Rotação que o player terá ao spawnar neste checkpoint")]
    public Transform spawnDirection; // Opcional: um objeto vazio apontando para onde o player deve olhar

    private void Start()
    {
        meshRenderer = GetComponent<Renderer>();
        if (meshRenderer != null)
        {
            meshRenderer.material.color = inactiveColor;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player") && !isActivated)
        {
            ActivateCheckpoint();
        }
    }

    private void ActivateCheckpoint()
    {
        isActivated = true;

        // Salva o checkpoint no GameManager
        string currentScene = SceneManager.GetActiveScene().name;

        // Se tem direção definida, usa ela; senão usa a rotação atual do checkpoint
        Quaternion spawnRotation = spawnDirection != null ? spawnDirection.rotation : transform.rotation;

        GameManager.Instance.SaveCheckpoint(transform.position, spawnRotation, currentScene);

        // Feedback visual
        if (meshRenderer != null)
        {
            meshRenderer.material.color = activeColor;
        }

        Debug.Log("[Checkpoint] Checkpoint ativado automaticamente!");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isActivated ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1f);

        // Desenha uma seta mostrando a direção do spawn
        if (spawnDirection != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, spawnDirection.forward * 2f);
        }
    }
}

