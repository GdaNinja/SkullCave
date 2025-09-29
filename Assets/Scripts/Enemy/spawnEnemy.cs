using UnityEngine;
using System.Collections;

public class spawnEnemy : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    
    [Header("Camera Settings")]
    public Transform playerCamera; // Referência à câmera do player
    public float lookDuration = 2f; // Tempo que olha para o inimigo
    public float rotationSpeed = 3f; // Velocidade da rotação da câmera
    
    [Header("Enemy Behavior")]
    public float delayBeforeChase = 0.5f; // Delay antes do inimigo começar a perseguir
    
    private bool hasSpawned = false;
    private Quaternion originalCameraRotation;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasSpawned)
        {
            hasSpawned = true;
            StartCoroutine(SpawnEnemyCinematic());
        }
    }

    IEnumerator SpawnEnemyCinematic()
    {
        // Salva a rotação original da câmera
        originalCameraRotation = playerCamera.rotation;
        
        // Instancia o inimigo
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        
        // Desativa o movimento do player (se você tiver um script de movimento)
        PlayerStateManager playerController = playerCamera.GetComponentInParent<PlayerStateManager>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        
        // Calcula a direção para olhar para o inimigo
        Vector3 directionToEnemy = (spawnPoint.position - playerCamera.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy);
        
        // Rotaciona suavemente a câmera para o inimigo
        float elapsed = 0f;
        while (elapsed < lookDuration / 2)
        {
            playerCamera.rotation = Quaternion.Slerp(originalCameraRotation, targetRotation, elapsed / (lookDuration / 2));
            elapsed += Time.deltaTime * rotationSpeed;
            yield return null;
        }
        
        playerCamera.rotation = targetRotation;
        
        // Mantém olhando para o inimigo
        yield return new WaitForSeconds(lookDuration);
        
        // Rotaciona de volta para a direção original
        elapsed = 0f;
        while (elapsed < lookDuration / 2)
        {
            playerCamera.rotation = Quaternion.Slerp(targetRotation, originalCameraRotation, elapsed / (lookDuration / 2));
            elapsed += Time.deltaTime * rotationSpeed;
            yield return null;
        }
        
        playerCamera.rotation = originalCameraRotation;
        
        // Reativa o movimento do player
        if (playerController != null)
        {
            playerController.enabled = true;
        }
        
        // Aguarda um momento e ativa o comportamento de perseguição do inimigo
        yield return new WaitForSeconds(delayBeforeChase);
        
        // Ativa o script de IA do inimigo (exemplo)
    }
}