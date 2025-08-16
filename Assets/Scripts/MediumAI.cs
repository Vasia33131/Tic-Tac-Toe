using System.Collections.Generic;
using UnityEngine;

public class MediumAI : AIPlayer
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

        // 3. Случайный ход
        List<int> emptyCells = GetEmptyCells(board);
        if (emptyCells.Count > 0)
        {
            int move = emptyCells[Random.Range(0, emptyCells.Count)];
            gameController.ProcessAIMove(move);
        }
    }
}