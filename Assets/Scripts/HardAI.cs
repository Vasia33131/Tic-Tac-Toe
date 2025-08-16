using System.Collections.Generic;
using UnityEngine;

public class HardAI : AIPlayer
{
    public override void MakeMove(string[] board)
    {
        // 1. Попытаться выиграть
        int? winMove = FindWinningMove(board, AISymbol);
        if (winMove.HasValue)
        {
            gameController.ProcessAIMove(winMove.Value);
            return;
        }

        // 2. Блокировать игрока
        int? blockMove = FindWinningMove(board, PlayerSymbol);
        if (blockMove.HasValue)
        {
            gameController.ProcessAIMove(blockMove.Value);
            return;
        }

        // 3. Захватить центр
        if (string.IsNullOrEmpty(board[4]))
        {
            gameController.ProcessAIMove(4);
            return;
        }

        // 4. Захватить угол
        int[] corners = { 0, 2, 6, 8 };
        foreach (int corner in corners)
        {
            if (string.IsNullOrEmpty(board[corner]))
            {
                gameController.ProcessAIMove(corner);
                return;
            }
        }

        // 5. Случайный ход
        List<int> emptyCells = GetEmptyCells(board);
        if (emptyCells.Count > 0)
        {
            int move = emptyCells[Random.Range(0, emptyCells.Count)];
            gameController.ProcessAIMove(move);
        }
    }
}