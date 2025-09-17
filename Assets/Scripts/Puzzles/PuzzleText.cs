using System.Collections;
using Unity.AppUI.UI;
using UnityEngine;

public class PuzzleText : MonoBehaviour
{
    public bool textStart;
    [SerializeField, TextArea(1, 6)] string[] textLines;
    [SerializeField] GameObject textPanel, textContinue;
    [SerializeField] TMPro.TMP_Text textTxt;
    private int index;
    private float timingChar = 0.05f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textStart = true;
        textContinue.SetActive(false);
        TextStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (!textStart)
        {
            TextStart();
        }
        else if (textTxt.text == textLines[index])
        {
            textContinue.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                textContinue.SetActive(true);
                NextText();
                textContinue.SetActive(false);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                StopAllCoroutines();
                textTxt.text = textLines[index];
            }
        }
    }
    void TextStart()
    {
        textStart = true;
        index = 0;
        StartCoroutine(ShowLines());
    }
    void NextText()
    {
        index++;
        if (index < textLines.Length)
        {
            StartCoroutine(ShowLines());
        }
        else
        {
            textStart = false;
            textPanel.SetActive(false);
        }
    }
    IEnumerator ShowLines()
    {
        textTxt.text = string.Empty;
        foreach (char ch in textLines[index])
        {
            textTxt.text += ch;
            yield return new WaitForSeconds(timingChar);
        }
    }
}
