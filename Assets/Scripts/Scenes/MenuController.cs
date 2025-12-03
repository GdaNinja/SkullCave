using System.Collections;
using System.Collections.Generic;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject menuInitial, menuOptions;

    void Start()
    {
        menuOptions.SetActive(false);
    }

    // Update is called once per frame

    public void NewGame()
    {
        // Reseta o checkpoint ao iniciar novo jogo
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetCheckpoint();
        }
        // IMPORTANTE: Volta o Time.timeScale para 1 (caso tenha pausado)
        Time.timeScale = 1f;
        
        SceneManager.LoadScene("CutScene");
    }
     public void ContinueGame()
    {
        // Carrega do último checkpoint
        if (GameManager.Instance != null && GameManager.Instance.hasCheckpoint)
        {
            GameManager.Instance.LoadFromCheckpoint();
        }
        else
        {
            Debug.LogWarning("Nenhum checkpoint salvo!");
            // Opcional: mostrar mensagem na UI
        }
    }

    public void Options()
    {
        menuInitial.SetActive(false);
        menuOptions.SetActive(true);
    }
    public void Salvar()
    {
        SaveConfigs();
        ReturnMenu();
    }
    public void ReturnMenu()
    {
        menuInitial.SetActive(true);
        menuOptions.SetActive(false);
    }
    private void SaveConfigs()
    {
        //Salvar configurações
    }
    public void Exit()
    {
        Application.Quit();
    }
    public void RestartFromCheckpoint()
    {
        Debug.Log("[MenuController] RestartFromCheckpoint chamado!");
        
        // Volta o time scale para normal
        Time.timeScale = 1f;
        
        if (GameManager.Instance != null && GameManager.Instance.hasCheckpoint)
        {
            Debug.Log("[MenuController] Carregando do checkpoint...");
            GameManager.Instance.LoadFromCheckpoint();
        }
        else
        {
            Debug.LogWarning("[MenuController] Nenhum checkpoint disponível! Reiniciando a cena atual...");
            // Se não tem checkpoint, recarrega a cena atual
            SceneManager.LoadScene("SampleScene");
        }
    }
}
