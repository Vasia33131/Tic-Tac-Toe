using System.Collections.Generic;
using UnityEngine;

public class ExpertAI : AIPlayer
{
    public override void MakeMove(string[] board)
    {
        int bestScore = int.MinValue;
        int bestMove = -1;

        List<int> emptyCells = GetEmptyCells(board);

        foreach (int cell in emptyCells)
        {
            board[cell] = AISymbol;
            int score = Minimax(board, 0, false);
            board[cell] = "";

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = cell;
            }
        }

        if (bestMove != -1)
        {
            gameController.ProcessAIMove(bestMove);
        }
    }

    private int Minimax(string[] board, int depth, bool isMaximizing)
    {
        if (CheckWin(board, AISymbol)) return 10 - depth;
        if (CheckWin(board, PlayerSymbol)) return depth - 10;
        if (CheckDraw(board)) return 0;

        if (isMaximizing)
        {
            int bestScore = int.MinValue;
            List<int> emptyCells = GetEmptyCells(board);

            foreach (int cell in emptyCells)
            {
                board[cell] = AISymbol;
                int score = Minimax(board, depth + 1, false);
                board[cell] = "";
                bestScore = Mathf.Max(score, bestScore);
            }
            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;
            List<int> emptyCells = GetEmptyCells(board);

            foreach (int cell in emptyCells)
            {
                board[cell] = PlayerSymbol;
                int score = Minimax(board, depth + 1, true);
                board[cell] = "";
                bestScore = Mathf.Min(score, bestScore);
            }
            return bestScore;
        }
    }

    private bool CheckWin(string[] board, string player)
    {
        int[,] winCombinations = {
            {0, 1, 2}, {3, 4, 5}, {6, 7, 8},
            {0, 3, 6}, {1, 4, 7}, {2, 5, 8},
            {0, 4, 8}, {2, 4, 6}
        };

        for (int i = 0; i < 8; i++)
        {
            int a = winCombinations[i, 0];
            int b = winCombinations[i, 1];
            int c = winCombinations[i, 2];

            if (board[a] == player && board[b] == player && board[c] == player)
                return true;
        }
        return false;
    }

    private bool CheckDraw(string[] board)
    {
        foreach (string cell in board)
        {
            if (string.IsNullOrEmpty(cell))
                return false;
        }
        return true;
    }
}