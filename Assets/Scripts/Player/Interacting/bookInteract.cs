using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class bookInteract : MonoBehaviour, IExtendedInteractable
{
    public GameObject txtBook, txtContinue, paredeDeletavel, smokeEffect;
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
            PlayerStateManager player = FindFirstObjectByType<PlayerStateManager>();
            dialogueStart = false;
            dialoguePanel.SetActive(false);
            player.isInteracting = false;
            txtContinue.SetActive(false);
            if (paredeDeletavel != null)
            {
                paredeDeletavel.SetActive(false);
                audioSuspense.Play();
            }
            if (smokeEffect != null)
                smokeEffect.SetActive(true);
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
}
