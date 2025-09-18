// 2. PuzzleManagerUI.cs - Gerencia todo o sistema do puzzle em UI
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManagerUI : MonoBehaviour
{
    [Header("Configura��es do Grid")]
    public int gridWidth = 3;
    public int gridHeight = 2;
    public float pieceSize = 300f; // tamanho de cada pe�a em pixels
    public float pieceSpacing = 10f; // espa�o entre pe�as

    [Header("Pe�as do Puzzle")]
    public PuzzlePieceUI[] puzzlePieces; // array com todas as pe�as (incluindo o espa�o vazio)

    [Header("Container")]
    public RectTransform puzzleContainer; // GameObject pai que cont�m todas as pe�as

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip movePieceSound;
    public AudioClip completePuzzleSound;

    [Header("UI References")]
    public GameObject completePanel; // refer�ncia ao gerenciador de UI

    private PuzzlePieceUI[,] grid; // grid 2D para controlar posi��es
    private Vector2Int emptyPosition; // posi��o do espa�o vazio
    private bool isMoving = false;
    private InteractPuzzle mainPuzzleScript;

    void Start()
    {
        completePanel.SetActive(false);
        // Encontra o script principal do puzzle
        mainPuzzleScript = FindFirstObjectByType<InteractPuzzle>();

        InitializePuzzle();
    }

    void InitializePuzzle()
    {
        grid = new PuzzlePieceUI[gridWidth, gridHeight];

        // Configura as pe�as nas posi��es onde j� foram colocadas manualmente
        for (int i = 0; i < puzzlePieces.Length; i++)
        {
            if (puzzlePieces[i] == null) continue;

            // Pega a posi��o atual da pe�a (onde voc� colocou manualmente)
            Vector2 currentPos = puzzlePieces[i].GetComponent<RectTransform>().anchoredPosition;

            // Calcula qual posi��o do grid esta pe�a est� ocupando
            Vector2Int gridPos = GetGridPositionFromWorldPos(currentPos);

            // Configura a pe�a
            puzzlePieces[i].SetGridPosition(gridPos);
            puzzlePieces[i].correctPosition = puzzlePieces[i].pieceID; // posi��o correta baseada no ID

            // Adiciona ao grid
            if (gridPos.x >= 0 && gridPos.x < gridWidth && gridPos.y >= 0 && gridPos.y < gridHeight)
            {
                grid[gridPos.x, gridPos.y] = puzzlePieces[i];
            }

            // Encontra o espa�o vazio (pe�a com ID 0)
            if (puzzlePieces[i].pieceID == 0)
            {
                emptyPosition = gridPos;
                puzzlePieces[i].gameObject.SetActive(false); // esconde o espa�o vazio
            }
        }

        // Se n�o encontrou espa�o vazio nas pe�as existentes, define como �ltima posi��o
        if (emptyPosition == Vector2Int.zero)
        {
            emptyPosition = new Vector2Int(gridWidth - 1, gridHeight - 1); // canto inferior direito
        }

        Debug.Log($"Puzzle inicializado. Espa�o vazio em: {emptyPosition}");
    }

    void CenterPuzzleContainer()
    {
        if (puzzleContainer != null)
        {
            // Calcula o tamanho total do puzzle
            float totalWidth = gridWidth * pieceSize + (gridWidth - 1) * pieceSpacing;
            float totalHeight = gridHeight * pieceSize + (gridHeight - 1) * pieceSpacing;

            // Configura o tamanho do container
            puzzleContainer.sizeDelta = new Vector2(totalWidth, totalHeight);
        }
    }


    // Adiciona m�todo para converter posi��o UI em posi��o do grid
    Vector2Int GetGridPositionFromWorldPos(Vector2 worldPos)
    {
        // Calcula baseado na posi��o UI atual
        int x = Mathf.RoundToInt(worldPos.x / (pieceSize + pieceSpacing));
        int y = Mathf.RoundToInt(-worldPos.y / (pieceSize + pieceSpacing)); // negativo porque UI Y cresce para baixo

        // Garante que est� dentro dos limites
        x = Mathf.Clamp(x, 0, gridWidth - 1);
        y = Mathf.Clamp(y, 0, gridHeight - 1);

        return new Vector2Int(x, y);
    }

    public void OnPieceClicked(PuzzlePieceUI piece)
    {
        if (isMoving) return;

        Vector2Int piecePos = piece.GetGridPosition();

        // Verifica se a pe�a pode se mover (est� adjacente ao espa�o vazio)
        if (CanMovePiece(piecePos))
        {
            MovePieceToEmpty(piecePos, true);

            if (audioSource && movePieceSound)
                audioSource.PlayOneShot(movePieceSound);
        }
    }

    bool CanMovePiece(Vector2Int piecePos)
    {
        // Verifica se a pe�a est� adjacente ao espa�o vazio
        int distX = Mathf.Abs(piecePos.x - emptyPosition.x);
        int distY = Mathf.Abs(piecePos.y - emptyPosition.y);

        return (distX == 1 && distY == 0) || (distX == 0 && distY == 1);
    }

    void MovePieceToEmpty(Vector2Int piecePos, bool animate)
    {
        if (!animate)
        {
            // Movimento instant�neo (para embaralhamento)
            PuzzlePieceUI piece = grid[piecePos.x, piecePos.y];

            // Troca posi��es no grid
            grid[emptyPosition.x, emptyPosition.y] = piece;
            grid[piecePos.x, piecePos.y] = null;

            // Atualiza posi��es
            piece.SetGridPosition(emptyPosition);
            piece.GetComponent<RectTransform>().anchoredPosition = GetUIPosition(emptyPosition.x, emptyPosition.y);

            emptyPosition = piecePos;
        }
        else
        {
            // Movimento com anima��o
            StartCoroutine(MovePieceAnimated(piecePos));
        }
    }

    IEnumerator MovePieceAnimated(Vector2Int piecePos)
    {
        isMoving = true;

        PuzzlePieceUI piece = grid[piecePos.x, piecePos.y];
        Vector2 targetPos = GetUIPosition(emptyPosition.x, emptyPosition.y);

        // Move a pe�a
        piece.MoveTo(targetPos);

        // Atualiza o grid
        grid[emptyPosition.x, emptyPosition.y] = piece;
        grid[piecePos.x, piecePos.y] = null;

        // Atualiza posi��es
        piece.SetGridPosition(emptyPosition);
        emptyPosition = piecePos;

        // Espera a anima��o terminar
        yield return new WaitForSeconds(0.2f);

        isMoving = false;

        // Verifica se o puzzle foi completado
        if (IsPuzzleComplete())
        {
            OnPuzzleCompleted();
        }
    }

    List<Vector2Int> GetValidMoves()
    {
        List<Vector2Int> validMoves = new List<Vector2Int>();

        // Verifica todas as dire��es adjacentes ao espa�o vazio
        Vector2Int[] directions = {
            new Vector2Int(0, 1),  // cima
            new Vector2Int(0, -1), // baixo
            new Vector2Int(1, 0),  // direita
            new Vector2Int(-1, 0)  // esquerda
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int checkPos = emptyPosition + dir;

            if (checkPos.x >= 0 && checkPos.x < gridWidth &&
                checkPos.y >= 0 && checkPos.y < gridHeight)
            {
                validMoves.Add(checkPos);
            }
        }

        return validMoves;
    }

    Vector2 GetUIPosition(int x, int y)
    {
        // Calcula a posi��o UI baseada no grid
        float posX = x * (pieceSize + pieceSpacing);
        float posY = -y * (pieceSize + pieceSpacing); // negativo porque UI Y cresce para baixo

        return new Vector2(posX, posY);
    }

    bool IsPuzzleComplete()
    {
        // Verifica se todas as pe�as est�o na posi��o correta
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                PuzzlePieceUI piece = grid[x, y];

                if (piece != null) // n�o verifica o espa�o vazio
                {
                    // A posi��o correta � baseada no pieceID da pe�a
                    int expectedID = y * gridWidth + x + 1; // +1 porque IDs come�am em 1, n�o 0

                    // Para o formato 3x2, as posi��es corretas s�o:
                    // (0,0)=1, (1,0)=2, (2,0)=3
                    // (0,1)=4, (1,1)=5, (2,1)=vazio

                    if (piece.pieceID != expectedID)
                        return false;
                }
            }
        }

        return true;
    }

    void OnPuzzleCompleted()
    {
        Debug.Log("Puzzle Completado!");
        completePanel.SetActive(true);
        // Mostra o espa�o vazio (�ltima pe�a)
        PuzzlePieceUI emptyPiece = null;
        foreach (PuzzlePieceUI piece in puzzlePieces)
        {
            if (piece.pieceID == 0)
            {
                emptyPiece = piece;
                break;
            }
        }

        if (emptyPiece != null)
        {
            emptyPiece.gameObject.SetActive(true);
            emptyPiece.GetComponent<RectTransform>().anchoredPosition = GetUIPosition(emptyPosition.x, emptyPosition.y);
        }

        if (audioSource && completePuzzleSound)
            audioSource.PlayOneShot(completePuzzleSound);

        // Chama o m�todo do script principal
        if (mainPuzzleScript != null)
        {
            StartCoroutine(CompletePuzzleDelay());
        }
    }

    IEnumerator CompletePuzzleDelay()
    {
        yield return new WaitForSeconds(1f); // pequeno delay para o jogador ver o puzzle completo
        mainPuzzleScript.OnPuzzleComplete();
    }

    void Update()
    {
        // Opcional: highlight das pe�as que podem se mover
        if (!isMoving)
        {
            HighlightMovablePieces();
        }
    }

    void HighlightMovablePieces()
    {
        List<Vector2Int> validMoves = GetValidMoves();

        // Remove highlight de todas as pe�as
        foreach (PuzzlePieceUI piece in puzzlePieces)
        {
            if (piece.pieceID != 0)
                piece.SetHighlight(false);
        }

        // Adiciona highlight nas pe�as que podem se mover
        foreach (Vector2Int pos in validMoves)
        {
            PuzzlePieceUI piece = grid[pos.x, pos.y];
            if (piece != null)
                piece.SetHighlight(true);
        }
    }
}
