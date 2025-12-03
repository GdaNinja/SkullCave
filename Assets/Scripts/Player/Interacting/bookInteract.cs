using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class bookInteract : MonoBehaviour, IExtendedInteractable
{
    public GameObject txtBook, txtContinue, paredeDeletavel, smokeEffect;
    public bool checkpointInteract;
    [SerializeField, TextArea(4, 6)] string[] dialogueLines;
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] TMP_Text dialogueTxt;
    private int index;
    private bool dialogueStart;
    private float timingChar = 0.03f;
    [SerializeField] AudioSource audioSuspense;

    void Start()
    {
        txtBook.SetActive(false);
        dialoguePanel.SetActive(false);
        txtContinue.SetActive(false);
        smokeEffect.SetActive(false);
        checkpointInteract = false;

        // NOVO: Verifica se a parede já foi deletada anteriormente
        if (paredeDeletavel != null && GameManager.Instance != null)
        {
            if (GameManager.Instance.ShouldObjectBeDeactivated(paredeDeletavel.name))
            {
                paredeDeletavel.SetActive(false);
                checkpointInteract = true;
                Debug.Log("[bookInteract] Parede já foi deletada anteriormente, mantendo desativada");
            }
        }
    }

    public void Interact()
    {
        if (!dialogueStart)
        {
            DialogueStart();
        }
        else if (dialogueTxt.text == dialogueLines[index])
        {
            NextDialogue();
        }
    }

    public void ShowTxt(bool show)
    {
        txtBook.SetActive(show);
    }

    void DialogueStart()
    {
        dialogueStart = true;
        dialoguePanel.SetActive(true);
        index = 0;
        StartCoroutine(ShowLines());
    }

    void NextDialogue()
    {
        index++;
        if (index < dialogueLines.Length)
        {
            StartCoroutine(ShowLines());
        }
        else
        {
            StartCoroutine(ParedeDeletavel());
        }
    }

    IEnumerator ShowLines()
    {
        dialogueTxt.text = string.Empty;
        foreach (char ch in dialogueLines[index])
        {
            dialogueTxt.text += ch;
            yield return new WaitForSecondsRealtime(timingChar);
        }
        txtContinue.SetActive(true);
    }

    IEnumerator ParedeDeletavel()
    {
        PlayerStateManager player = FindFirstObjectByType<PlayerStateManager>();
        checkpointInteract = true;
        dialogueStart = false;
        dialoguePanel.SetActive(false);
        player.isInteracting = false;
        txtContinue.SetActive(false);

        if (smokeEffect != null)
        {
            smokeEffect.SetActive(true);
            audioSuspense.Play();
        }

        yield return new WaitForSecondsRealtime(5);

        if (paredeDeletavel != null)
        {
            paredeDeletavel.SetActive(false);
            
            // NOVO: Registra no GameManager que a parede foi deletada
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RegisterDeactivatedObject(paredeDeletavel.name);
                Debug.Log($"[bookInteract] Parede '{paredeDeletavel.name}' registrada como deletada");
            }
        }
    }
}