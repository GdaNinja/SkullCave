using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public GameObject prefab;
    public int totalCrystals = 0;

    private void Awake()
    {
        // Singleton que NÃO persiste entre cenas
        // (ele é recriado a cada cena)
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // NOVO: Restaura os cristais salvos quando a cena carrega
        RestoreInventory();
    }
    void Update()
    {
        if (totalCrystals == 4)
        {
            prefab.SetActive(false);
        }
    }

    public void AddCrystal(int amount)
    {
        totalCrystals += amount;
        Debug.Log($"[InventoryManager] Cristais: {totalCrystals}");
    }

 private void RestoreInventory()
    {
        // MUDANÇA: Agora restaura SEMPRE do GameManager (com ou sem checkpoint)
        if (GameManager.Instance != null)
        {
            totalCrystals = GameManager.Instance.totalCrystalsCollected;
            Debug.Log($"[InventoryManager] Inventário restaurado: {totalCrystals} cristais");
        }
        else
        {
            totalCrystals = 0;
            Debug.Log("[InventoryManager] GameManager não encontrado, iniciando com 0 cristais");
        }
    }
}