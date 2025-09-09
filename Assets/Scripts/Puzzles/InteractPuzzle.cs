using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InteractPuzzle : MonoBehaviour, IExtendedInteractable
{
    [SerializeField] private GameObject pinturaCompleta;
    public GameObject txtInteract;
    PlayerStateManager player;
    public bool puzzleActive = false; // controla se o puzzle está ativo

    void Start()
    {
        txtInteract.SetActive(false);
        player = FindFirstObjectByType<PlayerStateManager>();
    }
    public void Interact()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        puzzleActive = true;
        if (player.isInteracting == true && Input.GetKeyDown(KeyCode.E))
        {
            exitInteract();
        }
    }
    public void ShowTxt(bool show)
    {
        txtInteract.SetActive(show);
    }
    public void exitInteract()
    {
        puzzleActive = false;
        player.isInteracting = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void OnPuzzleComplete()
    {
        if (pinturaCompleta != null)
            pinturaCompleta.SetActive(true);

        exitInteract(); // volta o controle ao player
    }
}
