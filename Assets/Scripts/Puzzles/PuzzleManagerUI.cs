using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManagerUI : MonoBehaviour
{
    [Header("Peças do Puzzle")]
    public PuzzlePieceUI[] puzzlePieces;

    [Header("UI References")]
    public GameObject completePanel;

    // ✅ SISTEMA SUPER SIMPLES
    private PuzzlePieceUI[,] grid = new PuzzlePieceUI[3, 2];
    private Vector2Int emptyPos;
    private bool canMove = true;
    
    // ✅ POSIÇÕES FIXAS ONDE AS PEÇAS DEVEM FICAR
    private Vector2[] fixedPositions = new Vector2[6];

    void Start()
    {
        completePanel.SetActive(false);
        SetupSimple();
    }

    void SetupSimple()
    {
        Debug.Log("=== SETUP SIMPLES ===");
        
        // ✅ CAPTURA AS POSIÇÕES ONDE AS PEÇAS ESTÃO AGORA
        CapturePositions();
        
        // ✅ MAPEIA CADA PEÇA PARA O GRID
        MapPieces();
        
        Debug.Log("Setup concluído!");
        DebugGrid();
    }

    void CapturePositions()
    {
        Debug.Log("📍 CAPTURANDO POSIÇÕES:");
        
        // ✅ ORGANIZA AS PEÇAS POR POSIÇÃO VISUAL
        List<(Vector2 pos, PuzzlePieceUI piece)> pieces = new List<(Vector2, PuzzlePieceUI)>();
        
        foreach (var piece in puzzlePieces)
        {
            if (piece != null)
            {
                Vector2 pos = piece.GetComponent<RectTransform>().anchoredPosition;
                pieces.Add((pos, piece));
                Debug.Log($"Peça ID {piece.pieceID} em {pos}");
            }
        }
        
        // ✅ ORDENA: PRIMEIRO POR Y (CIMA PARA BAIXO), DEPOIS POR X (ESQUERDA PARA DIREITA)
        pieces.Sort((a, b) => {
            if (Mathf.Abs(a.pos.y - b.pos.y) > 10f) // Se Y é diferente
                return b.pos.y.CompareTo(a.pos.y); // Maior Y primeiro (linha de cima)
            return a.pos.x.CompareTo(b.pos.x); // Menor X primeiro (esquerda)
        });
        
        // ✅ ARMAZENA AS POSIÇÕES EM ORDEM
        for (int i = 0; i < Mathf.Min(6, pieces.Count); i++)
        {
            fixedPositions[i] = pieces[i].pos;
            Debug.Log($"Posição {i}: {fixedPositions[i]}");
        }
    }

    void MapPieces()
    {
        Debug.Log("🗺️ MAPEANDO PEÇAS:");
        
        // ✅ ESTADO ATUAL INFORMADO: 2 1 5 / 4 3 0
        foreach (var piece in puzzlePieces)
        {
            if (piece == null) continue;
            
            Vector2Int gridPos = Vector2Int.zero;
            
            switch (piece.pieceID)
            {
                case 2: gridPos = new Vector2Int(0, 0); break; // Superior esquerdo
                case 1: gridPos = new Vector2Int(1, 0); break; // Superior meio  
                case 5: gridPos = new Vector2Int(2, 0); break; // Superior direito
                case 4: gridPos = new Vector2Int(0, 1); break; // Inferior esquerdo
                case 3: gridPos = new Vector2Int(1, 1); break; // Inferior meio
                case 0: // Espaço vazio
                    gridPos = new Vector2Int(2, 1); // Inferior direito
                    emptyPos = gridPos;
                    piece.gameObject.SetActive(false);
                    break;
            }
            
            piece.SetGridPosition(gridPos);
            grid[gridPos.x, gridPos.y] = piece;
            
            Debug.Log($"Peça ID {piece.pieceID} → Grid[{gridPos.x},{gridPos.y}]");
        }
        
        Debug.Log($"Espaço vazio: Grid[{emptyPos.x},{emptyPos.y}]");
    }

    // ========================================
    // MOVIMENTO SUPER DIRETO
    // ========================================

    public void OnPieceClicked(PuzzlePieceUI piece)
    {
        if (!canMove) return;
        
        Debug.Log($"🖱️ CLICOU NA PEÇA ID {piece.pieceID}");
        
        Vector2Int piecePos = piece.GetGridPosition();
        Debug.Log($"Peça em Grid[{piecePos.x},{piecePos.y}], vazio em Grid[{emptyPos.x},{emptyPos.y}]");
        
        // ✅ VERIFICA SE É ADJACENTE
        if (IsAdjacent(piecePos))
        {
            Debug.Log("✅ ADJACENTE - MOVENDO AGORA!");
            MoveNow(piece, piecePos);
        }
        else
        {
            Debug.Log("❌ NÃO ADJACENTE");
            ShowAdjacent();
        }
    }

    bool IsAdjacent(Vector2Int piecePos)
    {
        int dx = Mathf.Abs(piecePos.x - emptyPos.x);
        int dy = Mathf.Abs(piecePos.y - emptyPos.y);
        
        bool adj = (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
        Debug.Log($"Distâncias: dx={dx}, dy={dy} → Adjacente: {adj}");
        
        return adj;
    }

    void ShowAdjacent()
    {
        Debug.Log("🔍 PEÇAS QUE PODEM MOVER:");
        
        // Checa todas as direções
        Vector2Int[] dirs = {
            new Vector2Int(-1, 0), new Vector2Int(1, 0), // esquerda, direita
            new Vector2Int(0, -1), new Vector2Int(0, 1)  // cima, baixo  
        };
        
        foreach (var dir in dirs)
        {
            Vector2Int checkPos = emptyPos + dir;
            if (IsValid(checkPos))
            {
                var piece = grid[checkPos.x, checkPos.y];
                if (piece != null)
                {
                    Debug.Log($"  ✅ Peça ID {piece.pieceID} em Grid[{checkPos.x},{checkPos.y}]");
                }
            }
        }
    }

    bool IsValid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < 3 && pos.y >= 0 && pos.y < 2;
    }

    void MoveNow(PuzzlePieceUI piece, Vector2Int piecePos)
    {
        Debug.Log($"🔄 MOVENDO PEÇA ID {piece.pieceID}");
        
        // ✅ CALCULA QUAL SLOT A PEÇA VAI
        int targetSlot = emptyPos.y * 3 + emptyPos.x; // Converte grid para índice
        Vector2 targetPos = fixedPositions[targetSlot];
        
        Debug.Log($"Peça vai para slot {targetSlot} na posição {targetPos}");
        
        // ✅ ATUALIZA O GRID
        grid[piecePos.x, piecePos.y] = null;
        grid[emptyPos.x, emptyPos.y] = piece;
        piece.SetGridPosition(emptyPos);
        emptyPos = piecePos;
        
        // ✅ MOVIMENTO VISUAL DIRETO
        RectTransform rect = piece.GetComponent<RectTransform>();
        Vector2 oldPos = rect.anchoredPosition;
        
        Debug.Log($"Posição antes: {oldPos}");
        Debug.Log($"Posição depois: {targetPos}");
        
        // ✅ TESTA PRIMEIRO MOVIMENTO INSTANTÂNEO
        rect.anchoredPosition = targetPos;
        
        // ✅ FORÇA UPDATE
        rect.gameObject.SetActive(false);
        rect.gameObject.SetActive(true);
        
        Debug.Log($"Posição aplicada: {rect.anchoredPosition}");
        Debug.Log($"Posição real agora: {rect.anchoredPosition}");
        
        // ✅ INICIA ANIMAÇÃO SUAVE DEPOIS
        if (Vector2.Distance(oldPos, targetPos) > 1f)
        {
            StartCoroutine(AnimateSmooth(rect, oldPos, targetPos));
        }
        
        DebugGrid();
        CheckWin();
    }

    IEnumerator AnimateSmooth(RectTransform rect, Vector2 start, Vector2 target)
    {
        float time = 0;
        float duration = 0.2f;
        
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            
            Vector2 pos = Vector2.Lerp(start, target, t);
            rect.anchoredPosition = pos;
            
            yield return null;
        }
        
        rect.anchoredPosition = target;
        Debug.Log($"Animação finalizada em: {rect.anchoredPosition}");
    }

    void CheckWin()
    {
        // Verifica se está na ordem: 1 2 [vazio] / 3 4 5
        if (GetIDAt(0, 0) == 1 && GetIDAt(1, 0) == 2 && IsEmptyAt(2, 0) &&
            GetIDAt(0, 1) == 3 && GetIDAt(1, 1) == 4 && GetIDAt(2, 1) == 5)
        {
            Debug.Log("🎉 PUZZLE COMPLETADO!");
            
            // ✅ DESABILITA MOVIMENTOS
            canMove = false;
            
            // ✅ MOSTRA A PEÇA DO ESPAÇO VAZIO PARA COMPLETAR A IMAGEM
            ShowEmptyPiece();
            
            // ✅ INICIA SEQUÊNCIA DE CONCLUSÃO
            StartCoroutine(CompletionSequence());
        }
    }

    void ShowEmptyPiece()
    {
        // Encontra e mostra a peça do espaço vazio
        foreach (var piece in puzzlePieces)
        {
            if (piece != null && piece.pieceID == 0)
            {
                // Calcula a posição do espaço vazio atual
                int emptySlot = emptyPos.y * 3 + emptyPos.x;
                Vector2 emptyRealPos = fixedPositions[emptySlot];
                
                Debug.Log($"Mostrando peça do espaço vazio na posição {emptyRealPos}");
                
                piece.gameObject.SetActive(true);
                piece.GetComponent<RectTransform>().anchoredPosition = emptyRealPos;
                break;
            }
        }
    }

    IEnumerator CompletionSequence()
    {
        Debug.Log("⏰ SEQUÊNCIA DE CONCLUSÃO INICIADA");
        
        // ✅ ESPERA 2 SEGUNDOS COM A IMAGEM COMPLETA
        Debug.Log("Aguardando 2 segundos...");
        yield return new WaitForSeconds(2f);
        
        // ✅ MOSTRA O PAINEL DA PINTURA COMPLETA
        Debug.Log("📜 Mostrando painel da pintura completa");
        completePanel.SetActive(true);
        
        // ✅ ESPERA MAIS 2 SEGUNDOS
        Debug.Log("Aguardando mais 2 segundos...");
        yield return new WaitForSeconds(2f);
        
        // ✅ SAI DA CENA AUTOMATICAMENTE
        Debug.Log("🚪 Saindo da cena automaticamente");
        ExitPuzzleScene();
    }

    void ExitPuzzleScene()
    {
        // ✅ CHAMA O MÉTODO DO SCRIPT PRINCIPAL PARA SAIR
        InteractPuzzle mainScript = FindFirstObjectByType<InteractPuzzle>();
        if (mainScript != null)
        {
            Debug.Log("Chamando OnPuzzleComplete() do script principal");
            mainScript.OnPuzzleComplete();
        }
        else
        {
            Debug.LogWarning("Script InteractPuzzle não encontrado!");
        }
    }

    int GetIDAt(int x, int y)
    {
        var piece = grid[x, y];
        return piece != null ? piece.pieceID : 0;
    }

    bool IsEmptyAt(int x, int y)
    {
        return grid[x, y] == null;
    }

    // ========================================
    // DEBUG
    // ========================================

    [ContextMenu("Debug Grid")]
    public void DebugGrid()
    {
        Debug.Log("📊 GRID ATUAL:");
        
        string line0 = "";
        string line1 = "";
        
        for (int x = 0; x < 3; x++)
        {
            var p0 = grid[x, 0];
            var p1 = grid[x, 1];
            
            line0 += (p0 != null ? p0.pieceID.ToString() : "0") + " ";
            line1 += (p1 != null ? p1.pieceID.ToString() : "0") + " ";
        }
        
        Debug.Log($"Superior: {line0}");
        Debug.Log($"Inferior: {line1}");
    }

    [ContextMenu("Teste Movimento Direto")]
    public void TestDirectMovement()
    {
        if (puzzlePieces.Length > 0 && puzzlePieces[0] != null)
        {
            Debug.Log("🧪 TESTE DE MOVIMENTO DIRETO");
            
            RectTransform rect = puzzlePieces[0].GetComponent<RectTransform>();
            Vector2 oldPos = rect.anchoredPosition;
            Vector2 testPos = new Vector2(oldPos.x + 100, oldPos.y);
            
            Debug.Log($"Movendo de {oldPos} para {testPos}");
            
            rect.anchoredPosition = testPos;
            
            Debug.Log($"Posição após movimento: {rect.anchoredPosition}");
            
            // Volta para posição original
            StartCoroutine(ReturnToOriginal(rect, testPos, oldPos));
        }
    }

    IEnumerator ReturnToOriginal(RectTransform rect, Vector2 from, Vector2 to)
    {
        yield return new WaitForSeconds(1f);
        
        Debug.Log($"Retornando de {from} para {to}");
        rect.anchoredPosition = to;
        Debug.Log($"Posição final: {rect.anchoredPosition}");
    }

    [ContextMenu("Mostrar Posições Capturadas")]
    public void ShowCapturedPositions()
    {
        Debug.Log("📍 POSIÇÕES CAPTURADAS:");
        for (int i = 0; i < fixedPositions.Length; i++)
        {
            Debug.Log($"Slot {i}: {fixedPositions[i]}");
        }
    }
}