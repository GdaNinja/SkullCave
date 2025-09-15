using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractPuzzle : MonoBehaviour, IExtendedInteractable
{
    [SerializeField] private GameObject pinturaCompleta;
    public GameObject txtInteract;
    public bool puzzleActive;
    public bool puzzleCompleted;

    void Start()
    {
        txtInteract.SetActive(false);
        if (pinturaCompleta != null)
            pinturaCompleta.SetActive(false);
    }

    public void Interact()
    {
        if (!puzzleActive) // só abre se não estiver aberto
        {
            puzzleActive = true;
            // ativa puzzle
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            SceneManager.LoadSceneAsync("Puzzle", LoadSceneMode.Additive);
        }
    }

    void Update()
    {
        if (puzzleActive && !puzzleCompleted)
        {
            // tecla para sair sem terminar
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                exitInteract();
            }
        }
    }

    public void ShowTxt(bool show)
    {
        txtInteract.SetActive(show);
    }

    public void exitInteract()
    {
        PlayerStateManager player = FindFirstObjectByType<PlayerStateManager>();
        player.isInteracting = false;
        puzzleActive = false;
        SceneManager.UnloadSceneAsync("Puzzle");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnPuzzleComplete()
    {
        puzzleCompleted = true;

        if (pinturaCompleta != null)
            pinturaCompleta.SetActive(true);

        exitInteract(); // fecha o puzzle e volta ao jogo
    }
}
