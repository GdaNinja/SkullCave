using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class JumpScareManager : MonoBehaviour
{
    // Singleton para acesso global
    public static JumpScareManager Instance { get; private set; }

    [Header("Referências do JumpScare")]
    [Tooltip("Imagem do JumpScare (Canvas UI)")]
    public GameObject jumpScareImage;
    
    [Tooltip("Som do JumpScare")]
    public AudioSource jumpScareSound;

    [Header("Configurações")]
    [Tooltip("Tempo de espera antes de carregar a cena de Game Over")]
    public float waitTimeBeforeGameOver = 5f;
    
    [Tooltip("Nome da cena de Game Over")]
    public string gameOverSceneName = "GameOver";

    [Header("Debug")]
    public bool showDebugMessages = true;

    private bool jumpScareTriggered = false;

    private void Awake()
    {
        // Implementa o padrão Singleton
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Descomente se quiser que persista entre cenas
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Garante que a imagem começa desativada
        if (jumpScareImage != null)
        {
            jumpScareImage.SetActive(false);
        }
    }

    private void Start()
    {
        // Validação das referências
        if (jumpScareImage == null)
        {
            Debug.LogError("[JumpScareManager] jumpScareImage não foi atribuído! Atribua no Inspector.");
        }

        if (jumpScareSound == null)
        {
            Debug.LogWarning("[JumpScareManager] jumpScareSound não foi atribuído! O som não tocará.");
        }
    }

    /// <summary>
    /// Método público chamado pelos inimigos para disparar o jumpscare
    /// Garante que só aconteça uma vez
    /// </summary>
    public void TriggerJumpScare()
    {
        // Previne múltiplos jumpscares
        if (jumpScareTriggered)
        {
            if (showDebugMessages)
            {
                Debug.Log("[JumpScareManager] JumpScare já foi disparado, ignorando...");
            }
            return;
        }

        jumpScareTriggered = true;

        if (showDebugMessages)
        {
            Debug.Log("[JumpScareManager] ========== JUMPSCARE DISPARADO! ==========");
        }

        // Ativa a imagem
        if (jumpScareImage != null)
        {
            jumpScareImage.SetActive(true);
            if (showDebugMessages)
            {
                Debug.Log("[JumpScareManager] Imagem do JumpScare ativada!");
            }
        }
        else
        {
            Debug.LogError("[JumpScareManager] jumpScareImage é NULL! Não foi possível ativar.");
        }

        // Toca o som
        if (jumpScareSound != null)
        {
            jumpScareSound.Play();
            if (showDebugMessages)
            {
                Debug.Log("[JumpScareManager] Som do JumpScare tocando!");
            }
        }
        else
        {
            Debug.LogWarning("[JumpScareManager] jumpScareSound é NULL! Som não tocará.");
        }

        // Inicia corrotina para carregar a cena de Game Over
        StartCoroutine(LoadGameOverScene());
    }

    private IEnumerator LoadGameOverScene()
{
    if (showDebugMessages)
    {
        Debug.Log($"[JumpScareManager] Aguardando {waitTimeBeforeGameOver} segundos...");
    }

    yield return new WaitForSeconds(waitTimeBeforeGameOver);

    if (showDebugMessages)
    {
        Debug.Log($"[JumpScareManager] Carregando cena '{gameOverSceneName}'...");
    }

    // IMPORTANTE: NÃO reseta o checkpoint aqui!
    // O checkpoint só é resetado quando o jogador clica em "Novo Jogo"
    
    try
    {
        SceneManager.LoadScene(gameOverSceneName);
    }
    catch (System.Exception e)
    {
        Debug.LogError($"[JumpScareManager] Erro ao carregar cena '{gameOverSceneName}': {e.Message}");
    }
}

    /// <summary>
    /// Reseta o jumpscare (útil para testes)
    /// </summary>
    public void ResetJumpScare()
    {
        jumpScareTriggered = false;
        if (jumpScareImage != null)
        {
            jumpScareImage.SetActive(false);
        }
        if (showDebugMessages)
        {
            Debug.Log("[JumpScareManager] JumpScare resetado!");
        }
    }
}
