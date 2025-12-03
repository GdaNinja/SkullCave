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

    [Header("Save Data")]
    public int savedCrystalCount = 0;

    // Objetos destruídos/desativados
    private HashSet<string> deactivatedObjects = new HashSet<string>();

    // Cristais coletados
    private HashSet<string> collectedCrystals = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // -----------------------------
    // CHECKPOINT
    // -----------------------------
    public void SaveCheckpoint(Vector3 position, Quaternion rotation, string sceneName)
    {
        lastCheckpointPosition = position;
        lastCheckpointRotation = rotation;
        checkpointSceneName = sceneName;
        hasCheckpoint = true;

        // Salva quantidade de cristais
        savedCrystalCount = InventoryManager.Instance.totalCrystals;
    }

    public void ResetCheckpoint()
    {
        hasCheckpoint = false;
        lastCheckpointPosition = Vector3.zero;
        lastCheckpointRotation = Quaternion.identity;
        checkpointSceneName = "";
        savedCrystalCount = 0;

        deactivatedObjects.Clear();
        collectedCrystals.Clear();
    }

    public void LoadFromCheckpoint()
    {
        if (hasCheckpoint && !string.IsNullOrEmpty(checkpointSceneName))
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(checkpointSceneName);
        }
    }

    // Quando a cena carregar, restaurar tudo
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        RestoreDestroyedObjects();
        RestoreCrystals();
    }

    // -----------------------------
    // OBJETOS DELETADOS
    // -----------------------------
    public void RegisterDeactivatedObject(string objectName)
    {
        if (!deactivatedObjects.Contains(objectName))
            deactivatedObjects.Add(objectName);
    }

    public bool ShouldObjectBeDeactivated(string objectName)
    {
        return deactivatedObjects.Contains(objectName);
    }

    private void RestoreDestroyedObjects()
    {
        foreach (string objName in deactivatedObjects)
        {
            GameObject obj = GameObject.Find(objName);

            if (obj != null)
            {
                MeshRenderer mr = obj.GetComponent<MeshRenderer>();
                Collider col = obj.GetComponent<Collider>();

                if (mr) mr.enabled = false;
                if (col) col.enabled = false;
            }
        }
    }

    // -----------------------------
    // CRISTAIS COLETADOS
    // -----------------------------
    public void RegisterCollectedCrystal(string crystalName)
    {
        if (!collectedCrystals.Contains(crystalName))
            collectedCrystals.Add(crystalName);
    }

    public bool IsCrystalCollected(string crystalName)
    {
        return collectedCrystals.Contains(crystalName);
    }

    private void RestoreCrystals()
    {
        foreach (string crystalName in collectedCrystals)
        {
            GameObject obj = GameObject.Find(crystalName);

            if (obj != null)
            {
                MeshRenderer mr = obj.GetComponent<MeshRenderer>();
                Collider col = obj.GetComponent<Collider>();

                if (mr) mr.enabled = false;
                if (col) col.enabled = false;
            }
        }
    }
}
