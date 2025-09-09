using UnityEngine;

public class NpcTrigger : MonoBehaviour
{
    public Transform player;            // Referência ao player
    public Transform npcModel;          // Objeto que rotaciona (corpo ou cabeça do NPC)
    public float rotationSpeed = 5f;    // Velocidade da rotação

    [SerializeField]private bool playerInRange = false;
    private Quaternion initialRotation;

    void Start()
    {
        // Salva a rotação inicial do NPC
        if (npcModel == null) npcModel = transform;
        initialRotation = npcModel.rotation;
    }

    void Update()
    {
        if (playerInRange)
        {
            // Calcula direção até o player
            Vector3 direction = (player.position - npcModel.position).normalized;
            direction.y = 0; // Mantém apenas no eixo horizontal

            if (direction.magnitude > 0.01f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(-direction);
                npcModel.rotation = Quaternion.Slerp(npcModel.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }
        }
        else
        {
            // Volta para a rotação inicial
            npcModel.rotation = Quaternion.Slerp(npcModel.rotation, initialRotation, Time.deltaTime * rotationSpeed);
        }
    }

    // Quando o player entra na área
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    // Quando o player sai da área
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
