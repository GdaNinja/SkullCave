using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;

public class NpcInteracting : MonoBehaviour, IInteractable
{
    public GameObject txtNpc;
    [SerializeField, TextArea(4, 6)] string[] dialogueLines;
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] TMP_Text dialogueTxt;
    private int index;
    private bool dialogueStart;
    private float timingChar = 0.1f;

    void Start()
    {
        txtNpc.SetActive(false);
        dialoguePanel.SetActive(false);
    }

    public void Interact()
    {
        if (!dialogueStart)
        {
            DialogueStart();
        }
        if (Input.GetButtonDown("Fire1"))
        {
            if (dialogueTxt.text == dialogueLines[index])
            {
                NextDialogue();
            }
            else
            {
                StopAllCoroutines();
                dialogueTxt.text = dialogueLines[index];
            }
        }
    }
    public void ShowTxt(bool show)
    {
        txtNpc.SetActive(show);
    }
    void DialogueStart()
    {
        dialogueStart = true;
        dialoguePanel.SetActive(true);
        txtNpc.SetActive(false);
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
            dialogueStart = false;
            dialoguePanel.SetActive(false);
            txtNpc.SetActive(true);
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
    }
}
