using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Checkpoint Settings")]
    public Vector3 lastCheckpointPosition;
    public Quaternion lastCheckpointRotation;
    public string checkpointSceneName;
    public bool hasCheckpoint = false;

    [Header("World State")]
    // Lista de objetos que devem ser desativados quando spawnar no checkpoint
    public List<string> deactivatedObjects = new List<string>();
    
    // Lista de cristais coletados (salva o nome único de cada cristal)
    public List<string> collectedCrystals = new List<string>();
    
    // Contagem total de cristais (ATUALIZADA EM TEMPO REAL)
    public int totalCrystalsCollected = 0;

    private void Awake()
    {
        // Implementa o padrão Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[GameManager] GameManager criado e configurado para persistir entre cenas");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Salva a posição e rotação do checkpoint
    public void SaveCheckpoint(Vector3 position, Quaternion rotation, string sceneName)
    {
        lastCheckpointPosition = position;
        lastCheckpointRotation = rotation;
        checkpointSceneName = sceneName;
        hasCheckpoint = true;
        
        Debug.Log($"[GameManager] Checkpoint salvo em: {position} na cena: {sceneName}");
        Debug.Log($"[GameManager] Cristais salvos no checkpoint: {totalCrystalsCollected}");
    }

    // Sobrecarga para manter compatibilidade (usa rotação padrão)
    public void SaveCheckpoint(Vector3 position, string sceneName)
    {
        SaveCheckpoint(position, Quaternion.identity, sceneName);
    }

    // Adiciona um objeto para ser desativado quando spawnar no checkpoint
    public void RegisterDeactivatedObject(string objectName)
    {
        if (!deactivatedObjects.Contains(objectName))
        {
            deactivatedObjects.Add(objectName);
            Debug.Log($"[GameManager] Objeto '{objectName}' registrado para desativação");
        }
    }

    // Remove um objeto da lista (caso precise reativar)
    public void UnregisterDeactivatedObject(string objectName)
    {
        if (deactivatedObjects.Contains(objectName))
        {
            deactivatedObjects.Remove(objectName);
            Debug.Log($"[GameManager] Objeto '{objectName}' removido da lista de desativação");
        }
    }

    // Verifica se um objeto deve estar desativado
    public bool ShouldObjectBeDeactivated(string objectName)
    {
        return deactivatedObjects.Contains(objectName);
    }

    // Registra um cristal como coletado (SALVA IMEDIATAMENTE)
    public void RegisterCollectedCrystal(string crystalName)
    {
        if (!collectedCrystals.Contains(crystalName))
        {
            collectedCrystals.Add(crystalName);
            totalCrystalsCollected++; // INCREMENTA A CONTAGEM PERMANENTE
            Debug.Log($"[GameManager] Cristal '{crystalName}' coletado! Total: {totalCrystalsCollected}");
        }
    }

    // Verifica se um cristal já foi coletado
    public bool IsCrystalCollected(string crystalName)
    {
        return collectedCrystals.Contains(crystalName);
    }

    // Reseta o checkpoint (útil para novo jogo)
    public void ResetCheckpoint()
    {
        hasCheckpoint = false;
        lastCheckpointPosition = Vector3.zero;
        lastCheckpointRotation = Quaternion.identity;
        checkpointSceneName = "";
        deactivatedObjects.Clear();
        collectedCrystals.Clear();
        totalCrystalsCollected = 0; // RESETA A CONTAGEM
        
        // Reseta também o inventário quando começa novo jogo
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.totalCrystals = 0;
            Debug.Log("[GameManager] Inventário resetado para novo jogo");
        }
        
        Debug.Log("[GameManager] Checkpoint resetado - Novo jogo iniciado");
    }

    // Carrega a cena do checkpoint
    public void LoadFromCheckpoint()
    {
        if (hasCheckpoint && !string.IsNullOrEmpty(checkpointSceneName))
        {
            Debug.Log($"[GameManager] Carregando checkpoint na cena: {checkpointSceneName}");
            SceneManager.LoadScene(checkpointSceneName);
        }
        else
        {
            Debug.LogWarning("[GameManager] Nenhum checkpoint disponível!");
        }
    }
}
