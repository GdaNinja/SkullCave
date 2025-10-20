using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static EnemyAI;


public class EventTrigger : MonoBehaviour
{
    [Header("Configurações do Trigger")]
    [Tooltip("Tag do inimigo para detectar colisão")]
    public string enemyTag = "Enemy";
    
    [Header("Debug")]
    [Tooltip("Mostrar mensagens de debug no console")]
    public bool showDebugMessages = true;

    private void OnTriggerEnter(Collider other)
    {
        if (showDebugMessages)
        {
            Debug.Log($"[EventTrigger] Colidiu com: {other.gameObject.name} | Tag: {other.tag}");
        }
        
        // Verifica se é o inimigo pela tag
        if (other.CompareTag(enemyTag))
        {
            if (showDebugMessages)
            {
                Debug.Log($"[EventTrigger] INIMIGO DETECTADO: {other.gameObject.name}");
            }
            
            // Tenta pegar o EnemyAI do objeto que colidiu
            EnemyAI enemy = other.GetComponent<EnemyAI>();
            
            if (enemy != null)
            {
                if (showDebugMessages)
                {
                    Debug.Log("[EventTrigger] EnemyAI encontrado! Executando JumpScare...");
                }
                enemy.PlayJumpScareImage();
            }
            else
            {
                // Se o script não está no objeto que colidiu, procura nos pais
                enemy = other.GetComponentInParent<EnemyAI>();
                
                if (enemy != null)
                {
                    if (showDebugMessages)
                    {
                        Debug.Log("[EventTrigger] EnemyAI encontrado no objeto pai! Executando JumpScare...");
                    }
                    enemy.PlayJumpScareImage();
                }
                else
                {
                    Debug.LogError($"[EventTrigger] Objeto '{other.gameObject.name}' tem a tag '{enemyTag}' mas não tem o script EnemyAI!");
                }
            }
        }
    }
}