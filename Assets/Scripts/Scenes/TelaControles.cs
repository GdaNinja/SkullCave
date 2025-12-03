using UnityEngine;

public class TelaControles : MonoBehaviour
{
    public GameObject telaControles;
    public bool controlesAtivos = false;
    public Transform playerTransform;
    public Vector3 playerPosition = new Vector3(0, 0.6f, 0);
    
    private bool jaChecou = false; // NOVO: evita checar múltiplas vezes

    void Start()
    {
        // CORREÇÃO: Só mostra controles se NÃO TEM checkpoint salvo
        // Se tem checkpoint, o player já sabe jogar!
        bool temCheckpoint = GameManager.Instance != null && GameManager.Instance.hasCheckpoint;
        
        if (!temCheckpoint && Vector3.Distance(playerTransform.position, playerPosition) < 1f)
        {
            controlesAtivos = true;
            telaControles.SetActive(true);
            jaChecou = true;
        }
        else
        {
            controlesAtivos = false;
            telaControles.SetActive(false);
            jaChecou = true;
        }
    }

    void Update()
    {
        if (controlesAtivos)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
        }
    }
    
    public void fecharControles()
    {
        controlesAtivos = false;
        telaControles.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}