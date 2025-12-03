using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerSpawn : MonoBehaviour
{
    [Header("Spawn Position")]
    public Transform defaultSpawnPoint;

    private void Start()
    {
        StartCoroutine(SpawnPlayer());
    }

    private IEnumerator SpawnPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player == null)
        {
            Debug.LogError("PLAYER NÃO ENCONTRADO!");
            yield break;
        }

        // DESABILITA TUDO ANTES
        var controller = player.GetComponent<CharacterController>();
        var stateManager = player.GetComponent<PlayerStateManager>();
        var playerCam = player.GetComponent<PlayerCam>();
        var interactController = player.GetComponent<InteractController>();
        
        if (controller) controller.enabled = false;
        if (stateManager) stateManager.enabled = false;
        if (playerCam) playerCam.enabled = false;
        if (interactController) interactController.enabled = false;

        // Decide posição
        Vector3 targetPos = defaultSpawnPoint.position;
        Quaternion targetRot = defaultSpawnPoint.rotation;
        
        if (GameManager.Instance != null && 
            GameManager.Instance.hasCheckpoint && 
            GameManager.Instance.checkpointSceneName == SceneManager.GetActiveScene().name)
        {
            targetPos = GameManager.Instance.lastCheckpointPosition;
            targetRot = GameManager.Instance.lastCheckpointRotation;
            Debug.Log($"[PlayerSpawn] Usando CHECKPOINT: {targetPos}");
        }
        else
        {
            Debug.Log($"[PlayerSpawn] Usando SPAWN INICIAL: {targetPos}");
        }

        // Move player e MANTÉM por 10 frames
        for (int i = 0; i < 10; i++)
        {
            player.transform.position = targetPos;
            player.transform.rotation = targetRot;
            yield return null;
        }
        
        // Zera a velocidade do CharacterController
        if (controller)
        {
            stateManager.velocity = Vector3.zero;
        }

        // REABILITA TUDO DEPOIS DE 0.5 SEGUNDOS
        yield return new WaitForSeconds(0.5f);
        
        if (controller) controller.enabled = true;
        if (stateManager) stateManager.enabled = true;
        if (playerCam) playerCam.enabled = true;
        if (interactController) interactController.enabled = true;
        
        Debug.Log($"[PlayerSpawn] Posição final: {player.transform.position}");
    }
}