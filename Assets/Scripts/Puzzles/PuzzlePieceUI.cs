// 1. PuzzlePieceUI.cs - Script para cada peça UI individual
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PuzzlePieceUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Configuração da Peça")]
    public int pieceID; // ID único da peça (0 = espaço vazio)
    public int correctPosition; // posição correta desta peça

    [Header("Componentes")]
    public Image pieceImage; // componente Image da peça

    private PuzzleManagerUI puzzleManager;
    private Vector2Int gridPosition;
    private RectTransform rectTransform;

    void Start()
    {
        puzzleManager = FindFirstObjectByType<PuzzleManagerUI>();
        rectTransform = GetComponent<RectTransform>();

        if (pieceImage == null)
            pieceImage = GetComponent<Image>();

        // Se for o espaço vazio, torna invisível
        if (pieceID == 0)
        {
            if (pieceImage != null)
                pieceImage.color = Color.clear; // transparente
            gameObject.SetActive(false); // ou desativa completamente
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (puzzleManager != null && pieceID != 0) // não clica no espaço vazio
        {
            puzzleManager.OnPieceClicked(this);
        }
    }

    public void SetGridPosition(Vector2Int pos)
    {
        gridPosition = pos;
    }

    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }

    public void MoveTo(Vector2 targetPosition)
    {
        StartCoroutine(MoveToPosition(targetPosition));
    }

    private IEnumerator MoveToPosition(Vector2 target)
    {
        Vector2 startPos = rectTransform.anchoredPosition;
        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = Mathf.SmoothStep(0f, 1f, t); // suaviza a animação

            rectTransform.anchoredPosition = Vector2.Lerp(startPos, target, t);
            yield return null;
        }

        rectTransform.anchoredPosition = target;
    }

    // Método para destacar peças que podem se mover
    public void SetHighlight(bool highlight)
    {
        if (pieceImage != null && pieceID != 0)
        {
            pieceImage.color = highlight ? Color.yellow : Color.white;
        }
    }
}
