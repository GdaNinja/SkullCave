using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractPuzzle : MonoBehaviour, IExtendedInteractable
{
    [SerializeField] private GameObject pinturaCompleta, imgPintura, gameObjectToEnable, smokeEffect; 
    public GameObject txtInteract;


    [HideInInspector] public bool puzzleActive; // está aberto agora?
    [HideInInspector] public bool puzzleCompleted; // já terminou antes?


    void Start()
    {
        txtInteract.SetActive(false);
        if (pinturaCompleta != null)
            pinturaCompleta.SetActive(false);
        if (gameObjectToEnable != null)
            gameObjectToEnable.SetActive(false);
        if (smokeEffect != null)
            smokeEffect.SetActive(false);
    }


    // chamada quando o jogador interage com a moldura (ex: aperta E)
    public void Interact()
    {
        // só abre se não estiver aberto e se ainda não tiver sido completado antes
        if (!puzzleActive && !puzzleCompleted)
        {
            puzzleActive = true;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            // carrega a cena do puzzle em modo additivo
            SceneManager.LoadSceneAsync("Puzzle", LoadSceneMode.Additive);
        }
    }


    void Update()
    {
        // se o puzzle estiver aberto e ainda não estiver completo, permite fechar com Q
        if (puzzleActive && !puzzleCompleted)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                exitInteract();
            }
        }
    }


    public void ShowTxt(bool show)
    {
        txtInteract.SetActive(show);
    }


    // fecha o puzzle e volta o controle ao player
    public void exitInteract()
    {
        PlayerStateManager player = FindFirstObjectByType<PlayerStateManager>();
        if (player != null) player.isInteracting = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        puzzleActive = false;
        // descarrega a cena do puzzle (se estiver carregada)
        SceneManager.UnloadSceneAsync("Puzzle");

    }
    public void OnPuzzleComplete()
    {
        puzzleCompleted = true;

        if (pinturaCompleta != null)
            pinturaCompleta.SetActive(true);
            if (imgPintura != null)
            imgPintura.SetActive(false);
            if (gameObjectToEnable != null)
            gameObjectToEnable.SetActive(true);
            if (smokeEffect != null)
            smokeEffect.SetActive(true);
        exitInteract(); // fecha o puzzle e volta ao jogo
    }
}