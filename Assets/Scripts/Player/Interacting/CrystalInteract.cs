using System.Collections;
using UnityEngine;
using TMPro;

public class CrystalInteract : MonoBehaviour, IInteractable
{
    public GameObject textInteract, textCount, crystalImage;

    public void Start()
    {
        textInteract.SetActive(false);
        textCount.SetActive(false);
        crystalImage.SetActive(false);

        // NOVO: Verifica se este cristal já foi coletado
        if (GameManager.Instance != null && GameManager.Instance.IsCrystalCollected(gameObject.name))
        {
            // Já foi coletado, desativa o cristal
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
            Debug.Log($"[CrystalInteract] Cristal '{gameObject.name}' já foi coletado, mantendo desativado");
        }
    }

    public void Interact()
    {
        // Só permite coletar se ainda não foi coletado
        if (GameManager.Instance == null || !GameManager.Instance.IsCrystalCollected(gameObject.name))
        {
            CollectCrystal();
        }
    }

    public void ShowTxt(bool show)
    {
        textInteract.SetActive(show);
    }

    public void CollectCrystal()
    {
        StartCoroutine(CountCrystal());
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        // NOVO: Registra no GameManager que este cristal foi coletado
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterCollectedCrystal(gameObject.name);
            Debug.Log($"[CrystalInteract] Cristal '{gameObject.name}' registrado no GameManager");
        }
    }

    public IEnumerator CountCrystal()
    {
        crystalImage.SetActive(true);
        textCount.SetActive(true);

        InventoryManager.Instance.AddCrystal(1);

        textCount.GetComponent<TMP_Text>().text =
            InventoryManager.Instance.totalCrystals.ToString();

        yield return new WaitForSeconds(5f);

        crystalImage.SetActive(false);
        textCount.SetActive(false);
    }
}