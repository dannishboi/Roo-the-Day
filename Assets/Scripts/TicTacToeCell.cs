using UnityEngine;
using TMPro;

public class TicTacToeCell : MonoBehaviour
{
    [SerializeField] private TicTacToeGame game;
    [SerializeField, Range(0, 8)] private int cellIndex;
    [SerializeField] private TMP_Text markText;

    public void TrySelect()
    {
        if (game != null)
        {
            game.TryPlayerMove(cellIndex);
        }
    }

    public void ShowMark(string mark)
    {
        if (markText != null)
        {
            markText.text = mark;
        }
    }
}