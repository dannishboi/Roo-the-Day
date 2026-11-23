
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TicTacToeGame : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField]
    private TicTacToeCell[] cells =
        new TicTacToeCell[9];

    [SerializeField] private float computerDelay = 0.7f;

    [Header("Win Display")]
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private Transform winBar;

    [SerializeField] private float barThickness = 0.06f;
    [SerializeField] private float barExtraLength = 0.3f;
    [SerializeField] private float autoResetDelay = 3f;

    private Vector3 winBarOffsetLocal;

    private int[] board = new int[9];
    private bool playerTurn = true;
    private bool gameOver = false;

    // All eight possible winning combinations.
    private readonly int[,] winningLines =
    {
        {0, 1, 2},
        {3, 4, 5},
        {6, 7, 8},
        {0, 3, 6},
        {1, 4, 7},
        {2, 5, 8},
        {0, 4, 8},
        {2, 4, 6}
    };

    void Start()
    {
        // Remember the bar's initial position relative to
        // the center cell, including its depth in front
        // of the board.
        if (winBar != null && cells[4] != null)
        {
            Vector3 offset =
                winBar.position - cells[4].transform.position;

            winBarOffsetLocal =
                transform.InverseTransformVector(offset);
        }

        ResetBoard();
    }

    public bool TryPlayerMove(int index)
    {
        // Only allow moves during the player's turn.
        if (gameOver || !playerTurn)
            return false;

        if (index < 0 || index >= 9 || board[index] != 0)
            return false;

        // Player always uses X.
        board[index] = 1;
        cells[index].ShowMark("X");

        Debug.Log("Player chose square " + index);

        playerTurn = false;

        if (!CheckGameOver())
        {
            StartCoroutine(ComputerTurn());
        }

        return true;
    }

    IEnumerator ComputerTurn()
    {
        yield return new WaitForSeconds(computerDelay);

        if (gameOver)
            yield break;

        int move = ChooseComputerMove();

        if (move == -1)
            yield break;

        // Computer always uses O.
        board[move] = 2;
        cells[move].ShowMark("O");

        Debug.Log("Computer chose square " + move);

        if (!CheckGameOver())
        {
            playerTurn = true;
        }
    }

    int ChooseComputerMove()
    {
        // Try to win first.
        int move = FindWinningMove(2);

        if (move != -1)
            return move;

        // Block the player's immediate win.
        move = FindWinningMove(1);

        if (move != -1)
            return move;

        // Prefer the center square.
        if (board[4] == 0)
            return 4;

        // Otherwise choose a random empty square.
        List<int> available = new List<int>();

        for (int i = 0; i < 9; i++)
        {
            if (board[i] == 0)
                available.Add(i);
        }

        if (available.Count == 0)
            return -1;

        return available[Random.Range(0, available.Count)];
    }

    int FindWinningMove(int mark)
    {
        for (int i = 0; i < 9; i++)
        {
            if (board[i] != 0)
                continue;

            board[i] = mark;

            bool wins = HasWon(mark);

            board[i] = 0;

            if (wins)
                return i;
        }

        return -1;
    }

    bool HasWon(int mark)
    {
        return FindWinningLine(mark, out _, out _);
    }

    // Find the first and last cells of a winning line.
    bool FindWinningLine(
        int mark,
        out int startCell,
        out int endCell)
    {
        for (int i = 0; i < 8; i++)
        {
            int a = winningLines[i, 0];
            int b = winningLines[i, 1];
            int c = winningLines[i, 2];

            if (board[a] == mark &&
                board[b] == mark &&
                board[c] == mark)
            {
                startCell = a;
                endCell = c;
                return true;
            }
        }

        startCell = -1;
        endCell = -1;
        return false;
    }

    bool CheckGameOver()
    {
        // Player wins
        if (FindWinningLine(1, out int a, out int c))
        {
            EndGame("PLAYER WINS!", a, c);
            return true;
        }

        // Computer wins
        if (FindWinningLine(2, out a, out c))
        {
            EndGame("COMPUTER WINS!", a, c);

            StartCoroutine(ResetAfterDelay());

            return true;
        }

        // Check for draw
        foreach (int square in board)
        {
            if (square == 0)
                return false;
        }

        gameOver = true;
        ShowResult("DRAW!");

        Debug.Log("Game ended in a draw.");

        StartCoroutine(ResetAfterDelay());

        return true;
    }

    void EndGame(
        string message,
        int startCell,
        int endCell)
    {
        gameOver = true;

        ShowResult(message);
        DrawWinBar(startCell, endCell);

        Debug.Log(message);
    }

    void ShowResult(string message)
    {
        if (resultText != null)
        {
            resultText.text = message;
        }
    }

    // Position, rotate, and resize the bar using
    // the winning cells' actual world positions.
    // This works even when the board is rotated.
    void DrawWinBar(int startCell, int endCell)
    {
        if (winBar == null)
            return;

        Vector3 start =
            cells[startCell].transform.position;

        Vector3 end =
            cells[endCell].transform.position;

        Vector3 direction = end - start;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        // Keep the bar in front of the board.
        Vector3 offset =
            transform.TransformVector(winBarOffsetLocal);

        // Move the bar to the center of the winning line.
        winBar.position =
            (start + end) / 2f + offset;

        // Align the bar's X-axis with the winning line.
        winBar.rotation =
            Quaternion.FromToRotation(
                Vector3.right,
                direction.normalized
            );

        // Extend across all three winning cells.
        winBar.localScale = new Vector3(
            direction.magnitude + barExtraLength,
            barThickness,
            barThickness
        );

        winBar.gameObject.SetActive(true);
    }

    public void ResetBoard()
    {
        StopAllCoroutines();

        board = new int[9];

        playerTurn = true;
        gameOver = false;

        // Clear all X and O markers.
        foreach (TicTacToeCell cell in cells)
        {
            if (cell != null)
            {
                cell.ShowMark("");
            }
        }

        // Hide the result message.
        if (resultText != null)
        {
            resultText.text = "";
        }

        // Hide the winning bar until the next win.
        if (winBar != null)
        {
            winBar.gameObject.SetActive(false);
        }

        Debug.Log("New game! Player goes first.");
    }

    IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(autoResetDelay);

        ResetBoard();
    }
}
