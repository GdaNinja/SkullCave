using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class spawnEnemy : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab, smokeSpawn; // Prefab do inimigo
    public Transform spawnPoint; // Ponto onde o inimigo vai spawnar (Objeto 2)
    public AudioSource Sound; // Fonte de áudio para o som de spawn
    
    [Header("Camera Settings")]
    public CinemachineVirtualCamera vcam; // Sua virtual camera
    public float lookDuration = 2f; // Quanto tempo olha para o inimigo
    public float rotationSpeed = 2f; // Velocidade da rotação
    
    [Header("Player References")]
    public PlayerStateManager playerState; // Referência ao seu PlayerStateManager
    public camInteracting camController; // Referência ao seu camInteracting
    
    private bool hasSpawned = false;
    private CinemachinePOV pov;
    private float originalHorizontalAngle;
    private float originalVerticalAngle;

    void Awake()
    {
        // Pega o componente POV da virtual camera
        pov = vcam.GetCinemachineComponent<CinemachinePOV>();
    }
    private void Start()
    {
        smokeSpawn.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        // Quando o player passa pela porta
        if (other.CompareTag("Player") && !hasSpawned)
        {
            hasSpawned = true;
            StartCoroutine(SpawnEnemyCinematic());
        }
    }

    IEnumerator SpawnEnemyCinematic()
    {
        // 1. SALVA OS ÂNGULOS ATUAIS DA CÂMERA
        originalHorizontalAngle = pov.m_HorizontalAxis.Value;
        originalVerticalAngle = pov.m_VerticalAxis.Value;
        
        // 2. TRAVA A CÂMERA (Player não consegue mais mexer)
        camController.LockCamera();
        
        // 3. DESABILITA O MOVIMENTO DO PLAYER
        PlayerBaseState originalState = playerState.currentState;
        playerState.SwitchState(playerState.interactingState);
        
        // 4. CALCULA O ÂNGULO PARA OLHAR PARA O SPAWN POINT
        Vector3 playerPos = playerState.transform.position;
        Vector3 directionToSpawn = spawnPoint.position - playerPos;
        
        // Calcula os ângulos necessários (Yaw = horizontal, Pitch = vertical)
        float targetYaw = Mathf.Atan2(directionToSpawn.x, directionToSpawn.z) * Mathf.Rad2Deg;
        float targetPitch = -Mathf.Atan2(directionToSpawn.y, 
        new Vector2(directionToSpawn.x, directionToSpawn.z).magnitude) * Mathf.Rad2Deg;
        
        // 5. ROTACIONA SUAVEMENTE PARA O SPAWN POINT
        float elapsed = 0f;
        float turnDuration = lookDuration / 3f; // 1/3 do tempo para girar

        while (elapsed < turnDuration)
        {
            elapsed += Time.deltaTime * rotationSpeed;
            float t = elapsed / turnDuration;

            pov.m_HorizontalAxis.Value = Mathf.LerpAngle(originalHorizontalAngle, targetYaw, t);
            pov.m_VerticalAxis.Value = Mathf.Lerp(originalVerticalAngle, targetPitch, t);

            yield return null;
        }
        
        // 6. SPAWNA O INIMIGO
        Sound.Play();
        smokeSpawn.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        
        // 7. MANTÉM OLHANDO PARA O INIMIGO
        yield return new WaitForSeconds(lookDuration);
        
        // 8. ROTACIONA DE VOLTA PARA A DIREÇÃO ORIGINAL
        elapsed = 0f;
        while (elapsed < turnDuration)
        {
            elapsed += Time.deltaTime * rotationSpeed;
            float t = elapsed / turnDuration;
            
            pov.m_HorizontalAxis.Value = Mathf.LerpAngle(targetYaw, originalHorizontalAngle, t);
            pov.m_VerticalAxis.Value = Mathf.Lerp(targetPitch, originalVerticalAngle, t);
            
            yield return null;
        }
        
        // 9. DESTRAVA A CÂMERA
        camController.UnlockCamera();
        
        // 10. REATIVA O MOVIMENTO DO PLAYER
        // Volta para idle para recomeçar normalmente
        playerState.isInteracting = false;
        
        // OPCIONAL: Se você quiser que o inimigo faça algo depois, use aqui
        // Exemplo: enemy.GetComponent<EnemyAI>().StartChase();
    }
}